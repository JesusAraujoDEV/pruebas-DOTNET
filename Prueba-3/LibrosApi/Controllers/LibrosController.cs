// Controllers/LibrosController.cs
// Controlador de API para gestionar los Libros.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Para los modelos
using LibrosAutoresApi.Services.Libro; // Para el servicio de Libro
using LibrosAutoresApi.Services.Autor; // Para el servicio de Autor (para validación)
using LibrosAutoresApi.Dtos.Libro; // Para los DTOs de Libro

namespace LibrosAutoresApi.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("api/[controller]")] // Define la ruta base: /api/Libros
    public class LibrosController : ControllerBase
    {
        private readonly ILibroService _libroService; // Inyección del servicio de Libro
        private readonly IAutorService _autorService; // Inyección del servicio de Autor para validación

        public LibrosController(ILibroService libroService, IAutorService autorService)
        {
            _libroService = libroService;
            _autorService = autorService;
        }

        // GET api/Libros
        // Obtiene todos los libros.
        [HttpGet]
        public ActionResult<IEnumerable<Models.Libro>> Get()
        {
            var libros = _libroService.GetAll();
            return Ok(libros);
        }

        // GET api/Libros/{id}
        // Obtiene un libro por su ID.
        [HttpGet("{id}")]
        public ActionResult<Models.Libro> Get(int id)
        {
            var libro = _libroService.GetById(id);
            if (libro == null)
            {
                return NotFound(); // HTTP 404
            }
            return Ok(libro); // HTTP 200
        }

        // POST api/Libros
        // Crea un nuevo libro.
        [HttpPost]
        public ActionResult<Models.Libro> Post([FromBody] CrearLibroDto crearLibroDto)
        {
            // Validar que el AutorId exista antes de crear el libro.
            if (_autorService.GetById(crearLibroDto.AutorId) == null)
            {
                return BadRequest($"El Autor con ID {crearLibroDto.AutorId} no existe."); // HTTP 400
            }

            var nuevoLibro = new Models.Libro
            {
                Titulo = crearLibroDto.Titulo,
                AnioPublicacion = crearLibroDto.AnioPublicacion,
                AutorId = crearLibroDto.AutorId // ¡IMPORTANTE! Asignar AutorId desde el DTO
            };

            var libroAgregado = _libroService.Add(nuevoLibro); // El servicio asigna el ID y carga el Autor

            if (libroAgregado == null) // Si el servicio devuelve null (por ejemplo, por autor no encontrado o error interno)
            {
                return StatusCode(500, "Ocurrió un error al agregar el libro."); // HTTP 500
            }

            return CreatedAtAction(nameof(Get), new { id = libroAgregado.Id }, libroAgregado); // HTTP 201
        }

        // PUT api/Libros/{id}
        // Actualiza un libro existente.
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ActualizarLibroDto actualizarLibroDto)
        {
            // Valida que el ID en la ruta coincida con el ID del objeto enviado en el cuerpo.
            if (id != actualizarLibroDto.Id)
            {
                return BadRequest("El ID del libro en la ruta no coincide con el ID en el cuerpo de la solicitud."); // HTTP 400
            }

            // Validar que el AutorId exista antes de actualizar el libro.
            if (_autorService.GetById(actualizarLibroDto.AutorId) == null)
            {
                return BadRequest($"El Autor con ID {actualizarLibroDto.AutorId} no existe."); // HTTP 400
            }

            var libroParaActualizar = new Models.Libro
            {
                Id = actualizarLibroDto.Id,
                Titulo = actualizarLibroDto.Titulo,
                AnioPublicacion = actualizarLibroDto.AnioPublicacion,
                AutorId = actualizarLibroDto.AutorId // ¡IMPORTANTE! Asignar AutorId desde el DTO
            };

            bool actualizado = _libroService.Update(libroParaActualizar);
            if (!actualizado)
            {
                return NotFound(); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }

        // DELETE api/Libros/{id}
        // Elimina un libro.
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool eliminado = _libroService.Delete(id);
            if (!eliminado)
            {
                return NotFound(); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }
    }
}