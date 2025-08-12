using System.Text.Json.Serialization;

namespace cabapi.Models;

public class CompraDetalle
{
    public int Id { get; set; }

    public int CompraId { get; set; }

    public int MateriaPrimaId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }

    [JsonIgnore]
    public virtual Compra Compra { get; set; } = null!;

    [JsonIgnore]
    public virtual MateriaPrima MateriaPrima { get; set; } = null!;
}
