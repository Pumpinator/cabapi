using cabapi.DTOs;
using cabapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cabapi.Controllers;

[ApiController]
[Route("api/zonas")]
public class ZonasController : ControllerBase
{
    private readonly CABDB _db;

    public ZonasController(CABDB db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Zona>>> GetAll()
    {
        return Ok(await _db.Zonas
            .OrderBy(z => z.Nombre)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Zona>> GetById(int id)
    {
        var zona = await _db.Zonas
            .Include(z => z.Clasificadores)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zona == null)
        {
            return NotFound();
        }

        return Ok(zona);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Zona>> Create([FromBody] ZonaDTO request)
    {
        var zona = new Zona
        {
            Nombre = request.Nombre
        };

        _db.Zonas.Add(zona);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = zona.Id }, zona);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] ZonaDTO request)
    {
        var zona = await _db.Zonas.FindAsync(id);
        if (zona == null)
        {
            return NotFound();
        }

        zona.Nombre = request.Nombre;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var zona = await _db.Zonas.FindAsync(id);
        if (zona == null)
        {
            return NotFound();
        }

        // Verificar si tiene clasificadores asociados
        var tieneClasificadores = await _db.Clasificadores.AnyAsync(c => c.ZonaId == id);
        if (tieneClasificadores)
        {
            return BadRequest(new { message = "No se puede eliminar la zona porque tiene clasificadores asociados" });
        }

        _db.Zonas.Remove(zona);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("estadisticas")]
    [Authorize]
    public async Task<ActionResult> GetEstadisticas()
    {
        var estadisticasZonas = await _db.Zonas
            .Include(z => z.Clasificadores)
            .ThenInclude(c => c.Detecciones)
            .Select(z => new
            {
                id = z.Id,
                nombre = z.Nombre,
                totalClasificadores = z.Clasificadores.Count(),
                totalDetecciones = z.Clasificadores.SelectMany(c => c.Detecciones).Count(),
                deteccionesHoy = z.Clasificadores
                    .SelectMany(c => c.Detecciones)
                    .Count(d => d.FechaHora.Date == DateTime.Today),
                ultimaActividad = z.Clasificadores
                    .SelectMany(c => c.Detecciones)
                    .Any() ? z.Clasificadores
                    .SelectMany(c => c.Detecciones)
                    .Max(d => d.FechaHora) : (DateTime?)null,
                tipoMasComun = z.Clasificadores
                    .SelectMany(c => c.Detecciones)
                    .GroupBy(d => d.Tipo)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            })
            .OrderByDescending(x => x.totalDetecciones)
            .ToListAsync();

        return Ok(estadisticasZonas);
    }

    [HttpGet("{id}/detecciones")]
    [Authorize]
    public async Task<ActionResult> GetDeteccionesPorZona(int id)
    {
        var zona = await _db.Zonas.FindAsync(id);
        if (zona == null)
        {
            return NotFound();
        }

        var detecciones = await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .Where(d => d.Clasificador.ZonaId == id)
            .OrderByDescending(d => d.FechaHora)
            .ToListAsync();

        return Ok(new
        {
            zona = zona.Nombre,
            totalDetecciones = detecciones.Count,
            detecciones = detecciones
        });
    }

    [HttpGet("{id}/clasificadores")]
    [Authorize]
    public async Task<ActionResult> GetClasificadoresPorZona(int id)
    {
        var zona = await _db.Zonas
            .Include(z => z.Clasificadores)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zona == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            zona = zona.Nombre,
            clasificadores = zona.Clasificadores.Select(c => new
            {
                id = c.Id,
                nombre = c.Nombre,
                latitud = c.Latitud,
                longitud = c.Longitud,
                fechaCreacion = c.FechaCreacion
            })
        });
    }
}
