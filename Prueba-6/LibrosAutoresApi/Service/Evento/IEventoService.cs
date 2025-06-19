// Services/Evento/IEventoService.cs
// Interfaz para el servicio de gestión de Eventos.

using LibrosAutoresApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibrosAutoresApi.Dtos.Evento; // ¡NUEVO! Para ActualizarEventoDto

namespace LibrosAutoresApi.Services.Evento
{
    public interface IEventoService
    {
        Task<IEnumerable<Models.Evento>> GetAll();
        Task<Models.Evento?> GetById(int id);
        Task<Models.Evento> Add(Models.Evento nuevoEvento);
        // ¡CAMBIO AQUÍ! Ahora el Update acepta el DTO de PATCH.
        Task<bool> Update(int id, ActualizarEventoDto eventoDto);
        Task<bool> Delete(int id);
        Task<bool> AddAutorToEvento(int eventoId, int autorId);
        Task<bool> RemoveAutorFromEvento(int eventoId, int autorId);
        Task<IEnumerable<Models.Autor>> GetAutoresByEventoId(int eventoId);
    }
}