// Dtos/Libro/ActualizarLibroDto.cs
// DTO para actualizar un Libro existente.

using System.ComponentModel.DataAnnotations; // Para atributos de validación

namespace LibrosAutoresApi.Dtos.Libro
{
    public class ActualizarLibroDto
    {
        [Required(ErrorMessage = "El ID del libro es obligatorio para actualizar.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título del libro es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres.")]
        public required string Titulo { get; set; }

        [Range(1000, 2025, ErrorMessage = "El año de publicación debe estar entre 1000 y 2025.")]
        public int AnioPublicacion { get; set; }

        [Required(ErrorMessage = "El ID del autor es obligatorio para asociar el libro.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del autor debe ser un número positivo.")]
        public int AutorId { get; set; }
    }
}
