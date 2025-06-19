// Services/Evento/EventoService.cs
// Implementación del servicio de gestión de Eventos.

using LibrosApi.Models; // Necesario para referenciar las clases de modelos (Evento, Autor, AutorEvento)
using LibrosApi.Services.Autor; // Para inyectar IAutorService
using System.Collections.Generic;
using System.Linq;

namespace LibrosApi.Services.Evento
{
    public class EventoService : IEventoService
    {
        private readonly IAutorService _autorService;

        // Simulación de base de datos en memoria para Eventos
        private static List<Models.Evento> _eventos = new List<Models.Evento> // Corregido: List<Models.Evento>
        {
            new Models.Evento { Id = 201, Nombre = "Feria del Libro de Bogotá", Fecha = new DateTime(2025, 4, 20), Ubicacion = "Corferias" },
            new Models.Evento { Id = 202, Nombre = "Conferencia de Escritores", Fecha = new DateTime(2025, 7, 15), Ubicacion = "Medellín" }
        };

        // Simulación de la tabla de unión AutorEvento en memoria
        private static List<Models.AutorEvento> _autorEventos = new List<Models.AutorEvento> // Corregido: List<Models.AutorEvento>
        {
            new Models.AutorEvento { AutorId = 1, EventoId = 201 }, // García Márquez en Feria del Libro
            new Models.AutorEvento { AutorId = 3, EventoId = 202 }  // George Orwell en Conferencia
        };

        public EventoService(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IEnumerable<Models.Evento> GetAll() // Corregido: Models.Evento
        {
            return _eventos;
        }

        public Models.Evento? GetById(int id) // Corregido: Models.Evento?
        {
            return _eventos.FirstOrDefault(e => e.Id == id);
        }

        public Models.Evento Add(Models.Evento nuevoEvento) // Corregido: Models.Evento
        {
            nuevoEvento.Id = _eventos.Any() ? _eventos.Max(e => e.Id) + 1 : 1;
            _eventos.Add(nuevoEvento);
            return nuevoEvento;
        }

        public bool Update(Models.Evento eventoActualizado) // Corregido: Models.Evento
        {
            var eventoExistente = _eventos.FirstOrDefault(e => e.Id == eventoActualizado.Id);
            if (eventoExistente == null)
            {
                return false;
            }

            eventoExistente.Nombre = eventoActualizado.Nombre;
            eventoExistente.Fecha = eventoActualizado.Fecha;
            eventoExistente.Ubicacion = eventoActualizado.Ubicacion;
            return true;
        }

        public bool Delete(int id)
        {
            var eventoAEliminar = _eventos.FirstOrDefault(e => e.Id == id);
            if (eventoAEliminar == null)
            {
                return false;
            }

            _eventos.Remove(eventoAEliminar);
            _autorEventos.RemoveAll(ae => ae.EventoId == id);
            return true;
        }

        public bool AddAutorToEvento(int eventoId, int autorId)
        {
            if (GetById(eventoId) == null || _autorService.GetById(autorId) == null)
            {
                return false;
            }

            if (_autorEventos.Any(ae => ae.EventoId == eventoId && ae.AutorId == autorId))
            {
                return false;
            }

            _autorEventos.Add(new Models.AutorEvento { EventoId = eventoId, AutorId = autorId }); // Corregido: Models.AutorEvento
            return true;
        }

        public bool RemoveAutorFromEvento(int eventoId, int autorId)
        {
            var relacion = _autorEventos.FirstOrDefault(ae => ae.EventoId == eventoId && ae.AutorId == autorId);
            if (relacion == null)
            {
                return false;
            }

            _autorEventos.Remove(relacion);
            return true;
        }

        public IEnumerable<Models.Autor> GetAutoresByEventoId(int eventoId) // Corregido: Models.Autor
        {
            if (GetById(eventoId) == null)
            {
                return Enumerable.Empty<Models.Autor>(); // Corregido: Models.Autor
            }

            var autorIds = _autorEventos.Where(ae => ae.EventoId == eventoId).Select(ae => ae.AutorId).ToList();

            var autores = new List<Models.Autor>(); // Corregido: Models.Autor
            foreach (var id in autorIds)
            {
                var autor = _autorService.GetById(id);
                if (autor != null)
                {
                    autores.Add(autor);
                }
            }
            return autores;
        }
    }
}