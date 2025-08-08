using System.ComponentModel.DataAnnotations;

namespace cabapi.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}
