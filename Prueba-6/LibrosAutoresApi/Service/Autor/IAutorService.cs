// Services/Autor/IAutorService.cs
// Interfaz (contrato) para el servicio de gestión de Autores.

using LibrosAutoresApi.Models; // Para referenciar la clase Autor
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks; // Para Task
using LibrosAutoresApi.Dtos.Autor; // ¡NUEVO! Para ActualizarAutorDto

namespace LibrosAutoresApi.Services.Autor
{
    public interface IAutorService
    {
        Task<IEnumerable<Models.Autor>> GetAll();
        Task<Models.Autor?> GetById(int id);
        Task<Models.Autor> Add(Models.Autor nuevoAutor);
        // ¡CAMBIO AQUÍ! Ahora el Update acepta el DTO de PATCH.
        Task<bool> Update(int id, ActualizarAutor autorDto);
        Task<bool> Delete(int id);
    }
}