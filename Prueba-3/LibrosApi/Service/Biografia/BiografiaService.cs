// Services/Biografia/BiografiaService.cs
// Implementación del servicio de gestión de Biografías.

using LibrosAutoresApi.Models; // Necesario para referenciar la clase Biografia (el modelo)
using LibrosAutoresApi.Services.Autor; // Para inyectar IAutorService
using System.Collections.Generic;
using System.Linq;

namespace LibrosAutoresApi.Services.Biografia
{
    public class BiografiaService : IBiografiaService
    {
        private readonly IAutorService _autorService;

        // Simulación de una base de datos de biografías en memoria.
        // La clave es AutorId porque es una relación 1-a-1.
        private static List<Models.Biografia> _biografias = new List<Models.Biografia> // Corregido: List<Models.Biografia>
        {
            new Models.Biografia { AutorId = 1, Contenido = "Gabriel García Márquez fue un escritor y periodista colombiano. Nació en Aracataca en 1927. Conocido por sus novelas y cuentos, fue el máximo exponente del realismo mágico. Recibió el Premio Nobel de Literatura en 1982." }
        };

        public BiografiaService(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public Models.Biografia? GetByAutorId(int autorId) // Corregido: Models.Biografia?
        {
            return _biografias.FirstOrDefault(b => b.AutorId == autorId);
        }

        public Models.Biografia? Add(Models.Biografia nuevaBiografia) // Corregido: Models.Biografia? y Models.Biografia
        {
            // 1. Validar que el AutorId exista
            if (_autorService.GetById(nuevaBiografia.AutorId) == null)
            {
                return null; // Autor no existe
            }

            // 2. Validar que el autor NO TENGA YA una biografía (relación 1 a 1)
            if (_biografias.Any(b => b.AutorId == nuevaBiografia.AutorId))
            {
                return null; // El autor ya tiene una biografía
            }

            _biografias.Add(nuevaBiografia);
            return nuevaBiografia;
        }

        public bool Update(Models.Biografia biografiaActualizada) // Corregido: Models.Biografia
        {
            var biografiaExistente = _biografias.FirstOrDefault(b => b.AutorId == biografiaActualizada.AutorId);
            if (biografiaExistente == null)
            {
                return false; // Biografía no encontrada
            }

            biografiaExistente.Contenido = biografiaActualizada.Contenido;
            return true;
        }

        public bool Delete(int autorId)
        {
            var biografiaAEliminar = _biografias.FirstOrDefault(b => b.AutorId == autorId);
            if (biografiaAEliminar == null)
            {
                return false; // Biografía no encontrada
            }

            _biografias.Remove(biografiaAEliminar);
            return true;
        }
    }
}