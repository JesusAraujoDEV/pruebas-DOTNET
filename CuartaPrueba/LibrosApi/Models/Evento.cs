// Models/Evento.cs
// Define la estructura de un Evento.

using System.Collections.Generic; // Para ICollection

namespace LibrosApi.Models
{
    public class Evento
    {
        public int Id { get; set; } // ID único del evento
        public required string Nombre { get; set; } // Nombre del evento
        public DateTime Fecha { get; set; } // Fecha del evento
        public required string Ubicacion { get; set; } // Lugar donde se realiza el evento

        // Propiedad de navegación para la relación MUCHOS A MUCHOS: Un Evento puede tener muchos Autores.
        // `AutorEventos` es la propiedad de navegación a la tabla de unión `AutorEvento`.
        // Cada entrada en `AutorEventos` representa un autor que participa en este evento.
        public ICollection<AutorEvento> AutorEventos { get; set; } = new List<AutorEvento>();
    }
}
