using System.Text.Json.Serialization;

namespace cabapi.Models;

public class VentaDetalle
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public int ProductoId { get; set; }

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }

    [JsonIgnore]
    public virtual Venta Venta { get; set; } = null!;

    [JsonIgnore]
    public virtual Producto Producto { get; set; } = null!;
}
