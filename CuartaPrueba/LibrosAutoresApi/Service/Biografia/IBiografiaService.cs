// Services/Biografia/IBiografiaService.cs
// Interfaz para el servicio de gestión de Biografías.

using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks; // Para Task
using LibrosAutoresApi.Models; // Necesario para referenciar la clase Biografia (el modelo)

namespace LibrosAutoresApi.Services.Biografia
{
    public interface IBiografiaService
    {
        Task<Models.Biografia?> GetByAutorId(int autorId); // Ahora asíncrono
        Task<Models.Biografia?> Add(Models.Biografia nuevaBiografia); // Ahora asíncrono
        Task<bool> Update(Models.Biografia biografiaActualizada); // Ahora asíncrono
        Task<bool> Delete(int autorId); // Ahora asíncrono
    }
}