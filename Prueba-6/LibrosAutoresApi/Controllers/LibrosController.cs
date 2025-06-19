// Controllers/LibrosController.cs
// Controlador de API para gestionar los Libros.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Para los modelos
using LibrosAutoresApi.Services.Libro; // Para el servicio de Libro
using LibrosAutoresApi.Services.Autor; // Para el servicio de Autor (para validación)
using LibrosAutoresApi.Dtos.Libro; // Para los DTOs de Libro
using System.Threading.Tasks; // Necesario para Task (siempre asegúrate de tenerlo)

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
        public async Task<ActionResult<IEnumerable<Models.Libro>>> Get() // Debe ser async Task
        {
            var libros = await _libroService.GetAll(); // ¡AQUÍ: DEBE LLEVAR await!
            return Ok(libros);
        }

        // GET api/Libros/{id}
        // Obtiene un libro por su ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Libro>> Get(int id) // Debe ser async Task
        {
            var libro = await _libroService.GetById(id); // ¡AQUÍ: DEBE LLEVAR await!
            if (libro == null)
            {
                return NotFound(); // HTTP 404
            }
            return Ok(libro); // HTTP 200
        }

        // POST api/Libros
        // Crea un nuevo libro.
        [HttpPost]
        public async Task<ActionResult<Models.Libro>> Post([FromBody] CrearLibroDto crearLibroDto) // Debe ser async Task
        {
            // Validar que el AutorId exista antes de crear el libro.
            if (await _autorService.GetById(crearLibroDto.AutorId) == null) // ¡AQUÍ: DEBE LLEVAR await!
            {
                return BadRequest($"El Autor con ID {crearLibroDto.AutorId} no existe."); // HTTP 400
            }

            var nuevoLibro = new Models.Libro
            {
                Titulo = crearLibroDto.Titulo,
                AnioPublicacion = crearLibroDto.AnioPublicacion,
                AutorId = crearLibroDto.AutorId
            };

            var libroAgregado = await _libroService.Add(nuevoLibro); // ¡AQUÍ: DEBE LLEVAR await!

            if (libroAgregado == null)
            {
                return StatusCode(500, "Ocurrió un error al agregar el libro.");
            }

            return CreatedAtAction(nameof(Get), new { id = libroAgregado.Id }, libroAgregado); // HTTP 201
        }

        // PUT api/Libros/{id}
        // Actualiza un libro existente.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActualizarLibroDto actualizarLibroDto) // Debe ser async Task
        {
            if (id != actualizarLibroDto.Id)
            {
                return BadRequest("El ID del libro en la ruta no coincide con el ID en el cuerpo de la solicitud.");
            }

            // Validar que el AutorId exista antes de actualizar el libro.
            if (await _autorService.GetById(actualizarLibroDto.AutorId) == null) // ¡AQUÍ: DEBE LLEVAR await!
            {
                return BadRequest($"El Autor con ID {actualizarLibroDto.AutorId} no existe.");
            }

            var libroParaActualizar = new Models.Libro
            {
                Id = actualizarLibroDto.Id,
                Titulo = actualizarLibroDto.Titulo,
                AnioPublicacion = actualizarLibroDto.AnioPublicacion,
                AutorId = actualizarLibroDto.AutorId
            };

            bool actualizado = await _libroService.Update(libroParaActualizar); // ¡AQUÍ: DEBE LLEVAR await!
            if (!actualizado)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE api/Libros/{id}
        // Elimina un libro.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) // Debe ser async Task
        {
            bool eliminado = await _libroService.Delete(id); // ¡AQUÍ: DEBE LLEVAR await!
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}