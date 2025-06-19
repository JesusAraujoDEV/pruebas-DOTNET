// Services/Libro/LibroService.cs
// Implementación del servicio de gestión de Libros.

using LibrosApi.Models; // Para referenciar la clase Libro y Autor
using LibrosApi.Services.Autor; // Para inyectar IAutorService
using System.Collections.Generic; // Para List<T>
using System.Linq; // Para LINQ (Any, Max, FirstOrDefault, Where, Remove)

namespace LibrosApi.Services.Libro
{
    public class LibroService : ILibroService
    {
        // Referencia al servicio de Autores para validar la existencia del AutorId.
        private readonly IAutorService _autorService;

        // Simulación de una base de datos de libros en memoria.
        private static List<Models.Libro> _libros = new List<Models.Libro>
        {
            // ¡DATOS DE EJEMPLO CORREGIDOS PARA INCLUIR AutorId!
            new Models.Libro { Id = 101, Titulo = "Cien años de soledad", AnioPublicacion = 1967, AutorId = 1 },
            new Models.Libro { Id = 102, Titulo = "El amor en los tiempos del cólera", AnioPublicacion = 1985, AutorId = 1 },
            new Models.Libro { Id = 103, Titulo = "Don Quijote de la Mancha", AnioPublicacion = 1605, AutorId = 2 },
            new Models.Libro { Id = 104, Titulo = "La granja de los animales", AnioPublicacion = 1945, AutorId = 3 },
            new Models.Libro { Id = 105, Titulo = "1984", AnioPublicacion = 1949, AutorId = 3 }
        };

        // Constructor para la inyección de dependencias de IAutorService.
        public LibroService(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IEnumerable<Models.Libro> GetAll()
        {
            // Cargamos el objeto Autor completo para cada libro si existe.
            return _libros.Select(l =>
            {
                // Solo si AutorId tiene un valor válido, intentamos cargar el autor.
                if (l.AutorId > 0)
                {
                    l.Autor = _autorService.GetById(l.AutorId);
                }
                return l;
            });
        }

        public Models.Libro? GetById(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
            if (libro != null)
            {
                // Si el libro se encuentra, carga su autor asociado si AutorId tiene un valor válido.
                if (libro.AutorId > 0)
                {
                    libro.Autor = _autorService.GetById(libro.AutorId);
                }
            }
            return libro;
        }

        public IEnumerable<Models.Libro> GetByAutorId(int autorId)
        {
            // Opcional: Validar que el autorId exista antes de buscar libros.
            if (_autorService.GetById(autorId) == null)
            {
                return Enumerable.Empty<Models.Libro>();
            }

            // Filtramos por AutorId y luego cargamos el objeto Autor para cada libro.
            return _libros.Where(l => l.AutorId == autorId).Select(l =>
            {
                if (l.AutorId > 0)
                {
                    l.Autor = _autorService.GetById(l.AutorId);
                }
                return l;
            });
        }

        public Models.Libro? Add(Models.Libro nuevoLibro)
        {
            // Validar que el AutorId del nuevo libro exista.
            if (_autorService.GetById(nuevoLibro.AutorId) == null)
            {
                return null; // Autor no existe, no se puede agregar el libro.
            }

            nuevoLibro.Id = _libros.Any() ? _libros.Max(l => l.Id) + 1 : 1;
            _libros.Add(nuevoLibro);
            nuevoLibro.Autor = _autorService.GetById(nuevoLibro.AutorId); // Cargar el autor para la respuesta
            return nuevoLibro;
        }

        public bool Update(Models.Libro libroActualizado)
        {
            var libroExistente = _libros.FirstOrDefault(l => l.Id == libroActualizado.Id);
            if (libroExistente == null)
            {
                return false; // Libro no encontrado
            }

            // Validar que el nuevo AutorId (si cambia) exista.
            if (_autorService.GetById(libroActualizado.AutorId) == null)
            {
                return false; // Autor no válido para la actualización
            }

            libroExistente.Titulo = libroActualizado.Titulo;
            libroExistente.AnioPublicacion = libroActualizado.AnioPublicacion;
            libroExistente.AutorId = libroActualizado.AutorId; // Actualizar el AutorId
            libroExistente.Autor = _autorService.GetById(libroActualizado.AutorId); // Actualizar la propiedad de navegación
            return true;
        }

        public bool Delete(int id)
        {
            var libroAEliminar = _libros.FirstOrDefault(l => l.Id == id);
            if (libroAEliminar == null)
            {
                return false; // Libro no encontrado
            }

            _libros.Remove(libroAEliminar);
            return true;
        }
    }
}