using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cabapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateriasPrimas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Stock = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriasPrimas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Producto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaUltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    EnLinea = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Rol = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, defaultValue: "cliente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zonas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroCotizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaCotizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NombreCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TelefonoCliente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmpresaCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequirimientosEspeciales = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoMateriasPrimas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    CantidadRequerida = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoMateriasPrimas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoMateriasPrimas_MateriasPrimas_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "MateriasPrimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoMateriasPrimas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroCompra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroVenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clasificadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ZonaId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clasificadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clasificadores_Zonas_ZonaId",
                        column: x => x.ZonaId,
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompraDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraDetalles_MateriasPrimas_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "MateriasPrimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Detecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ClasificadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detecciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Detecciones_Clasificadores_ClasificadorId",
                        column: x => x.ClasificadorId,
                        principalTable: "Clasificadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MateriasPrimas",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "Nombre", "PrecioUnitario", "Stock", "StockMinimo", "UnidadMedida" },
                values: new object[,]
                {
                    { 1, true, "Módulo miniatura microcontrolador con cámara integrada para IoT", new DateTime(2025, 6, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "ESP32-CAM", 350.00m, 5.00m, 10.00m, "Pieza" },
                    { 2, true, "Módulo microcontrolador con capacidades mejoradas para IoT", new DateTime(2025, 6, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "ESP32-S3", 650.00m, 10.00m, 20.00m, "Pieza" },
                    { 3, true, "Servo motor SG90 de 9g", new DateTime(2025, 6, 3, 11, 0, 0, 0, DateTimeKind.Unspecified), "SG90", 120.00m, 75.00m, 15.00m, "Pieza" },
                    { 4, true, "Sensor de distancia ultrasónico HC-SR04", new DateTime(2025, 6, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), "HC SR04", 45.00m, 30.00m, 5.00m, "Pieza" },
                    { 5, true, "Carcasa plástica resistente para dispositivos IoT", new DateTime(2025, 6, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), "Carcasa Plástica", 85.00m, 100.00m, 20.00m, "Pieza" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Costo", "Descripcion", "FechaCreacion", "Imagen", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, true, 1200.00m, "Sistema básico de clasificación automática de basura con ESP32-CAM. Incluye inteligencia artificial para identificar residuos orgánicos, valorizables y no valorizables.", new DateTime(2025, 6, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), "/images/productos/cab-mini.jpg", "CAB Clasificador Mini", 2500.00m, 25 },
                    { 2, true, 2100.00m, "Sistema avanzado con sensores adicionales, conectividad WiFi mejorada y dashboard en tiempo real. Perfecto para oficinas y centros educativos.", new DateTime(2025, 6, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), "/images/productos/cab-basico.jpg", "CAB Clasificador", 4200.00m, 15 }
                });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "Id", "Activo", "Contacto", "Nombre", "Producto" },
                values: new object[,]
                {
                    { 1, true, "ventas@techcomponents.com;477-123-4567;Av. Tecnológico 123, León, Guanajuato", "TechComponents SA de CV", "Componentes electrónicos" },
                    { 2, true, "contacto@plasticosmx.com;477-234-5678;Blvd. Industrial 456, León, Guanajuato", "PlásticosMX", "Carcasas y estructuras plásticas" },
                    { 3, true, "info@sensoresymas.com;477-345-6789;Zona Industrial Norte 789, León, Guanajuato", "Sensores y Más", "Sensores y cámaras" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Correo", "FechaCreacion", "FechaUltimoAcceso", "Nombre", "Password", "Rol" },
                values: new object[,]
                {
                    { 1, true, "superadmin@utleon.edu.mx", new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Super Administrador", "75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=", "superadmin" },
                    { 2, true, "admin@utleon.edu.mx", new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Administrador", "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=", "admin" },
                    { 3, true, "alejandro@gmail.com", new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Alejandro", "F/4GrE2x2dga5lksv3QVL2rkuo+kjyF4kxQPQW//7lo=", "cliente" }
                });

            migrationBuilder.InsertData(
                table: "Zonas",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Edificio A" },
                    { 2, true, "Edificio B" },
                    { 3, true, "Edificio C" },
                    { 4, true, "Edificio C Pesado" },
                    { 5, true, "Edificio D" },
                    { 6, true, "Cafetería" },
                    { 7, true, "Edificio CVD" },
                    { 8, true, "Edificio F" },
                    { 9, true, "Edificio de Rectoría" },
                    { 10, true, "Almacén" }
                });

            migrationBuilder.InsertData(
                table: "Clasificadores",
                columns: new[] { "Id", "Activo", "FechaCreacion", "Latitud", "Longitud", "Nombre", "ZonaId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), 21.0635822m, -101.5803752m, "Entrada Principal", 5 },
                    { 2, true, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), 21.0636721m, -101.580911m, "Pasillo", 5 },
                    { 3, true, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), 21.0636795m, -101.5804751m, "Pasillo Superior", 5 }
                });

            migrationBuilder.InsertData(
                table: "Comentarios",
                columns: new[] { "Id", "Activo", "Calificacion", "FechaHora", "Texto", "UsuarioId" },
                values: new object[,]
                {
                    { 1, true, 5, new DateTime(2025, 7, 10, 10, 30, 0, 0, DateTimeKind.Unspecified), "Excelente sistema de clasificación automática. Muy útil para nuestro centro de trabajo.", 3 },
                    { 2, true, 5, new DateTime(2025, 7, 15, 14, 20, 0, 0, DateTimeKind.Unspecified), "La precisión del clasificador es impresionante. Recomiendo totalmente este producto.", 3 },
                    { 3, true, 4, new DateTime(2025, 7, 20, 16, 45, 0, 0, DateTimeKind.Unspecified), "Fácil instalación y excelentes resultados. El soporte técnico es muy bueno.", 3 }
                });

            migrationBuilder.InsertData(
                table: "Compras",
                columns: new[] { "Id", "Estado", "FechaCompra", "NumeroCompra", "Observaciones", "ProveedorId", "SubTotal", "Total" },
                values: new object[,]
                {
                    { 1, "Completada", new DateTime(2025, 7, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "C-2025-001", "Compra de componentes ESP32-CAM para producción mensual", 1, 17500.00m, 20300.00m },
                    { 2, "En Proceso", new DateTime(2025, 7, 5, 14, 30, 0, 0, DateTimeKind.Unspecified), "C-2025-002", "Pedido de carcasas plásticas y accesorios", 2, 8500.00m, 9860.00m }
                });

            migrationBuilder.InsertData(
                table: "Cotizaciones",
                columns: new[] { "Id", "Cantidad", "CorreoCliente", "EmpresaCliente", "Estado", "FechaCotizacion", "FechaVencimiento", "NombreCliente", "NumeroCotizacion", "Observaciones", "PrecioUnitario", "ProductoId", "RequirimientosEspeciales", "SubTotal", "TelefonoCliente", "Total" },
                values: new object[,]
                {
                    { 1, 5, "maria.gonzalez@empresa.com", "Corporativo del Bajío SA", "Pendiente", new DateTime(2025, 7, 20, 9, 15, 0, 0, DateTimeKind.Unspecified), null, "María González", "COT-2025-001", "Cotización para implementación en 5 sucursales", 4200.00m, 2, "Necesitan capacitación del personal y soporte técnico por 6 meses", 21000.00m, "477-987-6543", 24360.00m },
                    { 2, 3, "roberto.h@hotel.com", "Hotel Ejecutivo León", "Aprobada", new DateTime(2025, 7, 22, 16, 20, 0, 0, DateTimeKind.Unspecified), null, "Roberto Hernández", "COT-2025-002", "Instalación en lobby, restaurante y áreas comunes", 4200.00m, 2, "Integración con sistema de gestión hotelera existente", 12600.00m, "477-456-7890", 14616.00m }
                });

            migrationBuilder.InsertData(
                table: "ProductoMateriasPrimas",
                columns: new[] { "Id", "CantidadRequerida", "MateriaPrimaId", "ProductoId", "UnidadMedida" },
                values: new object[,]
                {
                    { 1, 1.00m, 1, 1, "Pieza" },
                    { 2, 1.00m, 4, 1, "Pieza" },
                    { 3, 1.00m, 5, 1, "Pieza" },
                    { 4, 1.00m, 2, 2, "Pieza" },
                    { 5, 1.00m, 5, 2, "Pieza" },
                    { 6, 1.00m, 3, 2, "Pieza" },
                    { 7, 1.00m, 4, 2, "Pieza" }
                });

            migrationBuilder.InsertData(
                table: "Ventas",
                columns: new[] { "Id", "Cantidad", "DireccionEnvio", "Estado", "FechaVenta", "NumeroVenta", "Observaciones", "PrecioUnitario", "ProductoId", "SubTotal", "Total", "UsuarioId" },
                values: new object[,]
                {
                    { 1, 2, "Universidad Tecnológica de León, Blvd. Universidad Tecnológica #225, León, Gto.", "entregada", new DateTime(2025, 7, 10, 15, 30, 0, 0, DateTimeKind.Unspecified), "V-2025-001", "Instalación en edificios A y B de la universidad", 2500.00m, 1, 5000.00m, 5800.00m, 3 },
                    { 2, 1, "Universidad Tecnológica de León, Blvd. Universidad Tecnológica #225, León, Gto.", "en_proceso", new DateTime(2025, 7, 15, 11, 45, 0, 0, DateTimeKind.Unspecified), "V-2025-002", "Sistema para cafetería principal", 4200.00m, 2, 4200.00m, 4872.00m, 3 }
                });

            migrationBuilder.InsertData(
                table: "CompraDetalles",
                columns: new[] { "Id", "Cantidad", "CompraId", "MateriaPrimaId", "PrecioUnitario", "SubTotal" },
                values: new object[,]
                {
                    { 1, 50.00m, 1, 1, 350.00m, 17500.00m },
                    { 2, 100.00m, 2, 5, 85.00m, 8500.00m }
                });

            migrationBuilder.InsertData(
                table: "Detecciones",
                columns: new[] { "Id", "ClasificadorId", "FechaHora", "Tipo" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "organico" },
                    { 2, 2, new DateTime(2025, 7, 6, 12, 15, 0, 0, DateTimeKind.Unspecified), "valorizable" },
                    { 3, 3, new DateTime(2025, 7, 6, 12, 30, 0, 0, DateTimeKind.Unspecified), "no_valorizable" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clasificadores_ZonaId",
                table: "Clasificadores",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalles_CompraId",
                table: "CompraDetalles",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalles_MateriaPrimaId",
                table: "CompraDetalles",
                column: "MateriaPrimaId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ProveedorId",
                table: "Compras",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ProductoId",
                table: "Cotizaciones",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Detecciones_ClasificadorId",
                table: "Detecciones",
                column: "ClasificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoMateriasPrimas_MateriaPrimaId",
                table: "ProductoMateriasPrimas",
                column: "MateriaPrimaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoMateriasPrimas_ProductoId",
                table: "ProductoMateriasPrimas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ProductoId",
                table: "Ventas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "CompraDetalles");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Detecciones");

            migrationBuilder.DropTable(
                name: "ProductoMateriasPrimas");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Clasificadores");

            migrationBuilder.DropTable(
                name: "MateriasPrimas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Zonas");
        }
    }
}
