using cabapi.DTOs;
using cabapi.Models;
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

    public UsuariosController(CABDB db)
    {
        _db = db;
    }

    [HttpPost("ingresar")]
    public async Task<ActionResult> Login([FromBody] LoginDTO request)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == request.Correo);

        if (usuario == null || !VerifyPassword(request.Password, usuario.Password))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        var token = GenerateToken(usuario);

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
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }
        return usuario;
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(new { message = "Token inválido" });
        }

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

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var currentUserId) && currentUserId == id)
        {
            var token = GenerateToken(usuario);
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

    private string GenerateToken(Usuario usuario)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var key = Encoding.ASCII.GetBytes(secretKey);

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
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));
        }

        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
}