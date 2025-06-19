// Models/Autor.cs
// Define la estructura de un Autor con múltiples tipos de relaciones.

using System.Collections.Generic; // Para ICollection

namespace LibrosAutoresApi.Models
{
    public class Autor
    {
        public int Id { get; set; } // ID único del autor
        public required string Nombre { get; set; } // Nombre completo del autor
        public DateTime FechaNacimiento { get; set; } // Fecha de nacimiento del autor

        // Relación UNO A MUCHOS: Un Autor puede tener muchos Libros.
        // `Libros` es una propiedad de navegación que representa la colección de libros de este autor.
        public ICollection<Libro> Libros { get; set; } = new List<Libro>();

        // Relación UNO A UNO: Un Autor puede tener una Biografia.
        // `Biografia` es la propiedad de navegación que representa la biografía asociada a este autor.
        // `Biografia?` indica que puede ser nula (un autor podría no tener biografía aún).
        public Biografia? Biografia { get; set; }

        // Relación MUCHOS A MUCHOS: Un Autor participa en muchos Eventos.
        // `AutorEventos` es la propiedad de navegación a la tabla de unión `AutorEvento`.
        // Cada entrada en `AutorEventos` representa la participación de este autor en un evento.
        public ICollection<AutorEvento> AutorEventos { get; set; } = new List<AutorEvento>();
    }
}
