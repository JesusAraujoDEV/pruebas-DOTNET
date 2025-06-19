// Services/Autor/AutorService.cs
// Implementación del servicio de gestión de Autores.

using LibrosAutoresApi.Models; // Para referenciar la clase Autor
using System.Collections.Generic; // Para List<T>
using System.Linq; // Para LINQ (Any, Max, FirstOrDefault, Remove)

namespace LibrosAutoresApi.Services.Autor
{
    public class AutorService : IAutorService
    {
        // Simulación de una base de datos de autores en memoria.
        private static List<Models.Autor> _autores = new List<Models.Autor>
        {
            new Models.Autor { Id = 1, Nombre = "Gabriel García Márquez", FechaNacimiento = new DateTime(1927, 3, 6) },
            new Models.Autor { Id = 2, Nombre = "Miguel de Cervantes", FechaNacimiento = new DateTime(1547, 9, 29) },
            new Models.Autor { Id = 3, Nombre = "George Orwell", FechaNacimiento = new DateTime(1903, 6, 25) }
        };

        public IEnumerable<Models.Autor> GetAll()
        {
            return _autores;
        }

        public Models.Autor? GetById(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
        }

        public Models.Autor Add(Models.Autor nuevoAutor)
        {
            // Asigna un nuevo ID al autor
            nuevoAutor.Id = _autores.Any() ? _autores.Max(a => a.Id) + 1 : 1;
            _autores.Add(nuevoAutor);
            return nuevoAutor;
        }

        public bool Update(Models.Autor autorActualizado)
        {
            var autorExistente = _autores.FirstOrDefault(a => a.Id == autorActualizado.Id);
            if (autorExistente == null)
            {
                return false;
            }

            autorExistente.Nombre = autorActualizado.Nombre;
            autorExistente.FechaNacimiento = autorActualizado.FechaNacimiento;
            return true;
        }

        public bool Delete(int id)
        {
            var autorAEliminar = _autores.FirstOrDefault(a => a.Id == id);
            if (autorAEliminar == null)
            {
                return false;
            }

            // Aquí se debería manejar la eliminación de libros asociados si fuera una DB real,
            // o actualizar sus AutorId a null si la relación lo permite.
            // Para el ejercicio in-memory, solo eliminamos el autor.
            _autores.Remove(autorAEliminar);
            return true;
        }
    }
}
