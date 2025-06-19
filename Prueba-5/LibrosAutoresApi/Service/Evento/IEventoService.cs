// Services/Evento/IEventoService.cs
// Interfaz para el servicio de gestión de Eventos.

using LibrosAutoresApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks; // Para Task

namespace LibrosAutoresApi.Services.Evento
{
    public interface IEventoService
    {
        Task<IEnumerable<Models.Evento>> GetAll();
        Task<Models.Evento?> GetById(int id);
        Task<Models.Evento> Add(Models.Evento nuevoEvento);
        Task<bool> Update(Models.Evento eventoActualizado);
        Task<bool> Delete(int id);
        Task<bool> AddAutorToEvento(int eventoId, int autorId); // Para relación M-M
        Task<bool> RemoveAutorFromEvento(int eventoId, int autorId); // Para relación M-M
        Task<IEnumerable<Models.Autor>> GetAutoresByEventoId(int eventoId); // Para relación M-M (obtener autores de un evento)
    }
}