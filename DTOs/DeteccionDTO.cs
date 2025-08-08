namespace cabapi.DTOs;

public class DeteccionDTO
{
    public required Tipo Tipo { get; set; } // Organico, Valorizable, NoValorizable
    public int ClasificadorId { get; set; }
}