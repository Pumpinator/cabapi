// DTOs/ProductoDto.cs
using System.ComponentModel.DataAnnotations;

namespace cabapi.DTOs
{
    // DTO para crear un nuevo producto
    public class CreateProductoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El costo es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser mayor o igual a 0")]
        public decimal Costo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; } = 0;

        [StringLength(500, ErrorMessage = "La URL de la imagen no puede exceder 500 caracteres")]
        public string? Imagen { get; set; }

        public bool Activo { get; set; } = true;
    }

    // DTO para actualizar un producto
    public class UpdateProductoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El costo es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser mayor o igual a 0")]
        public decimal Costo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [StringLength(500, ErrorMessage = "La URL de la imagen no puede exceder 500 caracteres")]
        public string? Imagen { get; set; }

        public bool Activo { get; set; }
    }

    // DTO para respuesta (lo que se devuelve al cliente)
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Costo { get; set; }
        public int Stock { get; set; }
        public string? Imagen { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Propiedades calculadas que pueden ser útiles
        public string EstadoStock => Stock == 0 ? "Sin stock" : Stock <= 10 ? "Stock bajo" : "En stock";
        public string Estado => Activo ? "Activo" : "Inactivo";
        public decimal? Margen => Precio > 0 ? ((Precio - Costo) / Precio) * 100 : null;
    }

    // DTO para operaciones masivas
    public class BulkProductoOperationDto
    {
        [Required]
        public List<int> ProductoIds { get; set; } = new();

        [Required]
        public string Operacion { get; set; } = string.Empty; // "activate", "deactivate", "delete"
    }

    // DTO para filtros de búsqueda
    public class ProductoFilterDto
    {
        public string? SearchTerm { get; set; }
        public string StatusFilter { get; set; } = "all"; // all, active, inactive
        public string StockFilter { get; set; } = "all"; // all, in-stock, low-stock, out-of-stock
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "fechaCreacion"; // nombre, precio, stock, fechaCreacion
        public string SortDirection { get; set; } = "desc"; // asc, desc
    }

    // DTO para respuesta paginada
    public class PagedProductoResponseDto
    {
        public List<ProductoDto> Productos { get; set; } = new();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}