// Services/Libro/LibroService.cs
// Implementación del servicio de gestión de Libros con Entity Framework Core.

using LibrosAutoresApi.Models;
using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using LibrosAutoresApi.Exceptions;
using Microsoft.Extensions.Logging;
using LibrosAutoresApi.Dtos.Libro; // ¡NUEVO! Para ActualizarLibroDto

namespace LibrosAutoresApi.Services.Libro
{
    public class LibroService : ILibroService
    {
        private readonly IAutorService _autorService;
        private readonly AppDbContext _context;
        private readonly ILogger<LibroService> _logger; // Inyectar logger

        public LibroService(IAutorService autorService, AppDbContext context, ILogger<LibroService> logger)
        {
            _autorService = autorService;
            _context = context;
            _logger = logger;
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

        // ¡MÉTODO UPDATE CAMBIADO PARA PATCH!
        public async Task<bool> Update(int id, ActualizarLibroDto libroDto)
        {
            _logger.LogInformation("Intentando actualizar libro parcialmente con ID: {LibroId}", id);
            var libroExistente = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libroExistente == null)
            {
                _logger.LogWarning("Intento de actualizar libro con ID {LibroId} fallido: no encontrado.", id);
                throw new NotFoundException($"Libro con ID {id} no encontrado para actualizar.");
            }

            // Aplicar solo las propiedades que se proporcionaron en el DTO (no son nulas)
            if (libroDto.Titulo != null)
            {
                libroExistente.Titulo = libroDto.Titulo;
            }
            if (libroDto.AnioPublicacion.HasValue)
            {
                libroExistente.AnioPublicacion = libroDto.AnioPublicacion.Value;
            }
            if (libroDto.AutorId.HasValue)
            {
                // Si el AutorId se proporciona, validamos que exista antes de asignarlo
                var nuevoAutor = await _autorService.GetById(libroDto.AutorId.Value);
                if (nuevoAutor == null)
                {
                    _logger.LogWarning("Intento de actualizar libro {LibroId} con AutorId {AutorId} fallido: autor no encontrado.", id, libroDto.AutorId.Value);
                    // Lanzamos una excepción de BadRequest para indicar que el AutorId no es válido
                    // Podrías crear una excepción personalizada para BadRequest, pero para este caso
                    // lanzar ArgumentException es un sustituto simple que el middleware capturará.
                    throw new ArgumentException($"El Autor con ID {libroDto.AutorId.Value} no existe.");
                }
                libroExistente.AutorId = libroDto.AutorId.Value;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Libro con ID {LibroId} actualizado exitosamente.", id);
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