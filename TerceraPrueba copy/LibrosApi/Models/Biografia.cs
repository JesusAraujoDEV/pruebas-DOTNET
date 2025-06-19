// Models/Biografia.cs
// Define la estructura de una Biografia, en una relación uno a uno con Autor.

using System.ComponentModel.DataAnnotations; // Para atributos como Key

namespace LibrosApi.Models
{
    public class Biografia
    {
        // En una relación uno a uno, el Id de la Biografia a menudo es también la clave foránea
        // y primaria que apunta al Autor relacionado.
        [Key] // Marca esta propiedad como la clave primaria
        public int AutorId { get; set; } // También es la clave foránea que apunta a Autor

        [Required(ErrorMessage = "El contenido de la biografía es obligatorio.")]
        public required string Contenido { get; set; } // El texto de la biografía

        // Propiedad de navegación para la relación UNO A UNO: Una Biografia pertenece a un solo Autor.
        // Esto indica a qué Autor pertenece esta biografía.
        public Autor Autor { get; set; } = default!; // `default!` indica que será inicializado por EF Core/DI
    }
}
