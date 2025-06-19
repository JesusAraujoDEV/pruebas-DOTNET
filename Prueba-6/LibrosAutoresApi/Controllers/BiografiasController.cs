// Controllers/BiografiasController.cs
// Controlador de API para gestionar las Biografías.
using Microsoft.AspNetCore.Authorization; // ¡NUEVO! Para el atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Necesario para referenciar la clase Biografia (el modelo)
using LibrosAutoresApi.Services.Biografia; // Necesario para referenciar el servicio IBiografiaService
using LibrosAutoresApi.Dtos.Biografia; // Para los DTOs de Biografia
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Biografias
    [Authorize] // ¡AHORA REQUIERE AUTENTICACIÓN JWT PARA ACCEDER A CUALQUIER MÉTODO AQUÍ!
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
        public async Task<ActionResult<Models.Biografia>> Get(int autorId) // Ahora async Task
        {
            var biografia = await _biografiaService.GetByAutorId(autorId); // Usamos await
            if (biografia == null)
            {
                return NotFound($"Biografía no encontrada para el autor con ID {autorId}."); // HTTP 404
            }
            return Ok(biografia); // HTTP 200
        }

        // POST api/Biografias
        // Crea una nueva biografía para un autor.
        [HttpPost]
        public async Task<ActionResult<Models.Biografia>> Post([FromBody] CrearBiografiaDto crearBiografiaDto) // Ahora async Task
        {
            var nuevaBiografia = new Models.Biografia
            {
                AutorId = crearBiografiaDto.AutorId,
                Contenido = crearBiografiaDto.Contenido
            };

            var biografiaAgregada = await _biografiaService.Add(nuevaBiografia); // Usamos await

            if (biografiaAgregada == null)
            {
                return BadRequest("No se pudo crear la biografía. Verifique si el autor existe o si ya tiene una biografía."); // HTTP 400
            }

            return CreatedAtAction(nameof(Get), new { autorId = biografiaAgregada.AutorId }, biografiaAgregada); // HTTP 201
        }

        // PUT api/Biografias/{autorId}
        // Actualiza una biografía existente.
        [HttpPut("{autorId}")]
        public async Task<IActionResult> Put(int autorId, [FromBody] ActualizarBiografiaDto actualizarBiografiaDto) // Ahora async Task
        {
            if (autorId != actualizarBiografiaDto.AutorId)
            {
                return BadRequest("El ID del autor en la ruta no coincide con el ID en el cuerpo."); // HTTP 400
            }

            var biografiaParaActualizar = new Models.Biografia
            {
                AutorId = actualizarBiografiaDto.AutorId,
                Contenido = actualizarBiografiaDto.Contenido
            };

            bool actualizado = await _biografiaService.Update(biografiaParaActualizar); // Usamos await
            if (!actualizado)
            {
                return NotFound($"Biografía no encontrada para el autor con ID {autorId}."); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }

        // DELETE api/Biografias/{autorId}
        // Elimina la biografía de un autor.
        [HttpDelete("{autorId}")]
        public async Task<IActionResult> Delete(int autorId) // Ahora async Task
        {
            bool eliminado = await _biografiaService.Delete(autorId); // Usamos await
            if (!eliminado)
            {
                return NotFound($"Biografía no encontrada para el autor con ID {autorId}."); // HTTP 404
            }
            return NoContent(); // HTTP 204
        }
    }
}