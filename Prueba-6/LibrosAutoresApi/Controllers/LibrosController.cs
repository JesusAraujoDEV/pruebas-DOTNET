// Controllers/LibrosController.cs
// Controlador de API para gestionar los Libros.
using Microsoft.AspNetCore.Authorization; // ¡NUEVO! Para el atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Para los modelos
using LibrosAutoresApi.Services.Libro; // Para el servicio de Libro
using LibrosAutoresApi.Services.Autor; // Para el servicio de Autor (para validación)
using LibrosAutoresApi.Dtos.Libro; // Para los DTOs de Libro
using System.Threading.Tasks; // Necesario para Task (siempre asegúrate de tenerlo)
using LibrosAutoresApi.Dtos.Libro; // ¡NUEVO! Para ActualizarLibroDto

namespace LibrosAutoresApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibrosController : ControllerBase
    {
        private readonly ILibroService _libroService;
        private readonly IAutorService _autorService;
        private readonly ILogger<LibrosController> _logger; // Inyectar logger

        public LibrosController(ILibroService libroService, IAutorService autorService, ILogger<LibrosController> logger)
        {
            _libroService = libroService;
            _autorService = autorService;
            _logger = logger;
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

        // ¡MÉTODO CAMBIADO A PATCH!
        // PATCH api/Libros/{id}
        // Actualiza parcialmente un libro existente.
        [HttpPatch("{id}")] // ¡CAMBIADO A HttpPatch!
        public async Task<IActionResult> Patch(int id, [FromBody] ActualizarLibroDto libroDto) // ¡CAMBIADO EL DTO!
        {
            _logger.LogInformation("Solicitud PATCH para libro con ID: {LibroId}", id);
            // Validar que el AutorId exista si se proporciona en el DTO
            if (libroDto.AutorId.HasValue)
            {
                try
                {
                    await _autorService.GetById(libroDto.AutorId.Value); // Solo verifica existencia, el servicio lanzará NotFoundException
                }
                catch (NotFoundException)
                {
                    return BadRequest($"El Autor con ID {libroDto.AutorId.Value} no existe.");
                }
            }

            bool actualizado = await _libroService.Update(id, libroDto); // Pasamos el ID y el DTO
            // El servicio lanza NotFoundException si no lo encuentra.
            // El middleware lo capturará y devolverá un 404.

            // Si el servicio devuelve false por alguna otra razón no cubierta por NotFoundException
            if (!actualizado)
            {
                return StatusCode(500, "Ocurrió un error inesperado al actualizar el libro.");
            }
            return NoContent(); // HTTP 204
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