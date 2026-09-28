using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class ResolucionController : ControllerBase
{
    public static readonly List<Resolucion> resoluciones = new()
    {
        new Resolucion { Id = 1, ParticipanteId = 1, DesafioId = 1, PuntajeObtenido = 90, FechaEntrega = DateTime.Now },
        new Resolucion { Id = 2, ParticipanteId = 1, DesafioId = 2, PuntajeObtenido = 70, FechaEntrega = DateTime.Now },
        new Resolucion { Id = 3, ParticipanteId = 2, DesafioId = 1, PuntajeObtenido = 100, FechaEntrega = DateTime.Now },
        new Resolucion { Id = 4, ParticipanteId = 3, DesafioId = 1, PuntajeObtenido = 50, FechaEntrega = DateTime.Now }
    };

    [HttpPost]
    public IActionResult crearResolucion([FromBody] Resolucion nuevaResolucion)
    {
        try
        {
            if (!ParticipanteController.participantes.Any(x => x.Id == nuevaResolucion.ParticipanteId))
            {
                return NotFound("El participante no existe");
            }

            var desafio = DesafioController.desafios.FirstOrDefault(x => x.Id == nuevaResolucion.DesafioId);
            if (desafio is null)
            {
                return NotFound("El desafio no existe");
            }

            if (!desafio.Activo)
            {
                return BadRequest("El desafio no esta activo");
            }

            if (nuevaResolucion.Id <= 0 || nuevaResolucion.PuntajeObtenido < 0)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (nuevaResolucion.PuntajeObtenido > desafio.PuntajeMaximo)
            {
                return BadRequest("El puntaje supera el maximo del desafio");
            }

            if (resoluciones.Any(x => x.Id == nuevaResolucion.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }

            if (resoluciones.Any(x => x.ParticipanteId == nuevaResolucion.ParticipanteId && x.DesafioId == nuevaResolucion.DesafioId))
            {
                return Conflict("El participante ya resolvio ese desafio");
            }
            resoluciones.Add(nuevaResolucion);
            return Ok("Resolucion registrada con exito");
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Participante/{participanteId}")]
    public IActionResult porParticipante(int participanteId)
    {
        try
        {
            if (!ParticipanteController.participantes.Any(x => x.Id == participanteId))
            {
                return NotFound("El participante no existe");
            }

            var resultado = resoluciones.Where(x => x.ParticipanteId == participanteId).ToList();
            if (!resultado.Any())
            {
                return NotFound("El participante no tiene resoluciones");
            }
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("NoResueltos/{participanteId}")]
    public IActionResult noResueltos(int participanteId)
    {
        try
        {
            if (!ParticipanteController.participantes.Any(x => x.Id == participanteId))
            {
                return NotFound("El participante no existe");
            }

            var resultado = DesafioController.desafios.Where(x => !resoluciones.Any(n => n.ParticipanteId == participanteId && n.DesafioId == x.Id)).ToList();
            if (!resultado.Any())
            {
                return NotFound("El participante ya resolvio todos los desafios");
            }
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}
