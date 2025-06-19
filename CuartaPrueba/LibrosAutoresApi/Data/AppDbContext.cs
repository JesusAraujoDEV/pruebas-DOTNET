// Data/AppDbContext.cs
// Contexto de la base de datos para Entity Framework Core.

using Microsoft.EntityFrameworkCore; // Necesario para DbContext y sus métodos
using LibrosAutoresApi.Models; // Necesario para acceder a tus modelos (Autor, Libro, Biografia, Evento, AutorEvento)

namespace LibrosAutoresApi.Data
{
    public class AppDbContext : DbContext // Hereda de DbContext
    {
        // Define tus "tablas" como propiedades DbSet.
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Biografia> Biografias { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<AutorEvento> AutorEventos { get; set; } // DbSet para la tabla de unión M-M

        // Constructor que recibe las opciones del contexto (ej. el proveedor de base de datos).
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Este método se usa para configurar el modelo de la base de datos
        // y las relaciones entre tus entidades.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Llama a la implementación base

            // **************** Configuración de Relaciones ****************

            // Relación UNO A UNO (Autor y Biografia):
            modelBuilder.Entity<Autor>()
                .HasOne(a => a.Biografia)
                .WithOne(b => b.Autor)
                .HasForeignKey<Biografia>(b => b.AutorId)
                .IsRequired(false); // La relación es opcional

            // Relación MUCHOS A MUCHOS (Autor y Evento) a través de AutorEvento:
            modelBuilder.Entity<AutorEvento>()
                .HasKey(ae => new { ae.AutorId, ae.EventoId });

            modelBuilder.Entity<AutorEvento>()
                .HasOne(ae => ae.Autor)
                .WithMany(a => a.AutorEventos)
                .HasForeignKey(ae => ae.AutorId);

            modelBuilder.Entity<AutorEvento>()
                .HasOne(ae => ae.Evento)
                .WithMany(e => e.AutorEventos)
                .HasForeignKey(ae => ae.EventoId);

            // Relación UNO A MUCHOS (Autor y Libro):
            modelBuilder.Entity<Autor>()
                .HasMany(a => a.Libros)
                .WithOne(l => l.Autor)
                .HasForeignKey(l => l.AutorId)
                .IsRequired(); // Un Libro siempre debe tener un Autor

            // **************** Seed Data (Datos Iniciales) ****************
            // Aquí definimos los datos que se cargarán en la base de datos en memoria al inicio.

            // Seed Autores
            modelBuilder.Entity<Autor>().HasData(
                new Autor { Id = 1, Nombre = "Gabriel García Márquez", FechaNacimiento = new DateTime(1927, 3, 6) },
                new Autor { Id = 2, Nombre = "Miguel de Cervantes", FechaNacimiento = new DateTime(1547, 9, 29) },
                new Autor { Id = 3, Nombre = "George Orwell", FechaNacimiento = new DateTime(1903, 6, 25) }
            );

            // Seed Biografias (asociadas a Autores existentes)
            modelBuilder.Entity<Biografia>().HasData(
                new Biografia { AutorId = 1, Contenido = "Gabriel García Márquez fue un escritor y periodista colombiano. Nació en Aracataca en 1927. Conocido por sus novelas y cuentos, fue el máximo exponente del realismo mágico. Recibió el Premio Nobel de Literatura en 1982." }
            );

            // Seed Eventos
            modelBuilder.Entity<Evento>().HasData(
                new Evento { Id = 201, Nombre = "Feria del Libro de Bogotá", Fecha = new DateTime(2025, 4, 20), Ubicacion = "Corferias" },
                new Evento { Id = 202, Nombre = "Conferencia de Escritores", Fecha = new DateTime(2025, 7, 15), Ubicacion = "Medellín" }
            );

            // Seed AutorEventos (relaciones Muchos-a-Muchos)
            modelBuilder.Entity<AutorEvento>().HasData(
                new AutorEvento { AutorId = 1, EventoId = 201 }, // García Márquez en Feria del Libro
                new AutorEvento { AutorId = 3, EventoId = 202 }  // George Orwell en Conferencia
            );

            // Seed Libros
            modelBuilder.Entity<Libro>().HasData(
                new Libro { Id = 101, Titulo = "Cien años de soledad", AnioPublicacion = 1967, AutorId = 1 },
                new Libro { Id = 102, Titulo = "El amor en los tiempos del cólera", AnioPublicacion = 1985, AutorId = 1 },
                new Libro { Id = 103, Titulo = "Don Quijote de la Mancha", AnioPublicacion = 1605, AutorId = 2 },
                new Libro { Id = 104, Titulo = "La granja de los animales", AnioPublicacion = 1945, AutorId = 3 },
                new Libro { Id = 105, Titulo = "1984", AnioPublicacion = 1949, AutorId = 3 }
            );
        }
    }
}