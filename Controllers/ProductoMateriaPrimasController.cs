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
    public class ProductoMateriaPrimasController : ControllerBase
    {
        private readonly CABDB _context;

        public ProductoMateriaPrimasController(CABDB context)
        {
            _context = context;
        }

        // GET: api/ProductoMateriaPrimas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoMateriaPrima>>> GetProductoMateriasPrimas()
        {
            return await _context.ProductoMateriasPrimas.ToListAsync();
        }

        // GET: api/ProductoMateriaPrimas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoMateriaPrima>> GetProductoMateriaPrima(int id)
        {
            var productoMateriaPrima = await _context.ProductoMateriasPrimas.FindAsync(id);

            if (productoMateriaPrima == null)
            {
                return NotFound();
            }

            return productoMateriaPrima;
        }

        // PUT: api/ProductoMateriaPrimas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductoMateriaPrima(int id, ProductoMateriaPrima productoMateriaPrima)
        {
            if (id != productoMateriaPrima.Id)
            {
                return BadRequest();
            }

            _context.Entry(productoMateriaPrima).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoMateriaPrimaExists(id))
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

        // POST: api/ProductoMateriaPrimas
        [HttpPost]
        public async Task<ActionResult<ProductoMateriaPrima>> PostProductoMateriaPrima(ProductoMateriaPrima productoMateriaPrima)
        {
            _context.ProductoMateriasPrimas.Add(productoMateriaPrima);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductoMateriaPrima", new { id = productoMateriaPrima.Id }, productoMateriaPrima);
        }

        // DELETE: api/ProductoMateriaPrimas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductoMateriaPrima(int id)
        {
            var productoMateriaPrima = await _context.ProductoMateriasPrimas.FindAsync(id);
            if (productoMateriaPrima == null)
            {
                return NotFound();
            }

            _context.ProductoMateriasPrimas.Remove(productoMateriaPrima);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoMateriaPrimaExists(int id)
        {
            return _context.ProductoMateriasPrimas.Any(e => e.Id == id);
        }
    }
}
