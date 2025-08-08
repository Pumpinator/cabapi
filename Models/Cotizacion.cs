using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Cotizacion
{
    public int Id { get; set; }

    public required string NumeroCotizacion { get; set; }

    public DateTime FechaCotizacion { get; set; } = DateTime.Now;

    public required string NombreCliente { get; set; }

    public required string CorreoCliente { get; set; }

    public required string TelefonoCliente { get; set; }

    public string? EmpresaCliente { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Total { get; set; }

    public Estatus Estatus { get; set; } = Estatus.Pendiente; // Pendiente, Aceptada, Rechazada, Expirada

    public DateTime? FechaVencimiento { get; set; }

    public string? Observaciones { get; set; }

    public string? RequirimientosEspeciales { get; set; }

    [JsonIgnore]
    public virtual Producto Producto { get; set; } = null!;
}
