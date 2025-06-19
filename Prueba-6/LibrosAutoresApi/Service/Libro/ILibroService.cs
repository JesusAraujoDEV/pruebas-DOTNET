// Services/Libro/ILibroService.cs
// Interfaz (contrato) para el servicio de gestión de Libros.

using LibrosAutoresApi.Models; // Para referenciar la clase Libro
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks; // Para Task
using LibrosAutoresApi.Dtos.Libro; // ¡NUEVO! Para ActualizarLibroDto

namespace LibrosAutoresApi.Services.Libro
{
    public interface ILibroService
    {
        Task<IEnumerable<Models.Libro>> GetAll();
        Task<Models.Libro?> GetById(int id);
        Task<IEnumerable<Models.Libro>> GetByAutorId(int autorId);
        Task<Models.Libro?> Add(Models.Libro nuevoLibro);
        Task<bool> Update(int id, ActualizarLibroDto libroDto);
        Task<bool> Delete(int id);
    }
}