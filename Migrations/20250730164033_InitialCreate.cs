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
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnLinea = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
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
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonas", x => x.Id);
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
                    ZonaId = table.Column<int>(type: "int", nullable: false)
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
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Correo", "EnLinea", "FechaCreacion", "FechaUltimoAcceso", "Nombre", "Password" },
                values: new object[] { 1, true, "admin@utleon.edu.mx", false, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "root", "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=" });

            migrationBuilder.InsertData(
                table: "Zonas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Edificio A" },
                    { 2, "Edificio B" },
                    { 3, "Edificio C" },
                    { 4, "Edificio C Pesado" },
                    { 5, "Edificio D" },
                    { 6, "Cafetería" },
                    { 7, "Edificio CVD" },
                    { 8, "Edificio F" },
                    { 9, "Edificio de Rectoría" },
                    { 10, "Almacén" }
                });

            migrationBuilder.InsertData(
                table: "Clasificadores",
                columns: new[] { "Id", "FechaCreacion", "Latitud", "Longitud", "Nombre", "ZonaId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), 21.0635822m, -101.5803752m, "Entrada Principal", 5 },
                    { 2, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), 21.0636721m, -101.580911m, "Pasillo", 5 },
                    { 3, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), 21.0636795m, -101.5804751m, "Pasillo Superior", 5 }
                });

            migrationBuilder.InsertData(
                table: "Detecciones",
                columns: new[] { "Id", "ClasificadorId", "FechaHora", "Tipo" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "Organico" },
                    { 2, 2, new DateTime(2025, 7, 6, 12, 15, 0, 0, DateTimeKind.Unspecified), "Valorizable" },
                    { 3, 3, new DateTime(2025, 7, 6, 12, 30, 0, 0, DateTimeKind.Unspecified), "No Valorizanble" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clasificadores_ZonaId",
                table: "Clasificadores",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Detecciones_ClasificadorId",
                table: "Detecciones",
                column: "ClasificadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Detecciones");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Clasificadores");

            migrationBuilder.DropTable(
                name: "Zonas");
        }
    }
}
