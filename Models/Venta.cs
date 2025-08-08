namespace cabapi.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } 
        public int[] ProductoId { get; set; }
    }
}
