// Controllers/AutoresController.cs
// Controlador de API para gestionar los Autores.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Para los modelos
using LibrosAutoresApi.Services.Autor; // Para el servicio de Autor
using LibrosAutoresApi.Services.Libro; // Para el servicio de Libro
using LibrosAutoresApi.Dtos.Autor; // Para los DTOs de Autor
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("api/[controller]")] // Define la ruta base: /api/Autores
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService; // Inyección de la interfaz del servicio

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
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
        public async Task<ActionResult<Models.Autor>> Get(int id) // Ahora async Task
        {
            var autor = await _autorService.GetById(id); // Usamos await
            if (autor == null)
            {
                return NotFound(); // HTTP 404
            }
            return Ok(autor); // HTTP 200
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

        // PUT api/Autores/{id}
        // Actualiza un autor existente.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActualizarAutorDto actualizarAutorDto) // Ahora async Task
        {
            if (id != actualizarAutorDto.Id)
            {
                return BadRequest("El ID en la ruta no coincide con el ID en el cuerpo."); // HTTP 400
            }

            var autorParaActualizar = new Models.Autor
            {
                Id = actualizarAutorDto.Id,
                Nombre = actualizarAutorDto.Nombre,
                FechaNacimiento = actualizarAutorDto.FechaNacimiento
            };

            bool actualizado = await _autorService.Update(autorParaActualizar); // Usamos await
            if (!actualizado)
            {
                return NotFound(); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }

        // DELETE api/Autores/{id}
        // Elimina un autor.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) // Ahora async Task
        {
            bool eliminado = await _autorService.Delete(id); // Usamos await
            if (!eliminado)
            {
                return NotFound(); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }
    }
}