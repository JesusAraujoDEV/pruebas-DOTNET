// Controllers/AutoresController.cs
// Controlador de API para gestionar los Autores.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosApi.Models; // Para los modelos
using LibrosApi.Services.Autor; // Para el servicio de Autor
using LibrosApi.Services.Libro; // <-- ¡NUEVO! Para el servicio de Libro
using LibrosApi.Dtos.Autor; // Para los DTOs de Autor

namespace LibrosApi.Controllers
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
        public ActionResult<IEnumerable<Models.Autor>> Get()
        {
            var autores = _autorService.GetAll();
            return Ok(autores);
        }

        // GET api/Autores/{id}
        // Obtiene un autor por su ID.
        [HttpGet("{id}")]
        public ActionResult<Models.Autor> Get(int id)
        {
            var autor = _autorService.GetById(id);
            if (autor == null)
            {
                return NotFound(); // HTTP 404
            }
            return Ok(autor); // HTTP 200
        }

        // GET api/Autores/{id}/libros
        // Obtiene todos los libros escritos por un autor específico.
        // Se inyecta ILibroService directamente en el método para esta acción específica.
        [HttpGet("{id}/libros")]
        public ActionResult<IEnumerable<Models.Libro>> GetLibrosPorAutor(int id, [FromServices] ILibroService libroService) // Corregido
        {
            // Validar si el autor existe
            if (_autorService.GetById(id) == null)
            {
                return NotFound($"No se encontró el autor con ID {id}.");
            }
            var libros = libroService.GetByAutorId(id);
            return Ok(libros);
        }


        // POST api/Autores
        // Crea un nuevo autor.
        [HttpPost]
        public ActionResult<Models.Autor> Post([FromBody] CrearAutorDto crearAutorDto)
        {
            var nuevoAutor = new Models.Autor
            {
                Nombre = crearAutorDto.Nombre,
                FechaNacimiento = crearAutorDto.FechaNacimiento
            };

            _autorService.Add(nuevoAutor);
            return CreatedAtAction(nameof(Get), new { id = nuevoAutor.Id }, nuevoAutor); // HTTP 201
        }

        // PUT api/Autores/{id}
        // Actualiza un autor existente.
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ActualizarAutorDto actualizarAutorDto)
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

            bool actualizado = _autorService.Update(autorParaActualizar);
            if (!actualizado)
            {
                return NotFound(); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }

        // DELETE api/Autores/{id}
        // Elimina un autor.
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool eliminado = _autorService.Delete(id);
            if (!eliminado)
            {
                return NotFound(); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }
    }
}