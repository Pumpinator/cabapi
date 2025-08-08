using System.Security.Cryptography;
using System.Text;
using cabapi.Models;
using Microsoft.EntityFrameworkCore;

namespace cabapi;

public class CABDB : DbContext
{
    public DbSet<Clasificador> Clasificadores { get; set; }
    public DbSet<Deteccion> Detecciones { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Zona> Zonas { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<MateriaPrima> MateriasPrimas { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<ProductoMateriaPrima> ProductoMateriasPrimas { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<CompraDetalle> CompraDetalles { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<Cotizacion> Cotizaciones { get; set; }

    public CABDB(DbContextOptions<CABDB> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Usuario> usuarios = new List<Usuario>
            {
                new Usuario {
                    Id = 1,
                    Nombre = "Super Administrador",
                    Correo = "superadmin@utleon.edu.mx",
                    Rol = "superadmin",
                    Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("password123"))),
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    FechaUltimoAcceso = new DateTime(2025, 7, 6, 12, 0, 0),
                    EnLinea = false,
                    Activo = true,
                },
                new Usuario {
                    Id = 2,
                    Nombre = "Administrador",
                    Correo = "admin@utleon.edu.mx",
                    Rol = "admin",
                    Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("admin123"))),
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    FechaUltimoAcceso = new DateTime(2025, 7, 6, 12, 0, 0),
                    EnLinea = false,
                    Activo = true
                },
                new Usuario {
                    Id = 3,
                    Nombre = "Cliente Demo",
                    Correo = "cliente@utleon.edu.mx",
                    Rol = "cliente",
                    Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("cliente123"))),
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    FechaUltimoAcceso = new DateTime(2025, 7, 6, 12, 0, 0),
                    EnLinea = false,
                    Activo = true
                }
            };

        List<Zona> zonas = new List<Zona>
            {
                new Zona { Id = 1, Nombre = "Edificio A", Activo = true },
                new Zona { Id = 2, Nombre = "Edificio B" , Activo = true },
                new Zona { Id = 3, Nombre = "Edificio C" , Activo = true },
                new Zona { Id = 4, Nombre = "Edificio C Pesado" , Activo = true },
                new Zona { Id = 5, Nombre = "Edificio D", Activo = true  },
                new Zona { Id = 6, Nombre = "Cafetería", Activo = true  },
                new Zona { Id = 7, Nombre = "Edificio CVD" },
                new Zona { Id = 8, Nombre = "Edificio F", Activo = true  },
                new Zona { Id = 9, Nombre = "Edificio de Rectoría", Activo = true  },
                new Zona { Id = 10, Nombre = "Almacén", Activo = true  }
            };

        List<Clasificador> clasificadores = new List<Clasificador>
            {
                new Clasificador {
                    Id = 1,
                    Nombre = "Entrada Principal",
                    Latitud = 21.0635822m,
                    Longitud = -101.5803752m,
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    ZonaId = 5,
                    Activo = true
                },
                new Clasificador {
                    Id = 2,
                    Nombre = "Pasillo",
                    Latitud = 21.0636721m,
                    Longitud = -101.580911m,
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    ZonaId = 5,
                    Activo = true
                },
                new Clasificador {
                    Id = 3,
                    Nombre = "Pasillo Superior",
                    Latitud = 21.0636795m,
                    Longitud = -101.5804751m,
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    ZonaId = 5,
                    Activo = true
                }
            };

        List<Deteccion> detecciones = new List<Deteccion>
            {
                new Deteccion { Id = 1, Tipo = "organico", FechaHora = new DateTime(2025, 7, 6, 12, 0, 0), ClasificadorId = 1 },
                new Deteccion { Id = 2, Tipo = "valorizable", FechaHora = new DateTime(2025, 7, 6, 12, 15, 0), ClasificadorId = 2 },
                new Deteccion { Id = 3, Tipo = "no_valorizable", FechaHora = new DateTime(2025, 7, 6, 12, 30, 0), ClasificadorId = 3 }
            };

        // Datos de demostración para Comentarios
        List<Comentario> comentarios = new List<Comentario>
            {
                new Comentario { 
                    Id = 1, 
                    Texto = "Excelente sistema de clasificación automática. Muy útil para nuestro centro de trabajo.", 
                    FechaHora = new DateTime(2025, 7, 10, 10, 30, 0), 
                    UsuarioId = 3, 
                    Calificacion = 5,
                    Activo = true 
                },
                new Comentario { 
                    Id = 2, 
                    Texto = "La precisión del clasificador es impresionante. Recomiendo totalmente este producto.", 
                    FechaHora = new DateTime(2025, 7, 15, 14, 20, 0), 
                    UsuarioId = 3, 
                    Calificacion = 5,
                    Activo = true 
                },
                new Comentario { 
                    Id = 3, 
                    Texto = "Fácil instalación y excelentes resultados. El soporte técnico es muy bueno.", 
                    FechaHora = new DateTime(2025, 7, 20, 16, 45, 0), 
                    UsuarioId = 3, 
                    Calificacion = 4,
                    Activo = true 
                }
            };

        // Datos de demostración para Proveedores
        List<Proveedor> proveedores = new List<Proveedor>
            {
                new Proveedor { 
                    Id = 1, 
                    Nombre = "TechComponents SA de CV", 
                    Contacto = new[] { "ventas@techcomponents.com", "477-123-4567", "Av. Tecnológico 123, León, Guanajuato" }, 
                    Producto = "Componentes electrónicos",
                    Activo = true,
                },
                new Proveedor { 
                    Id = 2, 
                    Nombre = "PlásticosMX", 
                    Contacto = new[] { "contacto@plasticosmx.com", "477-234-5678", "Blvd. Industrial 456, León, Guanajuato" }, 
                    Producto = "Carcasas y estructuras plásticas",
                    Activo = true,
                },
                new Proveedor { 
                    Id = 3, 
                    Nombre = "Sensores y Más", 
                    Contacto = new[] { "info@sensoresymas.com", "477-345-6789", "Zona Industrial Norte 789, León, Guanajuato" }, 
                    Producto = "Sensores y cámaras",
                    Activo = true,
                }
            };

        // Datos de demostración para Materias Primas
        List<MateriaPrima> materiasPrimas = new List<MateriaPrima>
            {
                new MateriaPrima { 
                    Id = 1, 
                    Nombre = "ESP32-CAM", 
                    Descripcion = "Módulo miniatura microcontrolador con cámara integrada para IoT",
                    PrecioUnitario = 350.00m,
                    UnidadMedida = "Pieza",
                    Stock = 5.00m,
                    StockMinimo = 10.00m,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 1, 9, 0, 0)
                },
                new MateriaPrima { 
                    Id = 2, 
                    Nombre = "ESP32-S3", 
                    Descripcion = "Módulo microcontrolador con capacidades mejoradas para IoT",
                    PrecioUnitario = 650.00m,
                    UnidadMedida = "Pieza",
                    Stock = 10.00m,
                    StockMinimo = 20.00m,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 2, 10, 0, 0)
                },
                new MateriaPrima { 
                    Id = 3, 
                    Nombre = "SG90", 
                    Descripcion = "Servo motor SG90 de 9g",
                    PrecioUnitario = 120.00m,
                    UnidadMedida = "Pieza",
                    Stock = 75.00m,
                    StockMinimo = 15.00m,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 3, 11, 0, 0)
                },
                new MateriaPrima { 
                    Id = 4, 
                    Nombre = "HC SR04", 
                    Descripcion = "Sensor de distancia ultrasónico HC-SR04",
                    PrecioUnitario = 45.00m,
                    UnidadMedida = "Pieza",
                    Stock = 30.00m,
                    StockMinimo = 5.00m,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 4, 12, 0, 0)
                },
            };

        // Datos de demostración para Productos
        List<Producto> productos = new List<Producto>
            {
                new Producto { 
                    Id = 1, 
                    Nombre = "CAB Clasificador Básico", 
                    Descripcion = "Sistema básico de clasificación automática de basura con ESP32-CAM. Incluye inteligencia artificial para identificar residuos orgánicos, valorizables y no valorizables.",
                    Precio = 2500.00m,
                    Costo = 1200.00m,
                    Stock = 25,
                    Imagen = "/images/productos/cab-basico.jpg",
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 15, 9, 0, 0)
                },
                new Producto { 
                    Id = 2, 
                    Nombre = "CAB Clasificador Pro", 
                    Descripcion = "Sistema avanzado con sensores adicionales, conectividad WiFi mejorada y dashboard en tiempo real. Perfecto para oficinas y centros educativos.",
                    Precio = 4200.00m,
                    Costo = 2100.00m,
                    Stock = 15,
                    Imagen = "/images/productos/cab-pro.jpg",
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 16, 10, 0, 0)
                },
                new Producto { 
                    Id = 3, 
                    Nombre = "CAB Clasificador Enterprise", 
                    Descripcion = "Solución empresarial completa con múltiples sensores, análisis avanzado, reportes detallados y soporte técnico premium.",
                    Precio = 7800.00m,
                    Costo = 3900.00m,
                    Stock = 8,
                    Imagen = "/images/productos/cab-enterprise.jpg",
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 17, 11, 0, 0)
                }
            };

        // Datos de demostración para ProductoMateriaPrima (Explosión de materiales)
        List<ProductoMateriaPrima> productoMateriasPrimas = new List<ProductoMateriaPrima>
            {
                // CAB Clasificador Básico
                new ProductoMateriaPrima { Id = 1, ProductoId = 1, MateriaPrimaId = 1, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // ESP32-CAM
                new ProductoMateriaPrima { Id = 2, ProductoId = 1, MateriaPrimaId = 2, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Carcasa
                new ProductoMateriaPrima { Id = 3, ProductoId = 1, MateriaPrimaId = 3, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Fuente
                new ProductoMateriaPrima { Id = 4, ProductoId = 1, MateriaPrimaId = 5, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Cable USB
                
                // CAB Clasificador Pro
                new ProductoMateriaPrima { Id = 5, ProductoId = 2, MateriaPrimaId = 1, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // ESP32-CAM
                new ProductoMateriaPrima { Id = 6, ProductoId = 2, MateriaPrimaId = 2, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Carcasa
                new ProductoMateriaPrima { Id = 7, ProductoId = 2, MateriaPrimaId = 3, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Fuente
                new ProductoMateriaPrima { Id = 8, ProductoId = 2, MateriaPrimaId = 4, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Sensor Ultrasónico
                new ProductoMateriaPrima { Id = 9, ProductoId = 2, MateriaPrimaId = 5, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Cable USB
                
                // CAB Clasificador Enterprise
                new ProductoMateriaPrima { Id = 10, ProductoId = 3, MateriaPrimaId = 1, CantidadRequerida = 2.00m, UnidadMedida = "Pieza" }, // ESP32-CAM x2
                new ProductoMateriaPrima { Id = 11, ProductoId = 3, MateriaPrimaId = 2, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Carcasa
                new ProductoMateriaPrima { Id = 12, ProductoId = 3, MateriaPrimaId = 3, CantidadRequerida = 1.00m, UnidadMedida = "Pieza" }, // Fuente
                new ProductoMateriaPrima { Id = 13, ProductoId = 3, MateriaPrimaId = 4, CantidadRequerida = 2.00m, UnidadMedida = "Pieza" }, // Sensor Ultrasónico x2
                new ProductoMateriaPrima { Id = 14, ProductoId = 3, MateriaPrimaId = 5, CantidadRequerida = 2.00m, UnidadMedida = "Pieza" }  // Cable USB x2
            };

        // Datos de demostración para Compras
        List<Compra> compras = new List<Compra>
            {
                new Compra { 
                    Id = 1, 
                    NumeroCompra = "C-2025-001", 
                    FechaCompra = new DateTime(2025, 7, 1, 10, 0, 0),
                    ProveedorId = 1,
                    SubTotal = 17500.00m,
                    Total = 20300.00m,
                    Estado = "Completada",
                    Observaciones = "Compra de componentes ESP32-CAM para producción mensual"
                },
                new Compra { 
                    Id = 2, 
                    NumeroCompra = "C-2025-002", 
                    FechaCompra = new DateTime(2025, 7, 5, 14, 30, 0),
                    ProveedorId = 2,
                    SubTotal = 8500.00m,
                    Total = 9860.00m,
                    Estado = "En Proceso",
                    Observaciones = "Pedido de carcasas plásticas y accesorios"
                }
            };

        // Datos de demostración para CompraDetalle
        List<CompraDetalle> compraDetalles = new List<CompraDetalle>
            {
                new CompraDetalle { Id = 1, CompraId = 1, MateriaPrimaId = 1, Cantidad = 50.00m, PrecioUnitario = 350.00m, SubTotal = 17500.00m },
                new CompraDetalle { Id = 2, CompraId = 2, MateriaPrimaId = 2, Cantidad = 100.00m, PrecioUnitario = 85.00m, SubTotal = 8500.00m }
            };

        // Datos de demostración para Ventas
        List<Venta> ventas = new List<Venta>
            {
                new Venta { 
                    Id = 1, 
                    NumeroVenta = "V-2025-001", 
                    FechaVenta = new DateTime(2025, 7, 10, 15, 30, 0),
                    UsuarioId = 3,
                    ProductoId = 1,
                    Cantidad = 2,
                    PrecioUnitario = 2500.00m,
                    SubTotal = 5000.00m,
                    Total = 5800.00m,
                    Estado = "Entregada",
                    DireccionEnvio = "Universidad Tecnológica de León, Blvd. Universidad Tecnológica #225, León, Gto.",
                    Observaciones = "Instalación en edificios A y B de la universidad"
                },
                new Venta { 
                    Id = 2, 
                    NumeroVenta = "V-2025-002", 
                    FechaVenta = new DateTime(2025, 7, 15, 11, 45, 0),
                    UsuarioId = 3,
                    ProductoId = 2,
                    Cantidad = 1,
                    PrecioUnitario = 4200.00m,
                    SubTotal = 4200.00m,
                    Total = 4872.00m,
                    Estado = "En Proceso",
                    DireccionEnvio = "Universidad Tecnológica de León, Blvd. Universidad Tecnológica #225, León, Gto.",
                    Observaciones = "Sistema para cafetería principal"
                }
            };

        // Datos de demostración para Cotizaciones
        List<Cotizacion> cotizaciones = new List<Cotizacion>
            {
                new Cotizacion { 
                    Id = 1, 
                    NumeroCotizacion = "COT-2025-001", 
                    FechaCotizacion = new DateTime(2025, 7, 20, 9, 15, 0),
                    NombreCliente = "María González",
                    CorreoCliente = "maria.gonzalez@empresa.com",
                    TelefonoCliente = "477-987-6543",
                    EmpresaCliente = "Corporativo del Bajío SA",
                    ProductoId = 3,
                    Cantidad = 5,
                    PrecioUnitario = 7800.00m,
                    SubTotal = 39000.00m,
                    Total = 45240.00m,
                    Estado = "Pendiente",
                    Observaciones = "Cotización para implementación en 5 sucursales",
                    RequirimientosEspeciales = "Necesitan capacitación del personal y soporte técnico por 6 meses"
                },
                new Cotizacion { 
                    Id = 2, 
                    NumeroCotizacion = "COT-2025-002", 
                    FechaCotizacion = new DateTime(2025, 7, 22, 16, 20, 0),
                    NombreCliente = "Roberto Hernández",
                    CorreoCliente = "roberto.h@hotel.com",
                    TelefonoCliente = "477-456-7890",
                    EmpresaCliente = "Hotel Ejecutivo León",
                    ProductoId = 2,
                    Cantidad = 3,
                    PrecioUnitario = 4200.00m,
                    SubTotal = 12600.00m,
                    Total = 14616.00m,
                    Estado = "Aprobada",
                    Observaciones = "Instalación en lobby, restaurante y áreas comunes",
                    RequirimientosEspeciales = "Integración con sistema de gestión hotelera existente"
                }
            };

        modelBuilder.Entity<Clasificador>(clasificador =>
        {
            clasificador.HasKey(c => c.Id);
            clasificador.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            clasificador.Property(c => c.Latitud).HasColumnType("decimal(9,6)");
            clasificador.Property(c => c.Longitud).HasColumnType("decimal(9,6)");
            clasificador.Property(c => c.FechaCreacion).HasDefaultValueSql("GETDATE()");
            clasificador.Property(u => u.Activo).HasDefaultValue(true);
            clasificador.HasOne(c => c.Zona)
                .WithMany(z => z.Clasificadores)
                .HasForeignKey(c => c.ZonaId);
            clasificador.HasData(clasificadores);
        });

        modelBuilder.Entity<Deteccion>(deteccion =>
        {
            deteccion.HasKey(d => d.Id);
            deteccion.Property(d => d.Tipo).IsRequired().HasMaxLength(50);
            deteccion.Property(d => d.FechaHora).HasDefaultValueSql("GETDATE()");
            deteccion.HasOne(d => d.Clasificador)
                .WithMany(c => c.Detecciones)
                .HasForeignKey(d => d.ClasificadorId);
            deteccion.HasData(detecciones);
        });


        modelBuilder.Entity<Usuario>(usuario =>
        {
            usuario.HasKey(u => u.Id);
            usuario.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
            usuario.Property(u => u.Correo).IsRequired().HasMaxLength(100);
            usuario.Property(u => u.Password).IsRequired().HasMaxLength(100);
            usuario.Property(u => u.FechaCreacion).HasDefaultValueSql("GETDATE()");
            usuario.Property(u => u.FechaUltimoAcceso).HasDefaultValueSql("GETDATE()");
            usuario.Property(u => u.EnLinea).HasDefaultValue(false);
            usuario.Property(u => u.Activo).HasDefaultValue(true);
            usuario.HasIndex(u => u.Correo).IsUnique();
            usuario.HasData(usuarios);
        });

        modelBuilder.Entity<Zona>(zona =>
        {
            zona.HasKey(z => z.Id);
            zona.Property(z => z.Nombre).IsRequired().HasMaxLength(100);
            zona.Property(u => u.Activo).HasDefaultValue(true);
            zona.HasData(zonas);
        });

        // Configuración de Comentario
        modelBuilder.Entity<Comentario>(comentario =>
        {
            comentario.HasKey(c => c.Id);
            comentario.Property(c => c.Texto).IsRequired().HasMaxLength(1000);
            comentario.Property(c => c.FechaHora).HasDefaultValueSql("GETDATE()");
            comentario.Property(c => c.Activo).HasDefaultValue(true);
            comentario.HasOne(c => c.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.UsuarioId);
            comentario.HasData(comentarios);
        });

        // Configuración de Proveedor
        modelBuilder.Entity<Proveedor>(proveedor =>
        {
            proveedor.HasKey(p => p.Id);
            proveedor.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            proveedor.Property(p => p.Contacto).HasColumnType("nvarchar(500)"); // Almacena múltiples contactos como JSON
            proveedor.Property(p => p.Producto).HasMaxLength(100);
            proveedor.Property(p => p.Activo).HasDefaultValue(true);
            proveedor.HasData(proveedores);
        });

        // Configuración de MateriaPrima
        modelBuilder.Entity<MateriaPrima>(materia =>
        {
            materia.HasKey(m => m.Id);
            materia.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
            materia.Property(m => m.Descripcion).IsRequired().HasMaxLength(500);
            materia.Property(m => m.PrecioUnitario).HasColumnType("decimal(10,2)");
            materia.Property(m => m.UnidadMedida).IsRequired().HasMaxLength(20);
            materia.Property(m => m.Stock).HasColumnType("decimal(10,2)");
            materia.Property(m => m.StockMinimo).HasColumnType("decimal(10,2)");
            materia.Property(m => m.Activo).HasDefaultValue(true);
            materia.Property(m => m.FechaCreacion).HasDefaultValueSql("GETDATE()");
            materia.HasData(materiasPrimas);
        });

        // Configuración de Producto
        modelBuilder.Entity<Producto>(producto =>
        {
            producto.HasKey(p => p.Id);
            producto.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            producto.Property(p => p.Descripcion).IsRequired().HasMaxLength(1000);
            producto.Property(p => p.Precio).HasColumnType("decimal(10,2)");
            producto.Property(p => p.Costo).HasColumnType("decimal(10,2)");
            producto.Property(p => p.Imagen).HasMaxLength(500);
            producto.Property(p => p.Activo).HasDefaultValue(true);
            producto.Property(p => p.FechaCreacion).HasDefaultValueSql("GETDATE()");
            producto.HasData(productos);
        });

        // Configuración de ProductoMateriaPrima
        modelBuilder.Entity<ProductoMateriaPrima>(pmp =>
        {
            pmp.HasKey(p => p.Id);
            pmp.Property(p => p.CantidadRequerida).HasColumnType("decimal(10,2)");
            pmp.Property(p => p.UnidadMedida).HasMaxLength(20);
            pmp.HasOne(p => p.Producto)
                .WithMany(pr => pr.ProductoMateriasPrimas)
                .HasForeignKey(p => p.ProductoId);
            pmp.HasOne(p => p.MateriaPrima)
                .WithMany(m => m.ProductoMateriasPrimas)
                .HasForeignKey(p => p.MateriaPrimaId);
            pmp.HasData(productoMateriasPrimas);
        });

        // Configuración de Compra
        modelBuilder.Entity<Compra>(compra =>
        {
            compra.HasKey(c => c.Id);
            compra.Property(c => c.NumeroCompra).IsRequired().HasMaxLength(50);
            compra.Property(c => c.FechaCompra).HasDefaultValueSql("GETDATE()");
            compra.Property(c => c.SubTotal).HasColumnType("decimal(10,2)");
            compra.Property(c => c.Total).HasColumnType("decimal(10,2)");
            compra.Property(c => c.Estado).HasMaxLength(20).HasDefaultValue("Pendiente");
            compra.Property(c => c.Observaciones).HasMaxLength(500);
            compra.HasOne(c => c.Proveedor)
                .WithMany(p => p.Compras)
                .HasForeignKey(c => c.ProveedorId);
            compra.HasData(compras);
        });

        // Configuración de CompraDetalle
        modelBuilder.Entity<CompraDetalle>(detalle =>
        {
            detalle.HasKey(d => d.Id);
            detalle.Property(d => d.Cantidad).HasColumnType("decimal(10,2)");
            detalle.Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");
            detalle.Property(d => d.SubTotal).HasColumnType("decimal(10,2)");
            detalle.HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraId);
            detalle.HasOne(d => d.MateriaPrima)
                .WithMany(m => m.CompraDetalles)
                .HasForeignKey(d => d.MateriaPrimaId);
            detalle.HasData(compraDetalles);
        });

        // Configuración de Venta
        modelBuilder.Entity<Venta>(venta =>
        {
            venta.HasKey(v => v.Id);
            venta.Property(v => v.NumeroVenta).IsRequired().HasMaxLength(50);
            venta.Property(v => v.FechaVenta).HasDefaultValueSql("GETDATE()");
            venta.Property(v => v.PrecioUnitario).HasColumnType("decimal(10,2)");
            venta.Property(v => v.SubTotal).HasColumnType("decimal(10,2)");
            venta.Property(v => v.Total).HasColumnType("decimal(10,2)");
            venta.Property(v => v.Estado).HasMaxLength(20).HasDefaultValue("Pendiente");
            venta.Property(v => v.DireccionEnvio).HasMaxLength(500);
            venta.Property(v => v.Observaciones).HasMaxLength(500);
            venta.HasOne(v => v.Usuario)
                .WithMany(u => u.Ventas)
                .HasForeignKey(v => v.UsuarioId);
            venta.HasOne(v => v.Producto)
                .WithMany(p => p.Ventas)
                .HasForeignKey(v => v.ProductoId);
            venta.HasData(ventas);
        });

        // Configuración de Cotizacion
        modelBuilder.Entity<Cotizacion>(cotizacion =>
        {
            cotizacion.HasKey(c => c.Id);
            cotizacion.Property(c => c.NumeroCotizacion).IsRequired().HasMaxLength(50);
            cotizacion.Property(c => c.FechaCotizacion).HasDefaultValueSql("GETDATE()");
            cotizacion.Property(c => c.NombreCliente).IsRequired().HasMaxLength(100);
            cotizacion.Property(c => c.CorreoCliente).IsRequired().HasMaxLength(100);
            cotizacion.Property(c => c.TelefonoCliente).IsRequired().HasMaxLength(20);
            cotizacion.Property(c => c.EmpresaCliente).HasMaxLength(100);
            cotizacion.Property(c => c.PrecioUnitario).HasColumnType("decimal(10,2)");
            cotizacion.Property(c => c.SubTotal).HasColumnType("decimal(10,2)");
            cotizacion.Property(c => c.Total).HasColumnType("decimal(10,2)");
            cotizacion.Property(c => c.Estado).HasMaxLength(20).HasDefaultValue("Pendiente");
            cotizacion.Property(c => c.Observaciones).HasMaxLength(1000);
            cotizacion.Property(c => c.RequirimientosEspeciales).HasMaxLength(1000);
            cotizacion.HasOne(c => c.Producto)
                .WithMany()
                .HasForeignKey(c => c.ProductoId);
            cotizacion.HasData(cotizaciones);
        });
    }
}