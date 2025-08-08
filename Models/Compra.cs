using System.ComponentModel.DataAnnotations;
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
}
