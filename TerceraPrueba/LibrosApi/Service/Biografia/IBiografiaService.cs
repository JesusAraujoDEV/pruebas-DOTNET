// Services/Biografia/IBiografiaService.cs
// Interfaz para el servicio de gestión de Biografías.

using System.Collections.Generic; // Para IEnumerable
using LibrosApi.Models; // Necesario para referenciar la clase Biografia (el modelo)

namespace LibrosApi.Services.Biografia
{
    public interface IBiografiaService
    {
        Models.Biografia? GetByAutorId(int autorId); // Obtener biografía por ID de autor
        Models.Biografia? Add(Models.Biografia nuevaBiografia); // Agregar una nueva biografía
        bool Update(Models.Biografia biografiaActualizada); // Actualizar una biografía existente
        bool Delete(int autorId); // Eliminar una biografía por ID de autor
    }
}