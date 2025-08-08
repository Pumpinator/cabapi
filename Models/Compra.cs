<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace cabapi.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public int ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedores Proveedor { get; set; } = null!;

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public int Total { get; set; }
    }
=======
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

    public string Estado { get; set; } = "Pendiente"; // Pendiente, Recibida, Cancelada

    public string? Observaciones { get; set; }

    [JsonIgnore]
    public virtual Proveedor Proveedor { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<CompraDetalle> Detalles { get; set; } = new List<CompraDetalle>();
>>>>>>> ce01e5a (modelos incompletos)
}
