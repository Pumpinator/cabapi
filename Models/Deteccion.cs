using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Deteccion
{
    public int Id { get; set; }

    public required string Tipo { get; set; }

    public DateTime FechaHora { get; set; }

    public int ClasificadorId { get; set; }
    
    [JsonIgnore]
    public virtual Clasificador Clasificador { get; set; } = null!;
}
