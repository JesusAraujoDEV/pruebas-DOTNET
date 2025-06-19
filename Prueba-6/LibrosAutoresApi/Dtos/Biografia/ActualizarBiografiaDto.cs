// Dtos/Biografia/ActualizarBiografiaDto.cs
// DTO para actualizar parcialmente una Biografía.

using System.ComponentModel.DataAnnotations; // Para MinLength

namespace LibrosAutoresApi.Dtos.Biografia
{
    public class ActualizarBiografiaDto
    {
        // El contenido es nullable (?)
        [MinLength(50, ErrorMessage = "El contenido de la biografía debe tener al menos 50 caracteres.")]
        public string? Contenido { get; set; }
    }
}