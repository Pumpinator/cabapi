using System.Security.Cryptography;
using System.Text;
using cabapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
    public DbSet<VentaDetalle> VentaDetalles { get; set; }

    public CABDB(DbContextOptions<CABDB> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Usuario> usuarios = new List<Usuario>
            {
                new Usuario {
                    Id = 1,
                    Nombre = "Super Administrador",
                    Correo = "superadmin@utleon.edu.mx",
                    Rol = Rol.SuperAdmin,
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
                    Rol = Rol.Admin,
                    Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("admin123"))),
                    FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0),
                    FechaUltimoAcceso = new DateTime(2025, 7, 6, 12, 0, 0),
                    EnLinea = false,
                    Activo = true
                },
                new Usuario {
                    Id = 3,
                    Nombre = "Alejandro",
                    Correo = "alejandro@gmail.com",
                    Rol = Rol.Cliente,
                    Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("alejandro2025"))),
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
                new Deteccion { Id = 1, Tipo = Tipo.Organico, FechaHora = new DateTime(2025, 7, 6, 12, 0, 0), ClasificadorId = 1 },
                new Deteccion { Id = 2, Tipo = Tipo.Valorizable, FechaHora = new DateTime(2025, 7, 6, 12, 15, 0), ClasificadorId = 2 },
                new Deteccion { Id = 3, Tipo = Tipo.NoValorizable, FechaHora = new DateTime(2025, 7, 6, 12, 30, 0), ClasificadorId = 3 }
            };

        List<Comentario> comentarios = new List<Comentario>
            {
                new Comentario {
                    Id = 1,
                    Texto = "Excelente sistema de clasificación automática. Muy útil para nuestro centro de trabajo.",
                    FechaHora = new DateTime(2025, 7, 10, 10, 30, 0),
                    UsuarioId = 3,
                    Calificacion = 5,
                    Activo = true,
                    ProductoId = 2
                },
                new Comentario {
                    Id = 2,
                    Texto = "La precisión del clasificador es impresionante. Recomiendo totalmente este producto.",
                    FechaHora = new DateTime(2025, 7, 15, 14, 20, 0),
                    UsuarioId = 3,
                    Calificacion = 5,
                    Activo = true,
                    ProductoId = 1
                },
                new Comentario {
                    Id = 3,
                    Texto = "Fácil instalación y excelentes resultados. El soporte técnico es muy bueno.",
                    FechaHora = new DateTime(2025, 7, 20, 16, 45, 0),
                    UsuarioId = 3,
                    Calificacion = 4,
                    Activo = true,
                    ProductoId = 1
                }
            };

        List<Proveedor> proveedores = new List<Proveedor>
            {
                new Proveedor {
                    Id = 1,
                    Nombre = "TechComponents SA de CV",
                    Contacto = new[] { "ventas@techcomponents.com", "477-123-4567", "Av. Tecnológico 123, León, Guanajuato" },
                    Activo = true
                },
                new Proveedor {
                    Id = 2,
                    Nombre = "PlásticosMX",
                    Contacto = new[] { "contacto@plasticosmx.com", "477-234-5678", "Blvd. Industrial 456, León, Guanajuato" },
                    Activo = true
                },
                new Proveedor {
                    Id = 3,
                    Nombre = "Sensores y Más",
                    Contacto = new[] { "info@sensoresymas.com", "477-345-6789", "Zona Industrial Norte 789, León, Guanajuato" },
                    Activo = true
                }
            };

        List<MateriaPrima> materiasPrimas = new List<MateriaPrima>
            {
                new MateriaPrima {
                    Id = 1,
                    Nombre = "ESP32-CAM",
                    Descripcion = "Módulo miniatura microcontrolador con cámara integrada para IoT",
                    Stock = 3,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 1, 9, 0, 0)
                },
                new MateriaPrima {
                    Id = 2,
                    Nombre = "ESP32-S3",
                    Descripcion = "Módulo microcontrolador con capacidades mejoradas para IoT",
                    Stock = 3,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 2, 10, 0, 0)
                },
                new MateriaPrima {
                    Id = 3,
                    Nombre = "SG90",
                    Descripcion = "Servo motor SG90 de 9g",
                    Stock = 15,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 3, 11, 0, 0)
                },
                new MateriaPrima {
                    Id = 4,
                    Nombre = "HC SR04",
                    Descripcion = "Sensor de distancia ultrasónico HC-SR04",
                    Stock = 5,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 4, 12, 0, 0)
                },
                new MateriaPrima {
                    Id = 5,
                    Nombre = "Carcasa Plástica",
                    Descripcion = "Carcasa plástica resistente para dispositivos IoT",
                    Stock = 10,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 5, 13, 0, 0)
                },
                new MateriaPrima {
                    Id = 6,
                    Nombre = "PCA9685",
                    Descripcion = "Controlador de servos PCA9685",
                    Stock = 2,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 5, 13, 0, 0)
                },
                new MateriaPrima {
                    Id = 7,
                    Nombre = "Buzzer Pasivo",
                    Descripcion = "Buzzer pasivo para alertas sonoras",
                    Stock = 5,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 5, 13, 0, 0)
                }
            };

        List<Producto> productos = new List<Producto>
            {
                new Producto {
                    Id = 1,
                    Nombre = "CAB Clasificador Mini",
                    Descripcion = "Sistema básico de clasificación automática de basura con ESP32-CAM. Incluye inteligencia artificial para identificar residuos orgánicos, valorizables y no valorizables.",
                    Precio = 2500.00m,
                    Costo = 1200.00m,
                    Stock = 25,
                    Imagen = "/images/productos/cab-mini.jpg",
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 15, 9, 0, 0)
                },
                new Producto {
                    Id = 2,
                    Nombre = "CAB Clasificador",
                    Descripcion = "Sistema avanzado con sensores adicionales, conectividad WiFi mejorada y dashboard en tiempo real. Perfecto para oficinas y centros educativos.",
                    Precio = 4200.00m,
                    Costo = 2100.00m,
                    Stock = 15,
                    Imagen = "/images/productos/cab-basico.jpg",
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 6, 16, 10, 0, 0)
                }
            };

        List<ProductoMateriaPrima> productoMateriasPrimas = new List<ProductoMateriaPrima>
            {
                new ProductoMateriaPrima { Id = 1, ProductoId = 1, MateriaPrimaId = 1, Cantidad = 1}, // ESP32-CAM
                new ProductoMateriaPrima { Id = 2, ProductoId = 1, MateriaPrimaId = 4, Cantidad = 1}, // Sensor Ultrasónico
                new ProductoMateriaPrima { Id = 3, ProductoId = 1, MateriaPrimaId = 5, Cantidad = 1}, // Carcasa
                new ProductoMateriaPrima { Id = 4, ProductoId = 1, MateriaPrimaId = 7, Cantidad = 1}, // Buzzer

                new ProductoMateriaPrima { Id = 5, ProductoId = 2, MateriaPrimaId = 2, Cantidad = 1}, // ESP32-S3
                new ProductoMateriaPrima { Id = 6, ProductoId = 2, MateriaPrimaId = 5, Cantidad = 1}, // Carcasa
                new ProductoMateriaPrima { Id = 7, ProductoId = 2, MateriaPrimaId = 3, Cantidad = 3}, // Servo SG90
                new ProductoMateriaPrima { Id = 8, ProductoId = 2, MateriaPrimaId = 4, Cantidad = 1}, // Sensor Ultrasónico
                new ProductoMateriaPrima { Id = 9, ProductoId = 2, MateriaPrimaId = 7, Cantidad = 1}, // Buzzer
                new ProductoMateriaPrima { Id = 10, ProductoId = 2, MateriaPrimaId = 6, Cantidad = 1}, // PCA9685
        };

        List<Compra> compras = new List<Compra>
        {
            new Compra
            {
                Id = 1,
                NumeroCompra = "CP-2025-001",
                FechaCompra = new DateTime(2025, 6, 10, 9, 0, 0),
                ProveedorId = 1,
                SubTotal = 2400.00m,
                Total = 2784.00m,        // con IVA 16%
                Estatus = Estatus.Pagada,
                Observaciones = "Reabastecimiento de módulos ESP32-CAM"
            },
            new Compra
            {
                Id = 2,
                NumeroCompra = "CP-2025-002",
                FechaCompra = new DateTime(2025, 6, 11, 14, 30, 0),
                ProveedorId = 2,
                SubTotal = 1000.00m,
                Total = 1160.00m,        // con IVA 16%
                Estatus = Estatus.Entregada,
                Observaciones = "Compra de servos SG90 y carcasas plásticas"
            }
        };

        List<CompraDetalle> compraDetalles = new List<CompraDetalle>
        {
            new CompraDetalle
            {
                Id = 1,
                CompraId = 1,
                MateriaPrimaId = 1,
                Cantidad = 4,
                PrecioUnitario = 600.00m,
                SubTotal = 2400.00m
            },
            new CompraDetalle
            {
                Id = 2,
                CompraId = 2,
                MateriaPrimaId = 3,
                Cantidad = 5,
                PrecioUnitario = 100.00m,
                SubTotal = 500.00m
            },
            new CompraDetalle
            {
                Id = 3,
                CompraId = 2,
                MateriaPrimaId = 5,
                Cantidad = 2,
                PrecioUnitario = 250.00m,
                SubTotal = 500.00m
            }
        };

        List<Venta> ventas = new List<Venta>
            {
                new Venta {
                    Id = 1,
                    NumeroVenta = "V-2025-001",
                    FechaVenta = new DateTime(2025, 7, 10, 15, 30, 0),
                    UsuarioId = 3,
                    Cantidad = 2,
                    PrecioUnitario = 2500.00m,
                    SubTotal = 5000.00m,
                    Total = 5800.00m,
                    Estatus = Estatus.Entregada, // Pendiente, Pagada, Enviada, Entregada, Cancelada
                    DireccionEnvio = "Universidad Tecnológica de León, Blvd. Universidad Tecnológica #225, León, Gto.",
                    Observaciones = "Instalación en edificios A y B de la universidad"
                },
                new Venta {
                    Id = 2,
                    NumeroVenta = "V-2025-002",
                    FechaVenta = new DateTime(2025, 7, 15, 11, 45, 0),
                    UsuarioId = 3,
                    Cantidad = 1,
                    PrecioUnitario = 4200.00m,
                    SubTotal = 4200.00m,
                    Total = 4872.00m,
                    Estatus = Estatus.EnProceso, // Pendiente, Pagada, Enviada, Entregada, Cancelada
                    DireccionEnvio = "Universidad Tecnológica de León, Blvd. Universidad Tecnológica #225, León, Gto.",
                    Observaciones = "Sistema para cafetería principal"
                }
            };

        List<VentaDetalle> ventaDetalles = new List<VentaDetalle>
            {
                new VentaDetalle { Id = 1, VentaId = 1, ProductoId = 1, Cantidad = 2, PrecioUnitario = 2500.00m, SubTotal = 5000.00m },
                new VentaDetalle { Id = 2, VentaId = 2, ProductoId = 2, Cantidad = 1, PrecioUnitario = 4200.00m, SubTotal = 4200.00m }
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
            deteccion.Property(d => d.Tipo)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);
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
            usuario.Property(u => u.Rol)
                .HasConversion<string>()
                .IsRequired()
                .HasDefaultValue(Rol.Cliente);
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

        modelBuilder.Entity<Comentario>(comentario =>
        {
            comentario.HasKey(c => c.Id);
            comentario.Property(c => c.Texto).IsRequired().HasMaxLength(1000);
            comentario.Property(c => c.FechaHora).HasDefaultValueSql("GETDATE()");
            comentario.Property(c => c.Activo).HasDefaultValue(true);
            comentario.HasOne(c => c.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.UsuarioId);
            comentario.HasOne(c => c.Producto).
                WithMany(p => p.Comentarios).
                HasForeignKey(c => c.ProductoId);
            comentario.HasData(comentarios);
        });

        modelBuilder.Entity<Proveedor>(proveedor =>
        {
            proveedor.HasKey(p => p.Id);
            proveedor.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            var contactoProp = proveedor.Property(p => p.Contacto)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries));
            contactoProp.HasColumnType("nvarchar(500)");
            contactoProp.Metadata.SetValueComparer(new ValueComparer<string[]>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? Array.Empty<string>() : c.ToArray()));
            proveedor.Property(p => p.Activo).HasDefaultValue(true);
            proveedor.HasData(proveedores);
        });

        modelBuilder.Entity<MateriaPrima>(materia =>
        {
            materia.HasKey(m => m.Id);
            materia.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
            materia.Property(m => m.Descripcion).IsRequired().HasMaxLength(500);
            materia.Property(m => m.Stock);
            materia.Property(m => m.Activo).HasDefaultValue(true);
            materia.Property(m => m.FechaCreacion).HasDefaultValueSql("GETDATE()");
            materia.HasData(materiasPrimas);
        });

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

        modelBuilder.Entity<ProductoMateriaPrima>(pmp =>
        {
            pmp.HasKey(p => p.Id);
            pmp.Property(p => p.Cantidad);
            pmp.HasOne(p => p.Producto)
                .WithMany(pr => pr.ProductoMateriasPrimas)
                .HasForeignKey(p => p.ProductoId);
            pmp.HasOne(p => p.MateriaPrima)
                .WithMany(m => m.ProductoMateriasPrimas)
                .HasForeignKey(p => p.MateriaPrimaId);
            pmp.HasData(productoMateriasPrimas);
        });

        modelBuilder.Entity<Compra>(compra =>
        {
            compra.HasKey(c => c.Id);
            compra.Property(c => c.NumeroCompra).IsRequired().HasMaxLength(50);
            compra.Property(c => c.FechaCompra).HasDefaultValueSql("GETDATE()");
            compra.Property(c => c.SubTotal).HasColumnType("decimal(10,2)");
            compra.Property(c => c.Total).HasColumnType("decimal(10,2)");
            compra.Property(c => c.Estatus).HasDefaultValue(Estatus.Pendiente).HasConversion<string>();
            compra.Property(c => c.Observaciones).HasMaxLength(500);
            compra.HasData(compras);
        });

        modelBuilder.Entity<CompraDetalle>(detalle =>
        {
            detalle.HasKey(d => d.Id);
            detalle.Property(d => d.Cantidad);
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

        modelBuilder.Entity<Venta>(venta =>
        {
            venta.HasKey(v => v.Id);
            venta.Property(v => v.NumeroVenta).IsRequired().HasMaxLength(50);
            venta.Property(v => v.FechaVenta).HasDefaultValueSql("GETDATE()");
            venta.Property(v => v.PrecioUnitario).HasColumnType("decimal(10,2)");
            venta.Property(v => v.SubTotal).HasColumnType("decimal(10,2)");
            venta.Property(v => v.Total).HasColumnType("decimal(10,2)");
            venta.Property(v => v.Estatus).HasDefaultValue(Estatus.Pendiente).HasConversion<string>();
            venta.Property(v => v.DireccionEnvio).HasMaxLength(500);
            venta.Property(v => v.Observaciones).HasMaxLength(500);
            venta.HasOne(v => v.Usuario)
                .WithMany(u => u.Ventas)
                .HasForeignKey(v => v.UsuarioId);
            venta.HasMany(v => v.Detalles)
                .WithOne(d => d.Venta)
                .HasForeignKey(d => d.VentaId);
            venta.HasData(ventas);
        });

        modelBuilder.Entity<VentaDetalle>(detalle =>
        {
            detalle.HasKey(d => d.Id);
            detalle.Property(d => d.Cantidad);
            detalle.Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");
            detalle.Property(d => d.SubTotal).HasColumnType("decimal(10,2)");
            detalle.HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaId);
            detalle.HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId);
            detalle.HasData(ventaDetalles);
        });
    }
}