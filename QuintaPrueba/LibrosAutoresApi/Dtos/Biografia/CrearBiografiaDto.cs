// Dtos/Biografia/CrearBiografiaDto.cs
// DTO para crear una nueva Biografia.

using System.ComponentModel.DataAnnotations;

namespace LibrosAutoresApi.Dtos.Biografia
{
    public class CrearBiografiaDto
    {
        [Required(ErrorMessage = "El ID del autor es obligatorio para la biografía.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del autor debe ser un número positivo.")]
        public int AutorId { get; set; } // El ID del autor al que pertenece esta biografía

        [Required(ErrorMessage = "El contenido de la biografía es obligatorio.")]
        [MinLength(50, ErrorMessage = "La biografía debe tener al menos 50 caracteres.")]
        public required string Contenido { get; set; }
    }
}