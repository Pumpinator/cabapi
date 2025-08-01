using cabapi.DTOs;
using cabapi.Models;
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
    public async Task<ActionResult<Clasificador>> Create([FromBody] ClasificadorDTO request)
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

        await _db.Entry(clasificador)
            .Reference(c => c.Zona)
            .LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = clasificador.Id }, clasificador);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] ClasificadorDTO request)
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

        var tieneDetecciones = await _db.Detecciones.AnyAsync(d => d.ClasificadorId == id);
        if (tieneDetecciones)
        {
            return BadRequest(new { message = "No se puede eliminar el clasificador porque tiene detecciones asociadas" });
        }

        _db.Clasificadores.Remove(clasificador);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("zona/{zonaId}")]
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
}