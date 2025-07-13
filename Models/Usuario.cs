using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Usuario
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string Correo { get; set; }

    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaUltimoAcceso { get; set; }

    public bool EnLinea { get; set; } = false;

    public bool Activo { get; set; } = true;

    
}
