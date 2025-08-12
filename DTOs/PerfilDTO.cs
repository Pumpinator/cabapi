using cabapi.DTOs;

public class PerfilDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // Cliente, Admin, SuperAdmin
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaUltimoAcceso { get; set; }
    public bool EnLinea { get; set; } = false;
    public bool Activo { get; set; } = true;
}