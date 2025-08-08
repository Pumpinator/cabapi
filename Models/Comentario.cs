using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Comentario
{
    public int Id { get; set; }

    public required string Texto { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.Now;

    public int UsuarioId { get; set; }

    public int? Calificacion { get; set; } // 1-5 estrellas

    public bool Activo { get; set; } = true;

    [JsonIgnore]
    public virtual Usuario Usuario { get; set; } = null!;
};
