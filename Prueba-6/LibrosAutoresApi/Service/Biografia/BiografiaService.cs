// Services/Biografia/BiografiaService.cs
// Implementación del servicio de gestión de Biografías con Entity Framework Core.

using LibrosAutoresApi.Models;
using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using LibrosAutoresApi.Exceptions;
using Microsoft.Extensions.Logging;
using LibrosAutoresApi.Dtos.Biografia; // ¡NUEVO! Para ActualizarBiografiaDto

namespace LibrosAutoresApi.Services.Biografia
{
    public class BiografiaService : IBiografiaService
    {
        private readonly IAutorService _autorService;
        private readonly AppDbContext _context;
        private readonly ILogger<BiografiaService> _logger; // Inyectar logger

        public BiografiaService(IAutorService autorService, AppDbContext context, ILogger<BiografiaService> logger)
        {
            _autorService = autorService;
            _context = context;
            _logger = logger;
        }

        public async Task<Models.Biografia?> GetByAutorId(int autorId)
        {
            return await _context.Biografias
                                 .Include(b => b.Autor) // Incluimos el objeto Autor asociado
                                 .FirstOrDefaultAsync(b => b.AutorId == autorId);
        }

        public async Task<Models.Biografia?> Add(Models.Biografia nuevaBiografia)
        {
            // 1. Validar que el AutorId exista (usando el servicio de autores)
            if (await _autorService.GetById(nuevaBiografia.AutorId) == null)
            {
                return null; // Autor no existe.
            }

            // 2. Validar que el autor NO TENGA YA una biografía (relación 1 a 1)
            if (await _context.Biografias.AnyAsync(b => b.AutorId == nuevaBiografia.AutorId))
            {
                return null; // El autor ya tiene una biografía.
            }

            await _context.Biografias.AddAsync(nuevaBiografia);
            await _context.SaveChangesAsync();
            return nuevaBiografia;
        }

        // ¡MÉTODO UPDATE CAMBIADO PARA PATCH!
        public async Task<bool> Update(int autorId, ActualizarBiografiaDto biografiaDto)
        {
            _logger.LogInformation("Intentando actualizar biografía parcialmente para AutorId: {AutorId}", autorId);
            var biografiaExistente = await _context.Biografias.FirstOrDefaultAsync(b => b.AutorId == autorId);
            if (biografiaExistente == null)
            {
                _logger.LogWarning("Intento de actualizar biografía para AutorId {AutorId} fallido: no encontrada.", autorId);
                throw new NotFoundException($"Biografía no encontrada para el autor con ID {autorId} para actualizar.");
            }

            // Aplicar solo las propiedades que se proporcionaron en el DTO (no son nulas)
            if (biografiaDto.Contenido != null)
            {
                biografiaExistente.Contenido = biografiaDto.Contenido;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Biografía para AutorId {AutorId} actualizada exitosamente.", autorId);
            return true;
        }

        public async Task<bool> Delete(int autorId)
        {
            var biografiaAEliminar = await _context.Biografias.FirstOrDefaultAsync(b => b.AutorId == autorId);
            if (biografiaAEliminar == null)
            {
                return false; // Biografía no encontrada.
            }

            _context.Biografias.Remove(biografiaAEliminar);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}