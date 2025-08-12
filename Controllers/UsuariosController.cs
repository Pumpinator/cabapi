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

        usuario.FechaUltimoAcceso = DateTime.Now;
        usuario.EnLinea = true;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            token,
            usuario = new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                correo = usuario.Correo,
                rol = usuario.Rol.ToString()
            }
        });
    }

    [HttpPost("salir")]
    [Authorize]
    public async Task<IActionResult> Logout()
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

        usuario.EnLinea = false;
        usuario.FechaUltimoAcceso = DateTime.Now;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Sesión cerrada correctamente" });
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
    {
        var currentUserRole = GetCurrentUserRole();

        // Solo admins y superadmins pueden ver todos los usuarios
        if (currentUserRole == Rol.Cliente)
        {
            return Forbid("No tienes permisos para ver la lista de usuarios");
        }

        var usuarios = await _db.Usuarios
            .Where(u => u.Activo) // Solo usuarios activos
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        // Ocultar contraseñas en la respuesta
        foreach (var usuario in usuarios)
        {
            usuario.Password = "";
        }

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var currentUserRole = GetCurrentUserRole();
        var currentUserId = GetCurrentUserId();

        var usuario = await _db.Usuarios.FindAsync(id);

        if (usuario == null || !usuario.Activo)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        // Los clientes solo pueden ver su propia información
        if (currentUserRole == Rol.Cliente && currentUserId != id)
        {
            return Forbid("Solo puedes ver tu propia información");
        }

        // Los admins pueden ver clientes y otros admins, pero no superadmins
        if (currentUserRole == Rol.Admin && usuario.Rol == Rol.SuperAdmin)
        {
            return Forbid("No tienes permisos para ver este usuario");
        }

        // Ocultar contraseña en la respuesta
        usuario.Password = "";

        return Ok(usuario);
    }


    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<PerfilDTO>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(new { message = "Token inválido" });
        }

        var usuario = await _db.Usuarios.FindAsync(userId);

        if (usuario == null || !usuario.Activo)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        var perfil = new PerfilDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Rol = usuario.Rol.ToString(),
            FechaCreacion = usuario.FechaCreacion,
            FechaUltimoAcceso = usuario.FechaUltimoAcceso,
            EnLinea = usuario.EnLinea,
            Activo = usuario.Activo
        };

        return Ok(perfil);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Register([FromBody] RegistroDTO request)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo))
        {
            return BadRequest(new { message = "El correo ya está registrado" });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Password = HashPassword(request.Password),
            Rol = Rol.Cliente
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        usuario.Password = "";
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPost("admin")]
    [Authorize]
    public async Task<ActionResult<Usuario>> CreateUserByAdmin([FromBody] RegistroDTO request)
    {
        // Verificar que el usuario autenticado sea admin o superadmin
        var currentUserRole = GetCurrentUserRole();
        if (currentUserRole != Rol.Admin && currentUserRole != Rol.SuperAdmin)
        {
            return Forbid("No tienes permisos para crear usuarios");
        }

        // Los admins solo pueden crear clientes y otros admins
        if (currentUserRole == Rol.Admin && request.Rol == Rol.SuperAdmin)
        {
            return BadRequest(new { message = "Los administradores no pueden crear superadministradores" });
        }

        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo))
        {
            return BadRequest(new { message = "El correo ya está registrado" });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Password = HashPassword(request.Password),
            Rol = request.Rol
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        // No devolver la contraseña en la respuesta
        usuario.Password = "";
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPost("superadmin")]
    [Authorize]
    public async Task<ActionResult<Usuario>> CreateUserBySuperAdmin([FromBody] RegistroDTO request)
    {
        // Verificar que el usuario autenticado sea superadmin
        var currentUserRole = GetCurrentUserRole();
        if (currentUserRole != Rol.SuperAdmin)
        {
            return Forbid("Solo los superadministradores pueden usar este endpoint");
        }

        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo))
        {
            return BadRequest(new { message = "El correo ya está registrado" });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Password = HashPassword(request.Password),
            Rol = request.Rol // Los superadmins pueden crear cualquier tipo de usuario
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        // No devolver la contraseña en la respuesta
        usuario.Password = "";
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] Usuario request)
    {
        var currentUserRole = GetCurrentUserRole();
        var currentUserId = GetCurrentUserId();

        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        // Los clientes solo pueden actualizar su propia información
        if (currentUserRole == Rol.Cliente && currentUserId != id)
        {
            return Forbid("Solo puedes actualizar tu propia información");
        }

        // Los admins pueden actualizar clientes y otros admins, pero no superadmins
        if (currentUserRole == Rol.Admin && usuario.Rol == Rol.SuperAdmin)
        {
            return Forbid("Los administradores no pueden modificar superadministradores");
        }

        // Los clientes no pueden cambiar su rol
        if (currentUserRole == Rol.Cliente && request.Rol != usuario.Rol)
        {
            return BadRequest(new { message = "No puedes cambiar tu propio rol" });
        }

        // Los admins no pueden crear o convertir usuarios en superadmins
        if (currentUserRole == Rol.Admin && request.Rol == Rol.SuperAdmin)
        {
            return BadRequest(new { message = "Los administradores no pueden crear o asignar el rol de superadministrador" });
        }

        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo && u.Id != id))
        {
            return BadRequest(new { message = "El correo ya está registrado por otro usuario" });
        }

        usuario.Nombre = request.Nombre;
        usuario.Correo = request.Correo;

        // Solo actualizar el rol si el usuario tiene permisos
        if (currentUserRole == Rol.SuperAdmin || (currentUserRole == Rol.Admin && request.Rol != Rol.SuperAdmin))
        {
            usuario.Rol = request.Rol;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            usuario.Password = HashPassword(request.Password);
        }

        await _db.SaveChangesAsync();

        // Si el usuario actual actualizó su propia información, generar nuevo token
        if (currentUserId == id)
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
        var currentUserRole = GetCurrentUserRole();
        var currentUserId = GetCurrentUserId();

        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        // Los usuarios solo pueden eliminar su propia cuenta
        if (currentUserRole == Rol.Cliente && currentUserId != id)
        {
            return Forbid("Solo puedes eliminar tu propia cuenta");
        }

        // Los admins pueden eliminar clientes y otros admins, pero no superadmins
        if (currentUserRole == Rol.Admin && usuario.Rol == Rol.SuperAdmin)
        {
            return Forbid("Los administradores no pueden eliminar superadministradores");
        }

        // Los superadmins pueden eliminar cualquier usuario excepto a sí mismos si son el último superadmin
        if (currentUserRole == Rol.SuperAdmin && usuario.Rol == Rol.SuperAdmin)
        {
            var superAdminCount = await _db.Usuarios.CountAsync(u => u.Rol == Rol.SuperAdmin && u.Activo);
            if (superAdminCount <= 1)
            {
                return BadRequest(new { message = "No se puede eliminar el último superadministrador del sistema" });
            }
        }

        usuario.Activo = false; // Soft delete
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private string GenerateToken(Usuario usuario)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT_SECRET_KEY no está configurado");
        }

        var key = Encoding.ASCII.GetBytes(secretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
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

    private Rol GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleClaim))
        {
            return Rol.Cliente; // Default fallback
        }

        if (Enum.TryParse<Rol>(roleClaim, out var role))
        {
            return role;
        }

        return Rol.Cliente; // Default fallback
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return 0; // Valor por defecto que indica error
        }
        return userId;
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