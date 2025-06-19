// Dtos/Biografia/ActualizarBiografiaDto.cs
// DTO para actualizar una Biografia existente.

using System.ComponentModel.DataAnnotations;

namespace LibrosAutoresApi.Dtos.Biografia
{
    public class ActualizarBiografiaDto
    {
        [Required(ErrorMessage = "El ID del autor es obligatorio para actualizar la biografía.")]
        public int AutorId { get; set; } // El ID del autor, que también es la clave primaria de Biografia

        [Required(ErrorMessage = "El contenido de la biografía es obligatorio.")]
        [MinLength(50, ErrorMessage = "La biografía debe tener al menos 50 caracteres.")]
        public required string Contenido { get; set; }
    }
}