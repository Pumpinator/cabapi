using cabapi.Models;

namespace cabapi.DTOs
{
    public class ComentarioDTO
    {
        public int Id { get; set; }

        public required string Texto { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public int UsuarioId { get; set; }
        public int ProductoID { get; set; }

        public int? Calificacion { get; set; }

        public bool Activo { get; set; } = true;

    }
}
