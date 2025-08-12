using System.Text.Json.Serialization;

namespace cabapi.Models;

public class MateriaPrima
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string Descripcion { get; set; }

    public required decimal PrecioUnitario { get; set; }

    public decimal Stock { get; set; } = 0;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [JsonIgnore]
    public virtual ICollection<ProductoMateriaPrima> ProductoMateriasPrimas { get; set; } = new List<ProductoMateriaPrima>();

    [JsonIgnore]
    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();
}
