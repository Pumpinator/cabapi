using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Usuario
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string Correo { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaUltimoAcceso { get; set; }

    public bool EnLinea { get; set; } = false;

    public bool Activo { get; set; } = true;

    public Rol Rol { get; set; } = Rol.Cliente;

    [JsonIgnore]
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

    [JsonIgnore]
    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();


}
