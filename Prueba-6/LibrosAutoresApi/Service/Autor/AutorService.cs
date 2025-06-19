// Services/Autor/AutorService.cs
// Implementación del servicio de gestión de Autores con Entity Framework Core.

using LibrosAutoresApi.Models;
using LibrosAutoresApi.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using LibrosAutoresApi.Exceptions;
using Microsoft.Extensions.Logging;
using LibrosAutoresApi.Dtos.Autor; // ¡NUEVO! Para ActualizarAutorDto

namespace LibrosAutoresApi.Services.Autor
{
    public class AutorService : IAutorService
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

        // ¡MÉTODO UPDATE CAMBIADO PARA PATCH!
        public async Task<bool> Update(int id, ActualizarAutor autorDto)
        {
            _logger.LogInformation("Intentando actualizar autor parcialmente con ID: {AutorId}", id);
            var autorExistente = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autorExistente == null)
            {
                _logger.LogWarning("Intento de actualizar autor con ID {AutorId} fallido: no encontrado.", id);
                throw new NotFoundException($"Autor con ID {id} no encontrado para actualizar.");
            }

            // Aplicar solo las propiedades que se proporcionaron en el DTO (no son nulas)
            if (autorDto.Nombre != null)
            {
                autorExistente.Nombre = autorDto.Nombre;
            }
            if (autorDto.FechaNacimiento.HasValue) // Para tipos anulables como DateTime?
            {
                autorExistente.FechaNacimiento = autorDto.FechaNacimiento.Value;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Autor con ID {AutorId} actualizado exitosamente.", id);
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