namespace cabapi.DTOs;

public class InferenciaDTO
{
    public required IFormFile Imagen { get; set; }
    public int ClasificadorId { get; set; }
}
