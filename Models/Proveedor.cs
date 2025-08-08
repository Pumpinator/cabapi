using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Proveedor
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string[] Contacto { get; set; }

    public string? Producto { get; set; } // Producto principal que ofrece

    public bool Activo { get; set; } = true;

    [JsonIgnore]
    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
