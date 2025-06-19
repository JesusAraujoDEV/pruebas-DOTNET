// Dtos/Evento/ActualizarEventoDto.cs
// DTO para actualizar un Evento existente.

using System.ComponentModel.DataAnnotations;

namespace LibrosAutoresApi.Dtos.Evento
{
    public class ActualizarEventoDto
    {
        [Required(ErrorMessage = "El ID del evento es obligatorio para actualizar.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La fecha del evento es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La ubicación del evento es obligatoria.")]
        [StringLength(100, ErrorMessage = "La ubicación no puede exceder los 100 caracteres.")]
        public required string Ubicacion { get; set; }
    }
}