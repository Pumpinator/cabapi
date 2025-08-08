using System.Text.Json.Serialization;

namespace cabapi.Models;

public class ProductoMateriaPrima
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public int MateriaPrimaId { get; set; }

    public decimal CantidadRequerida { get; set; }

    public string? UnidadMedida { get; set; }

    [JsonIgnore]
    public virtual Producto Producto { get; set; } = null!;

    [JsonIgnore]
    public virtual MateriaPrima MateriaPrima { get; set; } = null!;
}
