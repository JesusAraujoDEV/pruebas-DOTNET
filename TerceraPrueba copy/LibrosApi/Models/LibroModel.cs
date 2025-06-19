// Models/Libro.cs
// Define la estructura de un Libro, con una relación a Autor.

using System; // Para DateTime (si se usara para alguna propiedad de Libro, aunque no en este caso)

namespace LibrosApi.Models
{
    public class Libro
    {
        public int Id { get; set; } // ID único del libro
        public required string Titulo { get; set; } // Título del libro
        public int AnioPublicacion { get; set; } // Año de publicación

        // Clave foránea para la relación con Autor (Un Libro pertenece a un Autor)
        // ESTA ES LA PROPIEDAD QUE FALTABA
        public int AutorId { get; set; }

        // Propiedad de navegación para la relación 1-a-1 (Un Libro tiene un Autor)
        // Se marca como anulable (?) porque puede que no siempre se cargue automáticamente
        // o que al crear un Libro, el objeto Autor aún no esté completamente asociado.
        public Autor? Autor { get; set; }
    }
}