// Dtos/Autor/CrearAutorDto.cs
// DTO para crear un nuevo Autor.

using System.ComponentModel.DataAnnotations; // Para atributos de validación

namespace LibrosApi.Dtos.Autor
{
    public class CrearAutorDto
    {
        [Required(ErrorMessage = "El nombre del autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        // Aunque se almacena como DateTime, podemos pedirla como string para un mejor control
        // y luego parsearla en el servicio/controlador.
        // O si se espera un formato ISO 8601 (yyyy-MM-dd), se puede usar un DateTime directamente
        // y ASP.NET Core intentará parsearlo del JSON. Mantendremos DateTime para simplificar la deserialización JSON.
        public DateTime FechaNacimiento { get; set; }
    }
}
