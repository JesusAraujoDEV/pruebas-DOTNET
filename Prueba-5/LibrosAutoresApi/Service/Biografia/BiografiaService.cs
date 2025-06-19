// Services/Biografia/BiografiaService.cs
// Implementación del servicio de gestión de Biografías con Entity Framework Core.

using LibrosAutoresApi.Models; // Necesario para referenciar la clase Biografia (el modelo)
using LibrosAutoresApi.Services.Autor; // Para inyectar IAutorService
using LibrosAutoresApi.Data; // Para inyectar AppDbContext
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Necesario para FirstOrDefaultAsync
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Services.Biografia
{
    public class BiografiaService : IBiografiaService
    {
        private readonly IAutorService _autorService;
        private readonly AppDbContext _context; // Inyectamos el contexto de la base de datos

        public BiografiaService(IAutorService autorService, AppDbContext context)
        {
            _autorService = autorService;
            _context = context;
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

        public async Task<bool> Update(Models.Biografia biografiaActualizada)
        {
            var biografiaExistente = await _context.Biografias.FirstOrDefaultAsync(b => b.AutorId == biografiaActualizada.AutorId);
            if (biografiaExistente == null)
            {
                return false; // Biografía no encontrada.
            }

            // Actualiza solo la propiedad Contenido
            biografiaExistente.Contenido = biografiaActualizada.Contenido;
            await _context.SaveChangesAsync();
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