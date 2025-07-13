using System.Text.Json.Serialization;

namespace cabapi.Models;

public class Zona
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    [JsonIgnore]
    public virtual ICollection<Clasificador> Clasificadores { get; set; } = new List<Clasificador>();
}
