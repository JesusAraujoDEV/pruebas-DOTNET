// Services/Libro/LibroService.cs
// Implementación del servicio de gestión de Libros con Entity Framework Core.

using LibrosAutoresApi.Models; // Para referenciar la clase Libro
using LibrosAutoresApi.Services.Autor; // Para inyectar IAutorService
using LibrosAutoresApi.Data; // Para inyectar AppDbContext
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Necesario para ToListAsync, FirstOrDefaultAsync
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Services.Libro
{
    public class LibroService : ILibroService // Implementa la interfaz ILibroService
    {
        private readonly IAutorService _autorService;
        private readonly AppDbContext _context; // Inyectamos el contexto de la base de datos

        public LibroService(IAutorService autorService, AppDbContext context)
        {
            _autorService = autorService;
            _context = context;
        }

        // Método para obtener todos los libros
        public async Task<IEnumerable<Models.Libro>> GetAll()
        {
            return await _context.Libros
                                 .Include(l => l.Autor)
                                 .ToListAsync();
        }

        // Método para obtener un libro por su ID
        public async Task<Models.Libro?> GetById(int id)
        {
            return await _context.Libros
                                 .Include(l => l.Autor)
                                 .FirstOrDefaultAsync(l => l.Id == id);
        }

        // Método para obtener libros por AutorId
        public async Task<IEnumerable<Models.Libro>> GetByAutorId(int autorId)
        {
            var autorExiste = await _autorService.GetById(autorId);
            if (autorExiste == null)
            {
                return Enumerable.Empty<Models.Libro>();
            }

            return await _context.Libros
                                 .Include(l => l.Autor)
                                 .Where(l => l.AutorId == autorId)
                                 .ToListAsync();
        }

        // Método para agregar un nuevo libro
        public async Task<Models.Libro?> Add(Models.Libro nuevoLibro)
        {
            var autorExiste = await _autorService.GetById(nuevoLibro.AutorId);
            if (autorExiste == null)
            {
                return null;
            }

            await _context.Libros.AddAsync(nuevoLibro);
            await _context.SaveChangesAsync();
            return nuevoLibro;
        }

        // Método para actualizar un libro existente
        public async Task<bool> Update(Models.Libro libroActualizado)
        {
            var autorExiste = await _autorService.GetById(libroActualizado.AutorId);
            if (autorExiste == null)
            {
                return false;
            }

            var libroExistente = await _context.Libros.FirstOrDefaultAsync(l => l.Id == libroActualizado.Id);
            if (libroExistente == null)
            {
                return false;
            }

            _context.Entry(libroExistente).CurrentValues.SetValues(libroActualizado);
            await _context.SaveChangesAsync();
            return true;
        }

        // Método para eliminar un libro por su ID
        public async Task<bool> Delete(int id)
        {
            var libroAEliminar = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libroAEliminar == null)
            {
                return false;
            }

            _context.Libros.Remove(libroAEliminar);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}