// Services/Autor/AutorService.cs
// Implementación del servicio de gestión de Autores con Entity Framework Core.

using LibrosAutoresApi.Models; // Para referenciar la clase Autor
using LibrosAutoresApi.Data; // Para inyectar AppDbContext
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Necesario para ToListAsync, FirstOrDefaultAsync
using System.Threading.Tasks; // Necesario para Task

using LibrosAutoresApi.Exceptions; // Para usar tu excepción NotFoundException
using Microsoft.Extensions.Logging; // Para el logger

namespace LibrosAutoresApi.Services.Autor
{
    public class AutorService : IAutorService // Implementa la interfaz IAutorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AutorService> _logger;

        public AutorService(AppDbContext context, ILogger<AutorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para obtener todos los autores
        public async Task<IEnumerable<Models.Autor>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los autores...");
            return await _context.Autores
                                 .Include(a => a.Libros)
                                 .Include(a => a.Biografia)
                                 .Include(a => a.AutorEventos)
                                    .ThenInclude(ae => ae.Evento)
                                 .ToListAsync();
        }

        // Método para obtener un autor por su ID
        public async Task<Models.Autor?> GetById(int id)
        {
            _logger.LogInformation("Intentando obtener autor con ID: {AutorId}", id);
            var autor = await _context.Autores
                                 .Include(a => a.Libros)
                                 .Include(a => a.Biografia)
                                 .Include(a => a.AutorEventos)
                                    .ThenInclude(ae => ae.Evento)
                                 .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null)
            {
                _logger.LogWarning("Autor con ID {AutorId} no encontrado.", id);
                throw new NotFoundException($"Autor con ID {id} no encontrado.");
            }
            _logger.LogInformation("Autor con ID {AutorId} encontrado exitosamente.", id);
            return autor;
        }

        // ¡MÉTODO ADD AÑADIDO Y CORREGIDO AQUÍ!
        public async Task<Models.Autor> Add(Models.Autor nuevoAutor)
        {
            _logger.LogInformation("Agregando nuevo autor: {AutorNombre}", nuevoAutor.Nombre);
            await _context.Autores.AddAsync(nuevoAutor);
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos.
            _logger.LogInformation("Autor {AutorNombre} (ID: {AutorId}) agregado exitosamente.", nuevoAutor.Nombre, nuevoAutor.Id);
            return nuevoAutor;
        }

        // Método para actualizar un autor existente
        public async Task<bool> Update(Models.Autor autorActualizado)
        {
            _logger.LogInformation("Intentando actualizar autor con ID: {AutorId}", autorActualizado.Id);
            var autorExistente = await _context.Autores.FirstOrDefaultAsync(a => a.Id == autorActualizado.Id);
            if (autorExistente == null)
            {
                _logger.LogWarning("Intento de actualizar autor con ID {AutorId} fallido: no encontrado.", autorActualizado.Id);
                throw new NotFoundException($"Autor con ID {autorActualizado.Id} no encontrado para actualizar.");
            }

            // Actualiza las propiedades del autor existente.
            _context.Entry(autorExistente).CurrentValues.SetValues(autorActualizado);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Autor con ID {AutorId} actualizado exitosamente.", autorActualizado.Id);
            return true;
        }

        // Método para eliminar un autor por su ID
        public async Task<bool> Delete(int id)
        {
            _logger.LogInformation("Intentando eliminar autor con ID: {AutorId}", id);
            var autorAEliminar = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autorAEliminar == null)
            {
                _logger.LogWarning("Intento de eliminar autor con ID {AutorId} fallido: no encontrado.", id);
                throw new NotFoundException($"Autor con ID {id} no encontrado para eliminar.");
            }

            _context.Autores.Remove(autorAEliminar);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Autor con ID {AutorId} eliminado exitosamente.", id);
            return true;
        }
    }
}