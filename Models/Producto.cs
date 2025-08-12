using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Producto
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string Descripcion { get; set; }

    public required decimal Precio { get; set; }

    public required decimal Costo { get; set; } // Costo de producción

    public int Stock { get; set; } = 0;

    public string? Imagen { get; set; } // URL o path de la imagen en el frontend

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [JsonIgnore]
    public virtual ICollection<ProductoMateriaPrima> ProductoMateriasPrimas { get; set; } = new List<ProductoMateriaPrima>();

    [JsonIgnore]
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

    [JsonIgnore]
    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
