// Dtos/Evento/ActualizarEventoDto.cs
// DTO para actualizar parcialmente un Evento.

using System; // Para DateTime?
using System.ComponentModel.DataAnnotations; // Para StringLength

namespace LibrosAutoresApi.Dtos.Evento
{
    public class ActualizarEventoDto
    {
        // Las propiedades son nullable (?) para indicar que son opcionales
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string? Nombre { get; set; }

        public DateTime? Fecha { get; set; }

        [StringLength(100, ErrorMessage = "La ubicación no puede exceder los 100 caracteres.")]
        public string? Ubicacion { get; set; }
    }
}