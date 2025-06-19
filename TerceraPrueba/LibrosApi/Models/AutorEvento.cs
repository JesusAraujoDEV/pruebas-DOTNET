// Models/AutorEvento.cs
// Modelo para la tabla de unión de la relación MUCHOS A MUCHOS entre Autor y Evento.

using System; // Para DateTime (si la relación tiene un campo de fecha)

namespace LibrosApi.Models
{
    public class AutorEvento
    {
        // Claves primarias compuestas y claves foráneas
        public int AutorId { get; set; }
        public int EventoId { get; set; }

        // Propiedades de navegación para los objetos completos
        // Esto es crucial para que los ORMs (como EF Core) entiendan la relación.
        public Autor Autor { get; set; } = default!; // Autor relacionado
        public Evento Evento { get; set; } = default!; // Evento relacionado

        // Ejemplo de propiedad adicional en la tabla de unión (si la relación tiene atributos)
        // public DateTime FechaInscripcion { get; set; }
    }
}
