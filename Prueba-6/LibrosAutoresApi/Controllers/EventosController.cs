// Controllers/EventosController.cs
// Controlador de API para gestionar los Eventos y sus relaciones Muchos-a-Muchos con Autores.
using Microsoft.AspNetCore.Authorization; // ¡NUEVO! Para el atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosAutoresApi.Models; // Necesario para referenciar las clases Evento y Autor (los modelos)
using LibrosAutoresApi.Services.Evento; // Necesario para referenciar el servicio IEventoService
using LibrosAutoresApi.Services.Autor; // Para validar existencia de Autor (servicio IAutorService)
using LibrosAutoresApi.Dtos.Evento; // Para los DTOs de Evento
using System.Threading.Tasks; // Necesario para Task

namespace LibrosAutoresApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Eventos
    [Authorize] // ¡AHORA REQUIERE AUTENTICACIÓN JWT PARA ACCEDER A CUALQUIER MÉTODO AQUÍ!
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly IAutorService _autorService;

        public EventosController(IEventoService eventoService, IAutorService autorService)
        {
            _eventoService = eventoService;
            _autorService = autorService;
        }

        // GET api/Eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Evento>>> Get() // Ahora async Task
        {
            var eventos = await _eventoService.GetAll(); // Usamos await
            return Ok(eventos);
        }

        // GET api/Eventos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Evento>> Get(int id) // Ahora async Task
        {
            var evento = await _eventoService.GetById(id); // Usamos await
            if (evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        // GET api/Eventos/{id}/autores
        // Obtiene todos los autores asociados a un evento específico (relación M-M).
        [HttpGet("{id}/autores")]
        public async Task<ActionResult<IEnumerable<Models.Autor>>> GetAutores(int id) // Ahora async Task
        {
            if (await _eventoService.GetById(id) == null) // Usamos await
            {
                return NotFound($"Evento con ID {id} no encontrado.");
            }
            var autores = await _eventoService.GetAutoresByEventoId(id); // Usamos await
            return Ok(autores);
        }

        // POST api/Eventos
        [HttpPost]
        public async Task<ActionResult<Models.Evento>> Post([FromBody] CrearEventoDto crearEventoDto) // Ahora async Task
        {
            var nuevoEvento = new Models.Evento
            {
                Nombre = crearEventoDto.Nombre,
                Fecha = crearEventoDto.Fecha,
                Ubicacion = crearEventoDto.Ubicacion
            };

            await _eventoService.Add(nuevoEvento); // Usamos await
            return CreatedAtAction(nameof(Get), new { id = nuevoEvento.Id }, nuevoEvento);
        }

        // PUT api/Eventos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActualizarEventoDto actualizarEventoDto) // Ahora async Task
        {
            if (id != actualizarEventoDto.Id)
            {
                return BadRequest("El ID en la ruta no coincide con el ID en el cuerpo.");
            }

            var eventoParaActualizar = new Models.Evento
            {
                Id = actualizarEventoDto.Id,
                Nombre = actualizarEventoDto.Nombre,
                Fecha = actualizarEventoDto.Fecha,
                Ubicacion = actualizarEventoDto.Ubicacion
            };

            bool actualizado = await _eventoService.Update(eventoParaActualizar); // Usamos await
            if (!actualizado)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST api/Eventos/{eventoId}/autores
        // Asocia un autor existente a un evento (para la relación Muchos-a-Muchos).
        [HttpPost("{eventoId}/autores")]
        public async Task<IActionResult> AsociarAutorAEvento(int eventoId, [FromBody] AsociarAutorEventoDto dto) // Ahora async Task
        {
            // Validar que el evento y el autor existan
            if (await _eventoService.GetById(eventoId) == null) // Usamos await
            {
                return NotFound($"Evento con ID {eventoId} no encontrado.");
            }
            if (await _autorService.GetById(dto.AutorId) == null) // Usamos await
            {
                return NotFound($"Autor con ID {dto.AutorId} no encontrado.");
            }

            // Intentar agregar la relación
            bool exito = await _eventoService.AddAutorToEvento(eventoId, dto.AutorId); // Usamos await
            if (!exito)
            {
                return Conflict($"El autor con ID {dto.AutorId} ya está asociado al evento con ID {eventoId}.");
            }

            return NoContent();
        }

        // DELETE api/Eventos/{eventoId}/autores/{autorId}
        // Desasocia un autor de un evento (para la relación Muchos-a-Muchos).
        [HttpDelete("{eventoId}/autores/{autorId}")]
        public async Task<IActionResult> DesasociarAutorDeEvento(int eventoId, int autorId) // Ahora async Task
        {
            // Validar que el evento y el autor existan (opcional, el servicio ya lo hará)
            if (await _eventoService.GetById(eventoId) == null) // Usamos await
            {
                return NotFound($"Evento con ID {eventoId} no encontrado.");
            }
            if (await _autorService.GetById(autorId) == null) // Usamos await
            {
                return NotFound($"Autor con ID {autorId} no encontrado.");
            }

            bool exito = await _eventoService.RemoveAutorFromEvento(eventoId, autorId); // Usamos await
            if (!exito)
            {
                return NotFound($"La asociación entre el evento {eventoId} y el autor {autorId} no fue encontrada.");
            }

            return NoContent();
        }
    }
}