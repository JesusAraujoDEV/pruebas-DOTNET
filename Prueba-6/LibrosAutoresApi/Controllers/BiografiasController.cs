// Controllers/BiografiasController.cs
// Controlador de API para gestionar las Biografías.
using Microsoft.AspNetCore.Authorization; // ¡NUEVO! Para el atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Necesario para referenciar la clase Biografia (el modelo)
using LibrosAutoresApi.Services.Biografia; // Necesario para referenciar el servicio IBiografiaService
using LibrosAutoresApi.Dtos.Biografia; // Para los DTOs de Biografia
using System.Threading.Tasks; // Necesario para Task
using LibrosAutoresApi.Dtos.Biografia; // ¡NUEVO! Para ActualizarBiografiaDto

namespace LibrosAutoresApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BiografiasController : ControllerBase
    {
        private readonly IBiografiaService _biografiaService;
        private readonly ILogger<BiografiasController> _logger; // Inyectar logger

        public BiografiasController(IBiografiaService biografiaService, ILogger<BiografiasController> logger)
        {
            _biografiaService = biografiaService;
            _logger = logger;
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

        // ¡MÉTODO CAMBIADO A PATCH!
        // PATCH api/Biografias/{autorId}
        // Actualiza parcialmente una biografía existente.
        [HttpPatch("{autorId}")] // ¡CAMBIADO A HttpPatch!
        public async Task<IActionResult> Patch(int autorId, [FromBody] ActualizarBiografiaDto biografiaDto) // ¡CAMBIADO EL DTO!
        {
            _logger.LogInformation("Solicitud PATCH para biografía de AutorId: {AutorId}", autorId);

            bool actualizado = await _biografiaService.Update(autorId, biografiaDto); // Pasamos el AutorId y el DTO
            // El servicio lanza NotFoundException si no lo encuentra.
            // El middleware lo capturará y devolverá un 404.

            if (!actualizado)
            {
                return StatusCode(500, "Ocurrió un error inesperado al actualizar la biografía.");
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