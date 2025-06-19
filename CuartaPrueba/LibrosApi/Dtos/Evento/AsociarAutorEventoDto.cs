// Dtos/Evento/AsociarAutorEventoDto.cs
// DTO para asociar/desasociar un autor a un evento (para la relación M-M).
// Este DTO se usa para enviar los IDs de los recursos a relacionar.

using System.ComponentModel.DataAnnotations;

namespace LibrosApi.Dtos.Evento
{
    public class AsociarAutorEventoDto
    {
        [Required(ErrorMessage = "El ID del autor es obligatorio.")]
        public int AutorId { get; set; }
        // El EventoId se tomará de la ruta en el controlador, no es necesario aquí.
    }
}