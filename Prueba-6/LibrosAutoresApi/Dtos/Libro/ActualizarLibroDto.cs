// Dtos/Libro/ActualizarLibroDto.cs
// DTO para actualizar parcialmente un Libro.

using System.ComponentModel.DataAnnotations; // Para StringLength, Range

namespace LibrosAutoresApi.Dtos.Libro
{
    public class ActualizarLibroDto
    {
        // Las propiedades son nullable (?) para indicar que son opcionales
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres.")]
        public string? Titulo { get; set; }

        [Range(1000, 2025, ErrorMessage = "El año de publicación debe estar entre 1000 y 2025.")]
        public int? AnioPublicacion { get; set; }

        // El AutorId también es opcional, pero si se envía, debe ser válido.
        public int? AutorId { get; set; }
    }
}