using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class ReservaActividadController : ControllerBase
{
    public static readonly List<ReservaActividad> reservas = new()
    {
        new ReservaActividad { Id = 1, ActividadId = 1, ParticipanteId = 1, FechaReserva = DateTime.Now },
        new ReservaActividad { Id = 2, ActividadId = 2, ParticipanteId = 2, FechaReserva = DateTime.Now },
        new ReservaActividad { Id = 3, ActividadId = 2, ParticipanteId = 3, FechaReserva = DateTime.Now },
        new ReservaActividad { Id = 4, ActividadId = 3, ParticipanteId = 1, FechaReserva = DateTime.Now }
    };

    [HttpPost]
    public IActionResult inscribirParticipante([FromBody] ReservaActividad nuevaReserva)
    {
        try
        {
            var actividad = ActividadController.actividades.FirstOrDefault(x => x.Id == nuevaReserva.ActividadId);
            if (actividad is null)
            {
                return NotFound("La actividad no existe");
            }

            if (!actividad.Activa)
            {
                return BadRequest("La actividad no esta activa");
            }

            if (!ParticipanteController.participantes.Any(x => x.Id == nuevaReserva.ParticipanteId))
            {
                return NotFound("El participante no existe");
            }

            if (nuevaReserva.Id <= 0)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (reservas.Any(x => x.Id == nuevaReserva.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }

            if (reservas.Any(x => x.ActividadId == nuevaReserva.ActividadId && x.ParticipanteId == nuevaReserva.ParticipanteId))
            {
                return Conflict("El participante ya esta inscripto en esa actividad");
            }

            var inscriptos = reservas.Count(x => x.ActividadId == nuevaReserva.ActividadId);
            if (inscriptos >= actividad.Capacidad)
            {
                return BadRequest("La actividad no tiene lugares disponibles");
            }

            var inicio = actividad.Horario;
            var fin = actividad.Horario.AddMinutes(actividad.DuracionMinutos);
            var reservasDelParticipante = reservas.Where(x => x.ParticipanteId == nuevaReserva.ParticipanteId).ToList();
            var actividadesDelParticipante = ActividadController.actividades.Where(x => reservasDelParticipante.Any(n => n.ActividadId == x.Id)).ToList();
            if (actividadesDelParticipante.Any(x => x.Horario < fin && x.Horario.AddMinutes(x.DuracionMinutos) > inicio))
            {
                return Conflict("El participante ya tiene una actividad en ese horario");
            }
            reservas.Add(nuevaReserva);
            return Ok("Inscripcion registrada con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult cancelarInscripcion(int id)
    {
        try
        {
            var reserva = reservas.FirstOrDefault(x => x.Id == id);
            if (reserva is null)
            {
                return NotFound("La inscripcion no existe");
            }
            reservas.Remove(reserva);
            return Ok("Inscripcion cancelada con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Actividad/{actividadId}")]
    public IActionResult participantesDeActividad(int actividadId)
    {
        try
        {
            if (!ActividadController.actividades.Any(x => x.Id == actividadId))
            {
                return NotFound("La actividad no existe");
            }

            var reservasDeActividad = reservas.Where(x => x.ActividadId == actividadId).ToList();
            var resultado = ParticipanteController.participantes.Where(x => reservasDeActividad.Any(n => n.ParticipanteId == x.Id)).ToList();
            if (!resultado.Any())
            {
                return NotFound("La actividad no tiene participantes inscriptos");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Participante/{participanteId}")]
    public IActionResult actividadesDeParticipante(int participanteId)
    {
        try
        {
            if (!ParticipanteController.participantes.Any(x => x.Id == participanteId))
            {
                return NotFound("El participante no existe");
            }

            var reservasDelParticipante = reservas.Where(x => x.ParticipanteId == participanteId).ToList();
            var resultado = ActividadController.actividades.Where(x => reservasDelParticipante.Any(n => n.ActividadId == x.Id)).ToList();
            if (!resultado.Any())
            {
                return NotFound("El participante no esta inscripto en ninguna actividad");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}
