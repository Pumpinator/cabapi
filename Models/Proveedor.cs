using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Proveedor
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string[] Contacto { get; set; }

    public bool Activo { get; set; } = true;

    [JsonIgnore]
    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();
}
