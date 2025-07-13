using System.Security.Cryptography;
using System.Text;
using cabapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Empresa
{
    public class CABDB : DbContext
    {
        public DbSet<Clasificador> Clasificadores { get; set; }
        public DbSet<Deteccion> Detecciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Zona> Zonas { get; set; }

        public CABDB(DbContextOptions<CABDB> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Usuario> usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "root", Correo = "admin@utleon.edu.mx", Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("password"))) } // password hasheado
            };

            List<Zona> zonas = new List<Zona>
            {
                new Zona { Id = 1, Nombre = "Edificio A" },
                new Zona { Id = 2, Nombre = "Edificio B" },
                new Zona { Id = 3, Nombre = "Edificio C" },
                new Zona { Id = 4, Nombre = "Edificio C Pesado" },
                new Zona { Id = 5, Nombre = "Edificio D" },
                new Zona { Id = 6, Nombre = "Cafetería" },
                new Zona { Id = 7, Nombre = "Edificio CVD" },
                new Zona { Id = 8, Nombre = "Edificio F" },
                new Zona { Id = 9, Nombre = "Edificio de Rectoría" },
                new Zona { Id = 10, Nombre = "Almacén" }
            };
            
            List<Clasificador> clasificadores = new List<Clasificador>
            {
                new Clasificador { Id = 1, Nombre = "Entrada Principal", Latitud = 21.0635822m, Longitud = -101.5803752m, FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0), ZonaId = 5 },
                new Clasificador { Id = 2, Nombre = "Pasillo", Latitud = 21.0636721m, Longitud = -101.580911m, FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0), ZonaId = 5 },
                new Clasificador { Id = 3, Nombre = "Pasillo Superior", Latitud = 21.0636795m, Longitud = -101.5804751m, FechaCreacion = new DateTime(2025, 7, 6, 12, 0, 0), ZonaId = 5 }
            };

            List<Deteccion> detecciones = new List<Deteccion>
            {
                new Deteccion { Id = 1, Tipo = "Organico", FechaHora = new DateTime(2025, 7, 6, 12, 0, 0), ClasificadorId = 1 },
                new Deteccion { Id = 2, Tipo = "Valorizable", FechaHora = new DateTime(2025, 7, 6, 12, 15, 0), ClasificadorId = 2 },
                new Deteccion { Id = 3, Tipo = "No Valorizanble", FechaHora = new DateTime(2025, 7, 6, 12, 30, 0), ClasificadorId = 3 }
            };

            modelBuilder.Entity<Clasificador>(clasificador =>
            {
                clasificador.HasKey(c => c.Id);
                clasificador.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                clasificador.Property(c => c.Latitud).HasColumnType("decimal(9,6)");
                clasificador.Property(c => c.Longitud).HasColumnType("decimal(9,6)");
                clasificador.Property(c => c.FechaCreacion).HasDefaultValueSql("GETDATE()");
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
                usuario.HasData(usuarios);
            });

            modelBuilder.Entity<Zona>(zona =>
            {
                zona.HasKey(z => z.Id);
                zona.Property(z => z.Nombre).IsRequired().HasMaxLength(100);
                zona.HasData(zonas);
            });
        }
    }
}