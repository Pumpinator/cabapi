using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cabapi;
using cabapi.Models;

namespace cabapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraDetallesController : ControllerBase
    {
        private readonly CABDB _context;

        public CompraDetallesController(CABDB context)
        {
            _context = context;
        }

        // GET: api/CompraDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDetalle>>> GetCompraDetalles()
        {
            return await _context.CompraDetalles.ToListAsync();
        }

        // GET: api/CompraDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDetalle>> GetCompraDetalle(int id)
        {
            var compraDetalle = await _context.CompraDetalles.FindAsync(id);

            if (compraDetalle == null)
            {
                return NotFound();
            }

            return compraDetalle;
        }

        // PUT: api/CompraDetalles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompraDetalle(int id, CompraDetalle compraDetalle)
        {
            if (id != compraDetalle.Id)
            {
                return BadRequest();
            }

            _context.Entry(compraDetalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraDetalleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CompraDetalles
        [HttpPost]
        public async Task<ActionResult<CompraDetalle>> PostCompraDetalle(CompraDetalle compraDetalle)
        {
            _context.CompraDetalles.Add(compraDetalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompraDetalle", new { id = compraDetalle.Id }, compraDetalle);
        }

        // DELETE: api/CompraDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompraDetalle(int id)
        {
            var compraDetalle = await _context.CompraDetalles.FindAsync(id);
            if (compraDetalle == null)
            {
                return NotFound();
            }

            _context.CompraDetalles.Remove(compraDetalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompraDetalleExists(int id)
        {
            return _context.CompraDetalles.Any(e => e.Id == id);
        }
    }
}
