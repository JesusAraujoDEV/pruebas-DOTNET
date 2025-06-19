// Controllers/EventosController.cs
// Controlador de API para gestionar los Eventos y sus relaciones Muchos-a-Muchos con Autores.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibrosApi.Models; // Necesario para referenciar las clases Evento y Autor (los modelos)
using LibrosApi.Services.Evento; // Necesario para referenciar el servicio IEventoService
using LibrosApi.Services.Autor; // Para validar existencia de Autor (servicio IAutorService)
using LibrosApi.Dtos.Evento; // Para los DTOs de Evento

namespace LibrosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Eventos
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
        public ActionResult<IEnumerable<Models.Evento>> Get() // Corregido: Models.Evento
        {
            var eventos = _eventoService.GetAll();
            return Ok(eventos);
        }

        // GET api/Eventos/{id}
        [HttpGet("{id}")]
        public ActionResult<Models.Evento> Get(int id) // Corregido: Models.Evento
        {
            var evento = _eventoService.GetById(id);
            if (evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        // GET api/Eventos/{id}/autores
        // Obtiene todos los autores asociados a un evento específico (relación M-M).
        [HttpGet("{id}/autores")]
        public ActionResult<IEnumerable<Models.Autor>> GetAutores(int id) // Corregido: Models.Autor
        {
            if (_eventoService.GetById(id) == null)
            {
                return NotFound($"Evento con ID {id} no encontrado.");
            }
            var autores = _eventoService.GetAutoresByEventoId(id);
            return Ok(autores);
        }

        // POST api/Eventos
        [HttpPost]
        public ActionResult<Models.Evento> Post([FromBody] CrearEventoDto crearEventoDto) // Corregido: Models.Evento
        {
            var nuevoEvento = new Models.Evento // Corregido: Models.Evento
            {
                Nombre = crearEventoDto.Nombre,
                Fecha = crearEventoDto.Fecha,
                Ubicacion = crearEventoDto.Ubicacion
            };

            _eventoService.Add(nuevoEvento);
            return CreatedAtAction(nameof(Get), new { id = nuevoEvento.Id }, nuevoEvento);
        }

        // PUT api/Eventos/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ActualizarEventoDto actualizarEventoDto)
        {
            if (id != actualizarEventoDto.Id)
            {
                return BadRequest("El ID en la ruta no coincide con el ID en el cuerpo.");
            }

            var eventoParaActualizar = new Models.Evento // Corregido: Models.Evento
            {
                Id = actualizarEventoDto.Id,
                Nombre = actualizarEventoDto.Nombre,
                Fecha = actualizarEventoDto.Fecha,
                Ubicacion = actualizarEventoDto.Ubicacion
            };

            bool actualizado = _eventoService.Update(eventoParaActualizar);
            if (!actualizado)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST api/Eventos/{eventoId}/autores
        // Asocia un autor existente a un evento (para la relación Muchos-a-Muchos).
        [HttpPost("{eventoId}/autores")]
        public IActionResult AsociarAutorAEvento(int eventoId, [FromBody] AsociarAutorEventoDto dto)
        {
            if (_eventoService.GetById(eventoId) == null)
            {
                return NotFound($"Evento con ID {eventoId} no encontrado.");
            }
            if (_autorService.GetById(dto.AutorId) == null)
            {
                return NotFound($"Autor con ID {dto.AutorId} no encontrado.");
            }

            bool exito = _eventoService.AddAutorToEvento(eventoId, dto.AutorId);
            if (!exito)
            {
                return Conflict($"El autor con ID {dto.AutorId} ya está asociado al evento con ID {eventoId}.");
            }

            return NoContent();
        }

        // DELETE api/Eventos/{eventoId}/autores/{autorId}
        // Desasocia un autor de un evento (para la relación Muchos-a-Muchos).
        [HttpDelete("{eventoId}/autores/{autorId}")]
        public IActionResult DesasociarAutorDeEvento(int eventoId, int autorId)
        {
            if (_eventoService.GetById(eventoId) == null)
            {
                return NotFound($"Evento con ID {eventoId} no encontrado.");
            }
            if (_autorService.GetById(autorId) == null)
            {
                return NotFound($"Autor con ID {autorId} no encontrado.");
            }

            bool exito = _eventoService.RemoveAutorFromEvento(eventoId, autorId);
            if (!exito)
            {
                return NotFound($"La asociación entre el evento {eventoId} y el autor {autorId} no fue encontrada.");
            }

            return NoContent();
        }
    }
}