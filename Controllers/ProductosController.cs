using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cabapi.Models;
using cabapi.DTOs;
using System.ComponentModel.DataAnnotations;

namespace cabapi.Controllers
{
    [Route("api/productos")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly CABDB _context;

        public ProductosController(CABDB context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _context.Productos.ToListAsync();

            var productosDto = productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Costo = p.Costo,
                Stock = p.Stock,
                Imagen = p.Imagen,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            }).ToList();

            return Ok(productosDto);
        }

        // GET: api/productos/paginado
        [HttpGet("paginado")]
        public async Task<ActionResult<PagedProductoResponseDto>> GetProductosPaginado([FromQuery] ProductoFilterDto filtros)
        {
            var query = _context.Productos.AsQueryable();

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrWhiteSpace(filtros.SearchTerm))
            {
                var searchTerm = filtros.SearchTerm.ToLower().Trim();
                query = query.Where(p => p.Nombre.ToLower().Contains(searchTerm) ||
                                        p.Descripcion.ToLower().Contains(searchTerm));
            }

            // Aplicar filtro de estado
            if (filtros.StatusFilter != "all")
            {
                var isActive = filtros.StatusFilter == "active";
                query = query.Where(p => p.Activo == isActive);
            }

            // Aplicar filtro de stock
            if (filtros.StockFilter != "all")
            {
                query = filtros.StockFilter switch
                {
                    "in-stock" => query.Where(p => p.Stock > 10),
                    "low-stock" => query.Where(p => p.Stock > 0 && p.Stock <= 10),
                    "out-of-stock" => query.Where(p => p.Stock == 0),
                    _ => query
                };
            }

            // Aplicar ordenamiento
            query = filtros.SortBy.ToLower() switch
            {
                "nombre" => filtros.SortDirection == "asc" ? query.OrderBy(p => p.Nombre) : query.OrderByDescending(p => p.Nombre),
                "precio" => filtros.SortDirection == "asc" ? query.OrderBy(p => p.Precio) : query.OrderByDescending(p => p.Precio),
                "stock" => filtros.SortDirection == "asc" ? query.OrderBy(p => p.Stock) : query.OrderByDescending(p => p.Stock),
                "fechacreacion" => filtros.SortDirection == "asc" ? query.OrderBy(p => p.FechaCreacion) : query.OrderByDescending(p => p.FechaCreacion),
                _ => query.OrderByDescending(p => p.FechaCreacion)
            };

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / filtros.PageSize);

            var productos = await query
                .Skip((filtros.Page - 1) * filtros.PageSize)
                .Take(filtros.PageSize)
                .ToListAsync();

            var productosDto = productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Costo = p.Costo,
                Stock = p.Stock,
                Imagen = p.Imagen,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            }).ToList();

            var response = new PagedProductoResponseDto
            {
                Productos = productosDto,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = filtros.Page,
                PageSize = filtros.PageSize,
                HasNextPage = filtros.Page < totalPages,
                HasPreviousPage = filtros.Page > 1
            };

            return Ok(response);
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            var productoDto = new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Costo = producto.Costo,
                Stock = producto.Stock,
                Imagen = producto.Imagen,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion
            };

            return Ok(productoDto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto([FromBody] CreateProductoDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = new Producto
            {
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion,
                Precio = createDto.Precio,
                Costo = createDto.Costo,
                Stock = createDto.Stock,
                Imagen = createDto.Imagen,
                Activo = createDto.Activo,
                FechaCreacion = DateTime.Now
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var productoDto = new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Costo = producto.Costo,
                Stock = producto.Stock,
                Imagen = producto.Imagen,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion
            };

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, productoDto);
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> PutProducto(int id, [FromBody] UpdateProductoDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            // Actualizar solo las propiedades permitidas
            producto.Nombre = updateDto.Nombre;
            producto.Descripcion = updateDto.Descripcion;
            producto.Precio = updateDto.Precio;
            producto.Costo = updateDto.Costo;
            producto.Stock = updateDto.Stock;
            producto.Imagen = updateDto.Imagen;
            producto.Activo = updateDto.Activo;
            // No actualizamos FechaCreacion ni Id

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var productoDto = new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Costo = producto.Costo,
                Stock = producto.Stock,
                Imagen = producto.Imagen,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion
            };

            return Ok(productoDto);
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            // Verificar si el producto tiene relaciones que impidan su eliminación
            var tieneVentas = await _context.Ventas.AnyAsync(v => v.ProductoId == producto.Id);
            if (tieneVentas)
            {
                return BadRequest(new { message = "No se puede eliminar el producto porque tiene ventas asociadas" });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Producto '{producto.Nombre}' eliminado exitosamente" });
        }

        // POST: api/productos/bulk-operation
        [HttpPost("bulk-operation")]
        public async Task<IActionResult> BulkOperation([FromBody] BulkProductoOperationDto bulkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!bulkDto.ProductoIds.Any())
            {
                return BadRequest(new { message = "No se han proporcionado IDs de productos" });
            }

            var productos = await _context.Productos
                .Where(p => bulkDto.ProductoIds.Contains(p.Id))
                .ToListAsync();

            if (!productos.Any())
            {
                return NotFound(new { message = "No se encontraron productos con los IDs proporcionados" });
            }

            switch (bulkDto.Operacion.ToLower())
            {
                case "activate":
                    productos.ForEach(p => p.Activo = true);
                    break;
                case "deactivate":
                    productos.ForEach(p => p.Activo = false);
                    break;
                case "delete":
                    // Verificar si algún producto tiene ventas
                    var productosConVentas = await _context.Ventas
                        .Where(v => bulkDto.ProductoIds.Contains(v.ProductoId))
                        .Select(v => v.ProductoId)
                        .Distinct()
                        .ToListAsync();

                    if (productosConVentas.Any())
                    {
                        return BadRequest(new
                        {
                            message = "Algunos productos no se pueden eliminar porque tienen ventas asociadas",
                            productosConVentas
                        });
                    }

                    _context.Productos.RemoveRange(productos);
                    break;
                default:
                    return BadRequest(new { message = "Operación no válida. Use: activate, deactivate, delete" });
            }

            var affectedRows = await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Operación '{bulkDto.Operacion}' completada exitosamente",
                productosAfectados = affectedRows,
                productosIds = productos.Select(p => p.Id).ToList()
            });
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}