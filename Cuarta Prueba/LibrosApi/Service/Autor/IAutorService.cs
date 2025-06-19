// Services/Autor/IAutorService.cs
// Interfaz (contrato) para el servicio de gestión de Autores.

using LibrosApi.Models; // Para referenciar la clase Autor
using System.Collections.Generic; // Para IEnumerable

namespace LibrosApi.Services.Autor
{
    public interface IAutorService
    {
        IEnumerable<Models.Autor> GetAll(); // Obtener todos los autores
        Models.Autor? GetById(int id); // Obtener un autor por su ID
        Models.Autor Add(Models.Autor nuevoAutor); // Agregar un nuevo autor
        bool Update(Models.Autor autorActualizado); // Actualizar un autor existente
        bool Delete(int id); // Eliminar un autor por su ID
    }
}
