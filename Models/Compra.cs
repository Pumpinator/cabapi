using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Compra
{
    public int Id { get; set; }

    public required string NumeroCompra { get; set; }

    public DateTime FechaCompra { get; set; } = DateTime.Now;

    public int ProveedorId { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Total { get; set; }

    public Estatus Estatus { get; set; } = Estatus.Pendiente;

    public string? Observaciones { get; set; }

    [JsonIgnore]
    public virtual Proveedor Proveedor { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<CompraDetalle> Detalles { get; set; } = new List<CompraDetalle>();
}
