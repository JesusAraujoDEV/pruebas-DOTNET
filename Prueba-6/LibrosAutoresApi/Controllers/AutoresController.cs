// Controllers/AutoresController.cs
// Controlador de API para gestionar los Autores.
using Microsoft.AspNetCore.Authorization; // ¡NUEVO! Para el atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Para los modelos
using LibrosAutoresApi.Services.Autor; // Para el servicio de Autor
using LibrosAutoresApi.Services.Libro; // Para el servicio de Libro
using LibrosAutoresApi.Dtos.Autor; // Para los DTOs de Autor
using System.Threading.Tasks; // Necesario para Task

using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using Microsoft.Extensions.Logging; // Para inyectar ILogger

using LibrosAutoresApi.Dtos.Autor; // ¡NUEVO! Para ActualizarAutorDto

namespace LibrosAutoresApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(IAutorService autorService, ILogger<AutoresController> logger)
        {
            _autorService = autorService;
            _logger = logger;
        }

        // GET api/Autores
        // Obtiene todos los autores.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Autor>>> Get() // Ahora async Task
        {
            var autores = await _autorService.GetAll(); // Usamos await
            return Ok(autores);
        }

        // GET api/Autores/{id}
        // Obtiene un autor por su ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Autor>> Get(int id)
        {
            _logger.LogInformation("Solicitud GET para autor con ID: {AutorId}", id);
            // El servicio lanzará NotFoundException si no lo encuentra.
            // El middleware lo capturará y devolverá un 404.
            var autor = await _autorService.GetById(id);
            return Ok(autor); // Si llega aquí, el autor fue encontrado.
        }

        // GET api/Autores/{id}/libros
        // Obtiene todos los libros escritos por un autor específico.
        [HttpGet("{id}/libros")]
        public async Task<ActionResult<IEnumerable<Models.Libro>>> GetLibrosPorAutor(int id, [FromServices] ILibroService libroService) // Ahora async Task
        {
            // Validar si el autor existe
            if (await _autorService.GetById(id) == null) // Usamos await
            {
                return NotFound($"No se encontró el autor con ID {id}.");
            }
            var libros = await libroService.GetByAutorId(id); // ¡AQUÍ: DEBE LLEVAR await! (Línea 57 o similar)
            return Ok(libros);
        }

        // POST api/Autores
        // Crea un nuevo autor.
        [HttpPost]
        public async Task<ActionResult<Models.Autor>> Post([FromBody] CrearAutorDto crearAutorDto) // Ahora async Task
        {
            var nuevoAutor = new Models.Autor
            {
                Nombre = crearAutorDto.Nombre,
                FechaNacimiento = crearAutorDto.FechaNacimiento
            };

            await _autorService.Add(nuevoAutor); // Usamos await
            return CreatedAtAction(nameof(Get), new { id = nuevoAutor.Id }, nuevoAutor); // HTTP 201
        }

        // ¡MÉTODO CAMBIADO A PATCH!
        // PATCH api/Autores/{id}
        // Actualiza parcialmente un autor existente.
        [HttpPatch("{id}")] // ¡CAMBIADO A HttpPatch!
        public async Task<IActionResult> Patch(int id, [FromBody] ActualizarAutorDto autorDto) // ¡CAMBIADO EL DTO!
        {
            _logger.LogInformation("Solicitud PATCH para autor con ID: {AutorId}", id);
            // Ya no necesitamos validar id != autorDto.Id porque PATCH se enfoca en el ID de la URI
            // y el DTO de PATCH no contiene un ID propio.

            bool actualizado = await _autorService.Update(id, autorDto); // Pasamos el ID y el DTO
            // El servicio lanza NotFoundException si no lo encuentra.
            // El middleware lo capturará y devolverá un 404.

            // Si el servicio devuelve false por alguna otra razón no cubierta por NotFoundException,
            // (aunque con la nueva lógica de excepciones, es menos probable que pase por aquí).
            if (!actualizado)
            {
                return StatusCode(500, "Ocurrió un error inesperado al actualizar el autor.");
            }
            return NoContent(); // HTTP 204
        }

        // DELETE api/Autores/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Solicitud DELETE para autor con ID: {AutorId}", id);
            // El servicio lanzará NotFoundException si no lo encuentra.
            // El middleware lo capturará y devolverá un 404.
            bool eliminado = await _autorService.Delete(id);
            // Si el método Delete del servicio devuelve true, significa que se eliminó con éxito.
            return NoContent();
        }
    }
}