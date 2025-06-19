// Services/Evento/EventoService.cs
// Implementación del servicio de gestión de Eventos con Entity Framework Core.

using LibrosAutoresApi.Models;
using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using LibrosAutoresApi.Exceptions;
using Microsoft.Extensions.Logging;
using LibrosAutoresApi.Dtos.Evento; // ¡NUEVO! Para ActualizarEventoDto

namespace LibrosAutoresApi.Services.Evento
{
    public class EventoService : IEventoService
    {
        private readonly IAutorService _autorService;
        private readonly AppDbContext _context;
        private readonly ILogger<EventoService> _logger; // Inyectar logger

        public EventoService(IAutorService autorService, AppDbContext context, ILogger<EventoService> logger)
        {
            _autorService = autorService;
            _context = context;
            _logger = logger;
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

        // ¡MÉTODO UPDATE CAMBIADO PARA PATCH!
        public async Task<bool> Update(int id, ActualizarEventoDto eventoDto)
        {
            _logger.LogInformation("Intentando actualizar evento parcialmente con ID: {EventoId}", id);
            var eventoExistente = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id);
            if (eventoExistente == null)
            {
                _logger.LogWarning("Intento de actualizar evento con ID {EventoId} fallido: no encontrado.", id);
                throw new NotFoundException($"Evento con ID {id} no encontrado para actualizar.");
            }

            // Aplicar solo las propiedades que se proporcionaron en el DTO (no son nulas)
            if (eventoDto.Nombre != null)
            {
                eventoExistente.Nombre = eventoDto.Nombre;
            }
            if (eventoDto.Fecha.HasValue)
            {
                eventoExistente.Fecha = eventoDto.Fecha.Value;
            }
            if (eventoDto.Ubicacion != null)
            {
                eventoExistente.Ubicacion = eventoDto.Ubicacion;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Evento con ID {EventoId} actualizado exitosamente.", id);
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