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
        public DbSet<User> Users { get; set; } = default!; // 'default!' para evitar el warning de null

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

            // *** ¡IMPORTANTE! Hemos eliminado todas las llamadas a .HasData() aquí. ***
            // Para una base de datos real, los datos iniciales se gestionan con migraciones
            // o scripts de seeding separados para evitar re-insertar datos en cada inicio.
        }
    }
}