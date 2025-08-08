using cabapi.Models;

namespace cabapi.DTOs;

public class RegistroDTO
{
    public required string Correo { get; set; }
    public required string Password { get; set; }
    public required string Nombre { get; set; }
    public Rol Rol { get; set; } = Rol.Cliente; // Cliente, Admin, SuperAdmin
}