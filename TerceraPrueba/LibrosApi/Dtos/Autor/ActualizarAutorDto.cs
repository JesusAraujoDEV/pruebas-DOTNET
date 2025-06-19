// Dtos/Autor/ActualizarAutorDto.cs
// DTO para actualizar un Autor existente.

using System.ComponentModel.DataAnnotations; // Para atributos de validación

namespace LibrosApi.Dtos.Autor
{
    public class ActualizarAutorDto
    {
        [Required(ErrorMessage = "El ID del autor es obligatorio para actualizar.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime FechaNacimiento { get; set; }
    }
}
