// Services/Evento/IEventoService.cs
// Interfaz para el servicio de gestión de Eventos.

using System.Collections.Generic;
using LibrosAutoresApi.Models; // Necesario para referenciar las clases Evento y Autor (los modelos)

namespace LibrosAutoresApi.Services.Evento
{
    public interface IEventoService
    {
        IEnumerable<Models.Evento> GetAll(); // Corregido: Models.Evento
        Models.Evento? GetById(int id); // Corregido: Models.Evento?
        Models.Evento Add(Models.Evento nuevoEvento); // Corregido: Models.Evento
        bool Update(Models.Evento eventoActualizado); // Corregido: Models.Evento
        bool Delete(int id);
        bool AddAutorToEvento(int eventoId, int autorId);
        bool RemoveAutorFromEvento(int eventoId, int autorId);
        IEnumerable<Models.Autor> GetAutoresByEventoId(int eventoId); // Corregido: Models.Autor
    }
}