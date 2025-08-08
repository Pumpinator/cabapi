<<<<<<< HEAD
﻿namespace cabapi.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } 
        public int[] ProductoId { get; set; }
    }
=======
using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Venta
{
    public int Id { get; set; }

    public required string NumeroVenta { get; set; }

    public DateTime FechaVenta { get; set; } = DateTime.Now;

    public int UsuarioId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagada, Enviada, Entregada, Cancelada

    public string? DireccionEnvio { get; set; }

    public string? Observaciones { get; set; }

    [JsonIgnore]
    public virtual Usuario Usuario { get; set; } = null!;

    [JsonIgnore]
    public virtual Producto Producto { get; set; } = null!;
>>>>>>> ce01e5a (modelos incompletos)
}
