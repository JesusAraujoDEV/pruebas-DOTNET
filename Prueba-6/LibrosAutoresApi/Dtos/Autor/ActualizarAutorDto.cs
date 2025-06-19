// Dtos/Autor/ActualizarAutorDto.cs
// DTO para actualizar parcialmente un Autor.

using System; // Para DateTime?
using System.ComponentModel.DataAnnotations; // Para StringLength (opcional en propiedades nullable)

namespace LibrosAutoresApi.Dtos.Autor
{
    public class ActualizarAutorDto
    {
        // Las propiedades son nullable (?) para indicar que son opcionales
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string? Nombre { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}