using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Clasificador
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int ZonaId { get; set; }

    public virtual Zona Zona { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Deteccion> Detecciones { get; set; } = new List<Deteccion>();
}
