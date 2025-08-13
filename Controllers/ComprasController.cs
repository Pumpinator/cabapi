using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cabapi.Models;
using System.Text.Json;
using System.Linq;

namespace cabapi.Controllers
{
    [Route("api/compras")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly CABDB _context;

        public ComprasController(CABDB context)
        {
            _context = context;
        }

        // GET: api/compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCompras()
        {
            // Incluir detalles y proveedores para determinar si hay un proveedor único
            var comprasConDetalles = await _context.Compras
                .AsNoTracking()
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Proveedor)
                .ToListAsync();

            var compras = comprasConDetalles.Select(c =>
            {
                var proveedores = c.Detalles.Select(d => d.Proveedor).Where(p => p != null).DistinctBy(p => p!.Id).ToList();
                var proveedorUnico = proveedores.Count == 1 ? proveedores.First() : null;
                return new
                {
                    c.Id,
                    c.NumeroCompra,
                    c.FechaCompra,
                    proveedor = proveedorUnico == null ? null : new { id = proveedorUnico.Id, nombre = proveedorUnico.Nombre },
                    c.SubTotal,
                    c.Total,
                    estatus = c.Estatus
                };
            }).ToList();

            return Ok(compras);
        }

        // GET: api/compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCompra(int id)
        {
            var compraDb = await _context.Compras
                .AsNoTracking()
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.MateriaPrima)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Proveedor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compraDb == null)
            {
                return NotFound();
            }

            var proveedores = compraDb.Detalles.Select(d => d.Proveedor).Where(p => p != null).DistinctBy(p => p!.Id).ToList();
            var proveedorUnico = proveedores.Count == 1 ? proveedores.First() : null;

            var compra = new
            {
                compraDb.Id,
                compraDb.NumeroCompra,
                compraDb.FechaCompra,
                proveedor = proveedorUnico == null ? null : new { id = proveedorUnico.Id, nombre = proveedorUnico.Nombre },
                compraDb.Observaciones,
                compraDb.SubTotal,
                compraDb.Total,
                estatus = compraDb.Estatus,
                detalles = compraDb.Detalles.Select(d => new
                {
                    d.Id,
                    d.MateriaPrimaId,
                    materiaPrima = d.MateriaPrima == null ? null : new { id = d.MateriaPrima.Id, nombre = d.MateriaPrima.Nombre },
                    d.ProveedorId,
                    proveedor = d.Proveedor == null ? null : new { id = d.Proveedor.Id, nombre = d.Proveedor.Nombre },
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.SubTotal
                }).ToList()
            };

            return Ok(compra);
        }

        // PUT: api/compras/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompra(int id, [FromBody] JsonElement body)
        {
            var compraDb = await _context.Compras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (compraDb == null)
            {
                return NotFound();
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var fechaOriginal = compraDb.FechaCompra;
            var numeroOriginal = compraDb.NumeroCompra;

            if (body.TryGetProperty("observaciones", out var obsEl))
            {
                compraDb.Observaciones = obsEl.ValueKind == JsonValueKind.Null ? null : obsEl.GetString();
            }
            if (body.TryGetProperty("estatus", out var estEl) && estEl.ValueKind == JsonValueKind.String)
            {
                var estStr = estEl.GetString();
                if (!string.IsNullOrWhiteSpace(estStr) && Enum.TryParse<Estatus>(estStr, true, out var estatus))
                {
                    compraDb.Estatus = estatus;
                }
            }

            var oldDetalles = compraDb.Detalles.ToList();

            var mpIdsOld = oldDetalles.Select(d => d.MateriaPrimaId).Distinct().ToList();
            var materiasOld = await _context.MateriasPrimas.Where(m => mpIdsOld.Contains(m.Id)).ToDictionaryAsync(m => m.Id);
            foreach (var d in oldDetalles)
            {
                if (materiasOld.TryGetValue(d.MateriaPrimaId, out var mp))
                {
                    mp.Stock -= d.Cantidad;
                }
            }

            _context.CompraDetalles.RemoveRange(oldDetalles);

            int? defaultProveedorId = null;
            if (body.TryGetProperty("proveedorId", out var provEl) && provEl.ValueKind == JsonValueKind.Number)
            {
                defaultProveedorId = provEl.GetInt32();
            }

            var nuevosDetalles = new List<CompraDetalle>();
            if (body.TryGetProperty("detalles", out var detsEl) && detsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var dEl in detsEl.EnumerateArray())
                {
                    if (!dEl.TryGetProperty("materiaPrimaId", out var mpEl) || mpEl.ValueKind != JsonValueKind.Number) continue;
                    if (!dEl.TryGetProperty("cantidad", out var cantEl) || cantEl.ValueKind != JsonValueKind.Number) continue;
                    var mpId = mpEl.GetInt32();
                    var cantidad = cantEl.GetInt32();
                    decimal precio = 0m;
                    if (dEl.TryGetProperty("precioUnitario", out var puEl) && puEl.ValueKind == JsonValueKind.Number)
                    {
                        precio = puEl.GetDecimal();
                    }
                    int? proveedorId = null;
                    if (dEl.TryGetProperty("proveedorId", out var provDetEl) && provDetEl.ValueKind == JsonValueKind.Number)
                    {
                        proveedorId = provDetEl.GetInt32();
                    }
                    proveedorId ??= defaultProveedorId;
                    if (proveedorId is null || proveedorId.Value <= 0)
                    {
                        continue;
                    }
                    var sub = precio * cantidad;
                    nuevosDetalles.Add(new CompraDetalle
                    {
                        CompraId = compraDb.Id,
                        MateriaPrimaId = mpId,
                        ProveedorId = proveedorId.Value,
                        Cantidad = cantidad,
                        PrecioUnitario = precio,
                        SubTotal = sub
                    });
                }
            }

            if (nuevosDetalles.Count == 0)
            {
                return BadRequest(new { message = "Cada detalle debe incluir proveedorId, materiaPrimaId y cantidad (o proveer proveedorId a nivel compra)." });
            }

            compraDb.SubTotal = nuevosDetalles.Sum(d => d.SubTotal);
            compraDb.Total = compraDb.SubTotal; // impuestos/descuentos futuros

            await _context.CompraDetalles.AddRangeAsync(nuevosDetalles);

            var mpIdsNew = nuevosDetalles.Select(d => d.MateriaPrimaId).Distinct().ToList();
            var materiasNew = await _context.MateriasPrimas.Where(m => mpIdsNew.Contains(m.Id)).ToDictionaryAsync(m => m.Id);
            foreach (var d in nuevosDetalles)
            {
                if (!materiasNew.TryGetValue(d.MateriaPrimaId, out var mp))
                {
                    return BadRequest(new { message = $"Materia prima {d.MateriaPrimaId} no existe." });
                }
                mp.Stock += d.Cantidad; // aplicar nueva compra
            }

            compraDb.FechaCompra = fechaOriginal;
            compraDb.NumeroCompra = numeroOriginal;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return NoContent();
        }

        // POST: api/compras
        [HttpPost]
        public async Task<ActionResult<object>> PostCompra([FromBody] JsonElement body)
        {
            var compra = new Compra
            {
                NumeroCompra = $"CP-{DateTime.Now.Year}-0"
            };

            int? defaultProveedorId = null;
            if (body.TryGetProperty("proveedorId", out var provEl) && provEl.ValueKind == JsonValueKind.Number)
            {
                defaultProveedorId = provEl.GetInt32();
            }

            if (body.TryGetProperty("fechaCompra", out var fechaEl) && fechaEl.ValueKind == JsonValueKind.String)
            {
                if (DateTime.TryParse(fechaEl.GetString(), out var fecha))
                {
                    compra.FechaCompra = fecha;
                }
            }

            if (body.TryGetProperty("observaciones", out var obsEl))
            {
                compra.Observaciones = obsEl.ValueKind == JsonValueKind.Null ? null : obsEl.GetString();
            }

            compra.Estatus = Estatus.Pendiente;
            if (body.TryGetProperty("estatus", out var estEl) && estEl.ValueKind == JsonValueKind.String)
            {
                var estStr = estEl.GetString();
                if (!string.IsNullOrWhiteSpace(estStr) && Enum.TryParse<Estatus>(estStr, true, out var estatus))
                {
                    compra.Estatus = estatus;
                }
            }

            if (!body.TryGetProperty("detalles", out var detsEl) || detsEl.ValueKind != JsonValueKind.Array)
            {
                return BadRequest(new { message = "detalles es requerido" });
            }

            var detalles = new List<CompraDetalle>();
            foreach (var dEl in detsEl.EnumerateArray())
            {
                if (!dEl.TryGetProperty("materiaPrimaId", out var mpEl) || mpEl.ValueKind != JsonValueKind.Number) continue;
                if (!dEl.TryGetProperty("cantidad", out var cantEl) || cantEl.ValueKind != JsonValueKind.Number) continue;
                var mpId = mpEl.GetInt32();
                var cantidad = cantEl.GetInt32();
                decimal precio = 0m;
                if (dEl.TryGetProperty("precioUnitario", out var puEl) && puEl.ValueKind == JsonValueKind.Number)
                {
                    precio = puEl.GetDecimal();
                }
                int? proveedorId = null;
                if (dEl.TryGetProperty("proveedorId", out var provDetEl) && provDetEl.ValueKind == JsonValueKind.Number)
                {
                    proveedorId = provDetEl.GetInt32();
                }
                proveedorId ??= defaultProveedorId;
                if (proveedorId is null || proveedorId.Value <= 0)
                {
                    return BadRequest(new { message = "Cada detalle debe incluir proveedorId o proporcionarlo a nivel compra" });
                }
                var sub = precio * cantidad;
                detalles.Add(new CompraDetalle
                {
                    MateriaPrimaId = mpId,
                    ProveedorId = proveedorId.Value,
                    Cantidad = cantidad,
                    PrecioUnitario = precio,
                    SubTotal = sub
                });
            }

            if (detalles.Count == 0)
            {
                return BadRequest(new { message = "Se requiere al menos un detalle válido (materiaPrimaId, cantidad)." });
            }

            compra.SubTotal = detalles.Sum(d => d.SubTotal);
            compra.Total = compra.SubTotal; // impuestos/descuentos futuros

            await using var tx = await _context.Database.BeginTransactionAsync();

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            foreach (var d in detalles)
            {
                d.CompraId = compra.Id;
            }
            await _context.CompraDetalles.AddRangeAsync(detalles);

            compra.NumeroCompra = $"CP-{DateTime.Now.Year}-{compra.Id}";
            _context.Entry(compra).Property(c => c.NumeroCompra).IsModified = true;

            var mpIds = detalles.Select(d => d.MateriaPrimaId).Distinct().ToList();
            var materias = await _context.MateriasPrimas.Where(m => mpIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id);
            foreach (var d in detalles)
            {
                if (!materias.TryGetValue(d.MateriaPrimaId, out var mp))
                {
                    return BadRequest(new { message = $"Materia prima {d.MateriaPrimaId} no existe." });
                }
                mp.Stock += d.Cantidad;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            var provs = detalles.Select(d => d.ProveedorId).Distinct().ToList();
            Proveedor? proveedor = null;
            if (provs.Count == 1)
            {
                proveedor = await _context.Proveedores.AsNoTracking().FirstOrDefaultAsync(p => p.Id == provs[0]);
            }
            var response = new
            {
                compra.Id,
                compra.NumeroCompra,
                compra.FechaCompra,
                proveedor = proveedor == null ? null : new { id = proveedor.Id, nombre = proveedor.Nombre },
                compra.Observaciones,
                compra.SubTotal,
                compra.Total,
                estatus = compra.Estatus
            };

            return CreatedAtAction(nameof(GetCompra), new { id = compra.Id }, response);
        }

        // DELETE: api/compras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            // Revertir stock por cada detalle
            var mpIds = compra.Detalles.Select(d => d.MateriaPrimaId).Distinct().ToList();
            var materias = await _context.MateriasPrimas.Where(m => mpIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id);
            foreach (var d in compra.Detalles)
            {
                if (materias.TryGetValue(d.MateriaPrimaId, out var mp))
                {
                    mp.Stock -= d.Cantidad;
                }
            }

            _context.CompraDetalles.RemoveRange(compra.Detalles);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return NoContent();
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }
    }
}
