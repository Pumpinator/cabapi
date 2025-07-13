using cabapi.Models;
using Empresa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace cabapi.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly CABDB _db;
    private readonly IConfiguration _configuration;

    public UsuariosController(CABDB db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    [HttpPost("ingresar")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == request.Correo);

        if (usuario == null || !VerifyPassword(request.Password, usuario.Password))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        var token = GenerateJwtToken(usuario);

        return Ok(new
        {
            token,
            usuario = new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                correo = usuario.Correo
            }
        });
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
    {
        return Ok(await _db.Usuarios
            .OrderBy(u => u.Nombre)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }


    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<Usuario>> GetProfile()
    {
        var userId = GetCurrentUserId();
        var usuario = await _db.Usuarios.FindAsync(userId);

        if (usuario == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Register([FromBody] Usuario request)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo))
        {
            return BadRequest(new { message = "El correo ya está registrado" });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Password = HashPassword(request.Password)
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] Usuario request)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo && u.Id != id))
        {
            return BadRequest(new { message = "El correo ya está registrado por otro usuario" });
        }

        usuario.Nombre = request.Nombre;
        usuario.Correo = request.Correo;
        if (!string.IsNullOrEmpty(request.Password))
        {
            usuario.Password = HashPassword(request.Password);
        }
        
        await _db.SaveChangesAsync();

        if (GetCurrentUserId() == id)
        {
            var token = GenerateJwtToken(usuario);
            Response.Headers["Authorization"] = $"Bearer {token}";
        }


        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        await _db.SaveChangesAsync();

        return NoContent();
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ??
        "CAB_Secret_Key_2025_UTL_Very_Long_Secret_Key_For_Security_Must_Be_At_Least_32_Characters"
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"] ?? "CAB-API",
            Audience = jwtSettings["Audience"] ?? "CAB-Client"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        return Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashToVerify = HashPassword(password);
        return hashToVerify == hashedPassword;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }
}

public class LoginRequest
{
    public required string Correo { get; set; }
    public required string Password { get; set; }
}