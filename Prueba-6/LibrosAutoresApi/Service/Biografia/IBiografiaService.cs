// Services/Biografia/IBiografiaService.cs
// Interfaz para el servicio de gestión de Biografías.

using System.Collections.Generic;
using System.Threading.Tasks;
using LibrosAutoresApi.Models;
using LibrosAutoresApi.Dtos.Biografia; // ¡NUEVO! Para ActualizarBiografiaDto

namespace LibrosAutoresApi.Services.Biografia
{
    public interface IBiografiaService
    {
        Task<Models.Biografia?> GetByAutorId(int autorId);
        Task<Models.Biografia?> Add(Models.Biografia nuevaBiografia);
        // ¡CAMBIO AQUÍ! Ahora el Update acepta el DTO de PATCH.
        Task<bool> Update(int autorId, ActualizarBiografiaDto biografiaDto);
        Task<bool> Delete(int autorId);
    }
}