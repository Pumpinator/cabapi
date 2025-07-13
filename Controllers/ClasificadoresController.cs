using cabapi.Models;
using Empresa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cabapi.Controllers;

[ApiController]
[Route("api/clasificadores")]
public class ClasificadoresController : ControllerBase
{
    private readonly CABDB _db;

    public ClasificadoresController(CABDB db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Clasificador>>> GetAll()
    {
        return Ok(await _db.Clasificadores
            .Include(c => c.Zona)
            .OrderBy(c => c.Zona.Nombre)
            .ThenBy(c => c.Nombre)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Clasificador>> GetById(int id)
    {
        var clasificador = await _db.Clasificadores
            .Include(c => c.Zona)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clasificador == null)
        {
            return NotFound();
        }

        return Ok(clasificador);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Clasificador>> Create([FromBody] CreateClasificadorRequest request)
    {
        var clasificador = new Clasificador
        {
            Nombre = request.Nombre,
            Latitud = request.Latitud,
            Longitud = request.Longitud,
            FechaCreacion = DateTime.Now,
            ZonaId = request.ZonaId
        };

        _db.Clasificadores.Add(clasificador);
        await _db.SaveChangesAsync();

        // Incluir zona para la respuesta
        await _db.Entry(clasificador)
            .Reference(c => c.Zona)
            .LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = clasificador.Id }, clasificador);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateClasificadorRequest request)
    {
        var clasificador = await _db.Clasificadores.FindAsync(id);
        if (clasificador == null)
        {
            return NotFound();
        }

        clasificador.Nombre = request.Nombre;
        clasificador.Latitud = request.Latitud;
        clasificador.Longitud = request.Longitud;
        clasificador.ZonaId = request.ZonaId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var clasificador = await _db.Clasificadores.FindAsync(id);
        if (clasificador == null)
        {
            return NotFound();
        }

        // Verificar si tiene detecciones asociadas
        var tieneDetecciones = await _db.Detecciones.AnyAsync(d => d.ClasificadorId == id);
        if (tieneDetecciones)
        {
            return BadRequest(new { message = "No se puede eliminar el clasificador porque tiene detecciones asociadas" });
        }

        _db.Clasificadores.Remove(clasificador);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // RF09 - Clasificador más cercano
    [HttpPost("mas-cercano")]
    [Authorize]
    public async Task<ActionResult> GetMasCercano([FromBody] UbicacionRequest ubicacion)
    {
        var clasificadores = await _db.Clasificadores
            .Include(c => c.Zona)
            .ToListAsync();

        if (!clasificadores.Any())
        {
            return NotFound(new { message = "No hay clasificadores disponibles" });
        }

        var clasificadorCercano = clasificadores
            .Select(c => new
            {
                id = c.Id,
                nombre = c.Nombre,
                latitud = c.Latitud,
                longitud = c.Longitud,
                zona = c.Zona.Nombre,
                distancia = CalcularDistancia(
                    (double)ubicacion.Latitud, (double)ubicacion.Longitud,
                    (double)c.Latitud, (double)c.Longitud)
            })
            .OrderBy(c => c.distancia)
            .First();

        return Ok(new
        {
            clasificador = clasificadorCercano,
            instrucciones = $"El clasificador más cercano está a {clasificadorCercano.distancia:F0} metros de tu ubicación"
        });
    }

    [HttpGet("por-zona/{zonaId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Clasificador>>> GetByZona(int zonaId)
    {
        return Ok(await _db.Clasificadores
            .Include(c => c.Zona)
            .Where(c => c.ZonaId == zonaId)
            .OrderBy(c => c.Nombre)
            .ToListAsync());
    }

    [HttpGet("estadisticas")]
    [Authorize]
    public async Task<ActionResult> GetEstadisticas()
    {
        var estadisticas = await _db.Clasificadores
            .Include(c => c.Zona)
            .Include(c => c.Detecciones)
            .Select(c => new
            {
                id = c.Id,
                nombre = c.Nombre,
                zona = c.Zona.Nombre,
                totalDetecciones = c.Detecciones.Count(),
                deteccionesHoy = c.Detecciones.Count(d => d.FechaHora.Date == DateTime.Today),
                ultimaDeteccion = c.Detecciones.Any() ? c.Detecciones.Max(d => d.FechaHora) : (DateTime?)null
            })
            .OrderByDescending(x => x.totalDetecciones)
            .ToListAsync();

        return Ok(estadisticas);
    }

    private static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Radio de la Tierra en metros
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

// DTOs
public class CreateClasificadorRequest
{
    public required string Nombre { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public int ZonaId { get; set; }
}

public class UpdateClasificadorRequest
{
    public required string Nombre { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public int ZonaId { get; set; }
}

public class UbicacionRequest
{
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
}
