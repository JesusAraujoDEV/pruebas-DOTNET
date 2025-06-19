// Controllers/BiografiasController.cs
// Controlador de API para gestionar las Biografías.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Aunque no se usa directamente aquí, es buena práctica si se necesitara
using LibrosApi.Models; // Necesario para referenciar la clase Biografia (el modelo)
using LibrosApi.Services.Biografia; // Necesario para referenciar el servicio IBiografiaService
using LibrosApi.Dtos.Biografia; // Para los DTOs de Biografia

namespace LibrosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Biografias
    public class BiografiasController : ControllerBase
    {
        private readonly IBiografiaService _biografiaService;

        public BiografiasController(IBiografiaService biografiaService)
        {
            _biografiaService = biografiaService;
        }

        // GET api/Biografias/{autorId}
        // Obtiene la biografía de un autor específico.
        [HttpGet("{autorId}")]
        public ActionResult<Models.Biografia> Get(int autorId) // Corregido: Models.Biografia
        {
            var biografia = _biografiaService.GetByAutorId(autorId);
            if (biografia == null)
            {
                return NotFound($"Biografía no encontrada para el autor con ID {autorId}."); // HTTP 404
            }
            return Ok(biografia); // HTTP 200
        }

        // POST api/Biografias
        // Crea una nueva biografía para un autor.
        [HttpPost]
        public ActionResult<Models.Biografia> Post([FromBody] CrearBiografiaDto crearBiografiaDto) // Corregido: Models.Biografia
        {
            var nuevaBiografia = new Models.Biografia // Corregido: Models.Biografia
            {
                AutorId = crearBiografiaDto.AutorId,
                Contenido = crearBiografiaDto.Contenido
            };

            var biografiaAgregada = _biografiaService.Add(nuevaBiografia);

            if (biografiaAgregada == null)
            {
                return BadRequest("No se pudo crear la biografía. Verifique si el autor existe o si ya tiene una biografía."); // HTTP 400
            }

            return CreatedAtAction(nameof(Get), new { autorId = biografiaAgregada.AutorId }, biografiaAgregada); // HTTP 201
        }

        // PUT api/Biografias/{autorId}
        // Actualiza una biografía existente.
        [HttpPut("{autorId}")]
        public IActionResult Put(int autorId, [FromBody] ActualizarBiografiaDto actualizarBiografiaDto)
        {
            if (autorId != actualizarBiografiaDto.AutorId)
            {
                return BadRequest("El ID del autor en la ruta no coincide con el ID en el cuerpo."); // HTTP 400
            }

            var biografiaParaActualizar = new Models.Biografia // Corregido: Models.Biografia
            {
                AutorId = actualizarBiografiaDto.AutorId,
                Contenido = actualizarBiografiaDto.Contenido
            };

            bool actualizado = _biografiaService.Update(biografiaParaActualizar);
            if (!actualizado)
            {
                return NotFound($"Biografía no encontrada para el autor con ID {autorId}."); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }

        // DELETE api/Biografias/{autorId}
        // Elimina la biografía de un autor.
        [HttpDelete("{autorId}")]
        public IActionResult Delete(int autorId)
        {
            bool eliminado = _biografiaService.Delete(autorId);
            if (!eliminado)
            {
                return NotFound($"Biografía no encontrada para el autor con ID {autorId}."); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }
    }
}