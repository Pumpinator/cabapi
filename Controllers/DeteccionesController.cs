using cabapi.DTOs;
using cabapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cabapi.Controllers;

[ApiController]
[Route("api/detecciones")]
public class DeteccionesController : ControllerBase
{
    private readonly CABDB _db;

    public DeteccionesController(CABDB db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Deteccion>>> GetAll()
    {
        return Ok(await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .OrderByDescending(d => d.FechaHora)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Deteccion>> GetById(int id)
    {
        var deteccion = await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deteccion == null)
        {
            return NotFound();
        }

        return Ok(deteccion);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Deteccion>> Create([FromBody] DeteccionDTO request)
    {
        var deteccion = new Deteccion
        {
            Tipo = request.Tipo,
            FechaHora = DateTime.Now,
            ClasificadorId = request.ClasificadorId
        };

        _db.Detecciones.Add(deteccion);
        await _db.SaveChangesAsync();

        await _db.Entry(deteccion)
            .Reference(d => d.Clasificador)
            .LoadAsync();
        await _db.Entry(deteccion.Clasificador)
            .Reference(c => c.Zona)
            .LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = deteccion.Id }, deteccion);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] DeteccionDTO request)
    {
        var deteccion = await _db.Detecciones.FindAsync(id);
        if (deteccion == null)
        {
            return NotFound();
        }

        deteccion.Tipo = request.Tipo;
        deteccion.ClasificadorId = request.ClasificadorId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var deteccion = await _db.Detecciones.FindAsync(id);
        if (deteccion == null)
        {
            return NotFound();
        }

        _db.Detecciones.Remove(deteccion);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("estadisticas")]
    public async Task<ActionResult> GetEstadisticas()
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioAno = new DateTime(hoy.Year, 1, 1);

        var detecciones = await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .ToListAsync();

        var estadisticas = new
        {
            resumen = new
            {
                totalDetecciones = detecciones.Count,
                deteccionesHoy = detecciones.Count(d => d.FechaHora.Date == hoy),
                deteccionesEsteMes = detecciones.Count(d => d.FechaHora >= inicioMes),
                deteccionesEsteAno = detecciones.Count(d => d.FechaHora >= inicioAno)
            },
            porTipo = detecciones
                .GroupBy(d => d.Tipo)
                .Select(g => new { tipo = g.Key, cantidad = g.Count() })
                .OrderByDescending(x => x.cantidad)
                .ToList(),
            porZona = detecciones
                .GroupBy(d => d.Clasificador.Zona.Nombre)
                .Select(g => new { zona = g.Key, cantidad = g.Count() })
                .OrderByDescending(x => x.cantidad)
                .ToList(),
            porHora = detecciones
                .GroupBy(d => d.FechaHora.Hour)
                .Select(g => new { hora = g.Key, cantidad = g.Count() })
                .OrderBy(x => x.hora)
                .ToList(),
            tendenciaMensual = detecciones
                .GroupBy(d => new { d.FechaHora.Year, d.FechaHora.Month })
                .Select(g => new
                {
                    mes = g.Key.Month,
                    ano = g.Key.Year,
                    total = g.Count(),
                    organicos = g.Count(d => d.Tipo == "Organico"),
                    valorizables = g.Count(d => d.Tipo == "Valorizable"),
                    noValorizables = g.Count(d => d.Tipo == "No Valorizable")
                })
                .OrderBy(t => t.ano)
                .ThenBy(t => t.mes)
                .ToList()
        };

        return Ok(estadisticas);
    }

    [HttpGet("estadisticas/zonas")]
    public async Task<ActionResult> GetEstadisticasZonas()
    {
        var estadisticasZonas = await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .GroupBy(d => new { d.Clasificador.Zona.Id, d.Clasificador.Zona.Nombre })
            .Select(g => new
            {
                zonaId = g.Key.Id,
                zonaNombre = g.Key.Nombre,
                totalDetecciones = g.Count(),
                organicos = g.Count(d => d.Tipo == "Organico"),
                valorizables = g.Count(d => d.Tipo == "Valorizable"),
                noValorizables = g.Count(d => d.Tipo == "No Valorizable"),
                porcentaje = Math.Round((double)g.Count() / _db.Detecciones.Count() * 100, 2)
            })
            .OrderByDescending(x => x.totalDetecciones)
            .ToListAsync();

        return Ok(estadisticasZonas);
    }

    [HttpGet("estadisticas/populares")]
    [Authorize]
    public async Task<ActionResult> GetTiposPopulares()
    {
        var tiposPopulares = await _db.Detecciones
            .GroupBy(d => d.Tipo)
            .Select(g => new
            {
                tipo = g.Key,
                cantidad = g.Count(),
                porcentaje = Math.Round((double)g.Count() / _db.Detecciones.Count() * 100, 2),
                ultimaDeteccion = g.Max(d => d.FechaHora)
            })
            .OrderByDescending(x => x.cantidad)
            .ToListAsync();

        return Ok(tiposPopulares);
    }

    [HttpGet("estadisticas/horarios")]
    public async Task<ActionResult> GetHorariosRecurrentes()
    {
        var horariosRecurrentes = await _db.Detecciones
            .GroupBy(d => d.FechaHora.Hour)
            .Select(g => new
            {
                hora = g.Key,
                cantidad = g.Count(),
                porcentaje = Math.Round((double)g.Count() / _db.Detecciones.Count() * 100, 2),
                tipoMasComun = g.GroupBy(d => d.Tipo)
                    .OrderByDescending(tg => tg.Count())
                    .Select(tg => tg.Key)
                    .FirstOrDefault()
            })
            .OrderByDescending(x => x.cantidad)
            .ToListAsync();

        return Ok(horariosRecurrentes);
    }

    [HttpGet("zona/{zonaId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Deteccion>>> GetByZona(int zonaId)
    {
        return Ok(await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .Where(d => d.Clasificador.ZonaId == zonaId)
            .OrderByDescending(d => d.FechaHora)
            .ToListAsync());
    }

    [HttpGet("clasificador/{clasificadorId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Deteccion>>> GetByClasificador(int clasificadorId)
    {
        var grupos = await _db.Detecciones
            .Where(d => d.ClasificadorId == clasificadorId)
            .GroupBy(d => d.Tipo)
            .Select(g => new { Tipo = g.Key, Count = g.Count() })
            .ToListAsync();

        var resultado = new
        {
            valorizable = grupos.FirstOrDefault(g => g.Tipo == "valorizable")?.Count ?? 0,
            no_valorizable = grupos.FirstOrDefault(g => g.Tipo == "no_valorizable")?.Count ?? 0,
            organico = grupos.FirstOrDefault(g => g.Tipo == "organico")?.Count ?? 0
        };

        return Ok(resultado);
    }

    [HttpGet("recientes")]
    [Authorize]
    public async Task<ActionResult> GetRecientes([FromQuery] int limit = 10)
    {
        var deteccionesRecientes = await _db.Detecciones
            .Include(d => d.Clasificador)
            .Include(d => d.Clasificador.Zona)
            .OrderByDescending(d => d.FechaHora)
            .Take(limit)
            .ToListAsync();

        return Ok(deteccionesRecientes);
    }
}