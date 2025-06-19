// Services/Autor/AutorService.cs
// Implementación del servicio de gestión de Autores con Entity Framework Core.

using LibrosAutoresApi.Models; // Para referenciar la clase Autor
using LibrosAutoresApi.Data; // Para inyectar AppDbContext
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Necesario para ToListAsync, FirstOrDefaultAsync
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Services.Autor
{
    public class AutorService : IAutorService // Implementa la interfaz IAutorService
    {
        private readonly AppDbContext _context; // Inyectamos el contexto de la base de datos

        // Constructor para la inyección de dependencias de AppDbContext.
        public AutorService(AppDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los autores
        public async Task<IEnumerable<Models.Autor>> GetAll() // Debe coincidir con la interfaz
        {
            // .Include() carga las relaciones (Libros, Biografia, AutorEventos)
            // .ToListAsync() ejecuta la consulta de forma asíncrona y retorna una lista.
            return await _context.Autores
                                 .Include(a => a.Libros)
                                 .Include(a => a.Biografia)
                                 .Include(a => a.AutorEventos)
                                    .ThenInclude(ae => ae.Evento) // Incluye el Evento dentro de AutorEventos
                                 .ToListAsync();
        }

        // Método para obtener un autor por su ID
        public async Task<Models.Autor?> GetById(int id) // Debe coincidir con la interfaz
        {
            // .FirstOrDefaultAsync() busca de forma asíncrona y retorna el primer elemento o null.
            return await _context.Autores
                                 .Include(a => a.Libros)
                                 .Include(a => a.Biografia)
                                 .Include(a => a.AutorEventos)
                                    .ThenInclude(ae => ae.Evento)
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Método para agregar un nuevo autor
        public async Task<Models.Autor> Add(Models.Autor nuevoAutor) // Debe coincidir con la interfaz
        {
            // EF Core asignará el ID automáticamente al guardar los cambios.
            await _context.Autores.AddAsync(nuevoAutor);
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos en memoria.
            return nuevoAutor;
        }

        // Método para actualizar un autor existente
        public async Task<bool> Update(Models.Autor autorActualizado) // Debe coincidir con la interfaz
        {
            var autorExistente = await _context.Autores.FirstOrDefaultAsync(a => a.Id == autorActualizado.Id);
            if (autorExistente == null)
            {
                return false; // El autor no fue encontrado.
            }

            // Actualiza las propiedades del autor existente.
            _context.Entry(autorExistente).CurrentValues.SetValues(autorActualizado);
            await _context.SaveChangesAsync(); // Guarda los cambios.
            return true;
        }

        // Método para eliminar un autor por su ID
        public async Task<bool> Delete(int id) // Debe coincidir con la interfaz
        {
            var autorAEliminar = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autorAEliminar == null)
            {
                return false; // El autor no fue encontrado.
            }

            _context.Autores.Remove(autorAEliminar); // Marca el autor para eliminación.
            await _context.SaveChangesAsync(); // Guarda los cambios. EF Core manejará cascadas.
            return true;
        }
    }
}