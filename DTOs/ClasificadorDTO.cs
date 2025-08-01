namespace cabapi.DTOs;

public class ClasificadorDTO
{
    public required string Nombre { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public int ZonaId { get; set; }
}
