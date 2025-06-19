// Services/Libro/ILibroService.cs
// Interfaz (contrato) para el servicio de gestión de Libros.

using LibrosApi.Models; // Para referenciar la clase Libro y Autor
using System.Collections.Generic; // Para IEnumerable

namespace LibrosApi.Services.Libro
{
    public interface ILibroService
    {
        IEnumerable<Models.Libro> GetAll(); // Obtener todos los libros
        Models.Libro? GetById(int id); // Obtener un libro por su ID
        IEnumerable<Models.Libro> GetByAutorId(int autorId); // Obtener libros por AutorId
        Models.Libro? Add(Models.Libro nuevoLibro); // Agregar un nuevo libro
        bool Update(Models.Libro libroActualizado); // Actualizar un libro existente
        bool Delete(int id); // Eliminar un libro por su ID
    }
}
