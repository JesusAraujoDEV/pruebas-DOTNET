// Services/Evento/EventoService.cs
// Implementación del servicio de gestión de Eventos con Entity Framework Core.

using LibrosAutoresApi.Models;
using LibrosAutoresApi.Services.Autor; // Para inyectar IAutorService
using LibrosAutoresApi.Data; // Para inyectar AppDbContext
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Necesario para ToListAsync, FirstOrDefaultAsync, Include
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Services.Evento
{
    public class EventoService : IEventoService
    {
        private readonly IAutorService _autorService;
        private readonly AppDbContext _context; // Inyectamos el contexto de la base de datos

        public EventoService(IAutorService autorService, AppDbContext context)
        {
            _autorService = autorService;
            _context = context;
        }

        public async Task<IEnumerable<Models.Evento>> GetAll()
        {
            // Para eventos, también puedes optar por no cargar las relaciones por defecto
            // o cargar solo los AutorEventos. Aquí los incluimos para que se vean.
            return await _context.Eventos
                                 .Include(e => e.AutorEventos)
                                    .ThenInclude(ae => ae.Autor) // Incluye el Autor dentro de AutorEventos
                                 .ToListAsync();
        }

        public async Task<Models.Evento?> GetById(int id)
        {
            return await _context.Eventos
                                 .Include(e => e.AutorEventos)
                                    .ThenInclude(ae => ae.Autor)
                                 .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Models.Evento> Add(Models.Evento nuevoEvento)
        {
            await _context.Eventos.AddAsync(nuevoEvento);
            await _context.SaveChangesAsync();
            return nuevoEvento;
        }

        public async Task<bool> Update(Models.Evento eventoActualizado)
        {
            var eventoExistente = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == eventoActualizado.Id);
            if (eventoExistente == null)
            {
                return false;
            }

            _context.Entry(eventoExistente).CurrentValues.SetValues(eventoActualizado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var eventoAEliminar = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id);
            if (eventoAEliminar == null)
            {
                return false;
            }

            _context.Eventos.Remove(eventoAEliminar);
            await _context.SaveChangesAsync(); // EF Core manejará la eliminación de AutorEventos relacionados por cascada.
            return true;
        }

        public async Task<bool> AddAutorToEvento(int eventoId, int autorId)
        {
            // Validar que el evento y el autor existan en la DB
            var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == eventoId);
            var autor = await _autorService.GetById(autorId); // Usamos IAutorService para obtener el autor

            if (evento == null || autor == null)
            {
                return false; // Evento o Autor no existen.
            }

            // Validar que la relación no exista ya
            if (await _context.AutorEventos.AnyAsync(ae => ae.EventoId == eventoId && ae.AutorId == autorId))
            {
                return false; // La relación ya existe.
            }

            // Crear y agregar la nueva entrada en la tabla de unión.
            var nuevaRelacion = new Models.AutorEvento { EventoId = eventoId, AutorId = autorId };
            await _context.AutorEventos.AddAsync(nuevaRelacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAutorFromEvento(int eventoId, int autorId)
        {
            var relacion = await _context.AutorEventos.FirstOrDefaultAsync(ae => ae.EventoId == eventoId && ae.AutorId == autorId);
            if (relacion == null)
            {
                return false; // La relación no existe.
            }

            _context.AutorEventos.Remove(relacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Models.Autor>> GetAutoresByEventoId(int eventoId)
        {
            // Validar que el evento exista.
            if (await _context.Eventos.FirstOrDefaultAsync(e => e.Id == eventoId) == null)
            {
                return Enumerable.Empty<Models.Autor>(); // Evento no existe.
            }

            // Obtener los autores asociados a este evento a través de la tabla de unión.
            // Usamos .Include() para cargar el objeto Autor completo.
            return await _context.AutorEventos
                                 .Where(ae => ae.EventoId == eventoId)
                                 .Select(ae => ae.Autor!) // Seleccionamos el Autor (se que no será nulo porque la FK es obligatoria)
                                 .ToListAsync();
        }
    }
}