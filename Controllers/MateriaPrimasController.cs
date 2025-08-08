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
    public class MateriaPrimasController : ControllerBase
    {
        private readonly CABDB _context;

        public MateriaPrimasController(CABDB context)
        {
            _context = context;
        }

        // GET: api/MateriaPrimas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaPrima>>> GetMateriasPrimas()
        {
            return await _context.MateriasPrimas.ToListAsync();
        }

        // GET: api/MateriaPrimas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaPrima>> GetMateriaPrima(int id)
        {
            var materiaPrima = await _context.MateriasPrimas.FindAsync(id);

            if (materiaPrima == null)
            {
                return NotFound();
            }

            return materiaPrima;
        }

        // PUT: api/MateriaPrimas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMateriaPrima(int id, MateriaPrima materiaPrima)
        {
            if (id != materiaPrima.Id)
            {
                return BadRequest();
            }

            _context.Entry(materiaPrima).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MateriaPrimaExists(id))
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

        // POST: api/MateriaPrimas
        [HttpPost]
        public async Task<ActionResult<MateriaPrima>> PostMateriaPrima(MateriaPrima materiaPrima)
        {
            _context.MateriasPrimas.Add(materiaPrima);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMateriaPrima", new { id = materiaPrima.Id }, materiaPrima);
        }

        // DELETE: api/MateriaPrimas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMateriaPrima(int id)
        {
            var materiaPrima = await _context.MateriasPrimas.FindAsync(id);
            if (materiaPrima == null)
            {
                return NotFound();
            }

            _context.MateriasPrimas.Remove(materiaPrima);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MateriaPrimaExists(int id)
        {
            return _context.MateriasPrimas.Any(e => e.Id == id);
        }
    }
}
