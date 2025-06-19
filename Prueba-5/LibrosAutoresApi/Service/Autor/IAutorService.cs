// Services/Autor/IAutorService.cs
// Interfaz (contrato) para el servicio de gestión de Autores.

using LibrosAutoresApi.Models; // Para referenciar la clase Autor
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks; // Necesario para Task (para métodos asíncronos)

namespace LibrosAutoresApi.Services.Autor
{
    public interface IAutorService
    {
        // Todos los métodos retornan Task<T> para indicar que son asíncronos
        Task<IEnumerable<Models.Autor>> GetAll();
        Task<Models.Autor?> GetById(int id);
        Task<Models.Autor> Add(Models.Autor nuevoAutor);
        Task<bool> Update(Models.Autor autorActualizado);
        Task<bool> Delete(int id);
    }
}