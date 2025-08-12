using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cabapi.Models;
using cabapi.DTOs;

namespace cabapi.Controllers
{
    [Route("api/comentarios")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly CABDB _context;

        public ComentariosController(CABDB context)
        {
            _context = context;
        }

        // GET: api/comentarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            return await _context.Comentarios.ToListAsync();
        }

        // GET: api/comentarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return comentario;
        }

        [HttpGet("producto/{productoId}")]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentariosByProducto(int productoId)
        {
            return await _context.Comentarios
                .Where(c => c.ProductoId == productoId && c.Activo)
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        // PUT: api/comentarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentario(int id, Comentario comentario)
        {
            if (id != comentario.Id)
            {
                return BadRequest();
            }

            _context.Entry(comentario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentarioExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Comentario>> PostComentario(ComentarioDTO dto)
        {
            var comentario = new Comentario
            {
                Texto = dto.Texto,
                FechaHora = DateTime.Now,
                UsuarioId = dto.UsuarioId,
                Calificacion = dto.Calificacion,
                Activo = dto.Activo
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComentario", new { id = comentario.Id }, comentario);
        }

        // DELETE: api/comentarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
