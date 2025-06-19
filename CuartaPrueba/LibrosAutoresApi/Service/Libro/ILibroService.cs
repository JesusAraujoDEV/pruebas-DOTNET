// Services/Libro/ILibroService.cs
// Interfaz (contrato) para el servicio de gestión de Libros.

using LibrosAutoresApi.Models; // Para referenciar la clase Libro
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Services.Libro
{
    public interface ILibroService
    {
        Task<IEnumerable<Models.Libro>> GetAll(); // Ahora devuelve Task<IEnumerable<Libro>>
        Task<Models.Libro?> GetById(int id); // Ahora devuelve Task<Libro?>
        Task<IEnumerable<Models.Libro>> GetByAutorId(int autorId); // Ahora devuelve Task<IEnumerable<Libro>>
        Task<Models.Libro?> Add(Models.Libro nuevoLibro); // Ahora devuelve Task<Libro?>
        Task<bool> Update(Models.Libro libroActualizado); // Ahora devuelve Task<bool>
        Task<bool> Delete(int id); // Ahora devuelve Task<bool>
    }
}