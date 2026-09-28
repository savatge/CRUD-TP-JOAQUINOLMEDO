// ParticipanteController.cs
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipanteController : ControllerBase
{
    public static readonly List<Participante> participantes = new()
    {
        new Participante { Id = 1, Nombre = "Ana", Email = "ana111@mail.com", Nivel = "Avanzado" },
        new Participante { Id = 2, Nombre = "Bruno", Email = "bruno8999@mail.com", Nivel = "Intermedio" },
        new Participante { Id = 3, Nombre = "Carla", Email = "carla3434@mail.com", Nivel = "Principiante" }
    };

    [HttpPost]
    public IActionResult crearParticipante([FromBody] Participante nuevoParticipante)
    {
        try
        {
            if (nuevoParticipante.Id <= 0 || string.IsNullOrWhiteSpace(nuevoParticipante.Nombre) || string.IsNullOrWhiteSpace(nuevoParticipante.Email) || string.IsNullOrWhiteSpace(nuevoParticipante.Nivel))
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (participantes.Any(x => x.Id == nuevoParticipante.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }

            if (participantes.Any(x => x.Email == nuevoParticipante.Email))
            {
                return Conflict("Ese email ya esta registrado.");
            }
            participantes.Add(nuevoParticipante);
            return Ok("Participante registrado con exito");
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Puntaje/{puntaje}")]
    public IActionResult superaronPuntaje(int puntaje)
    {
        try
        {
            var totales = participantes.Select(x => new
            {
                Participante = x.Nombre,
                PuntajeTotal = ResolucionController.resoluciones.Where(n => n.ParticipanteId == x.Id).Sum(n => n.PuntajeObtenido)
            }).ToList();
            var resultado = totales.Where(x => x.PuntajeTotal > puntaje).ToList();
            if (!resultado.Any())
            {
                return NotFound("Ningun participante supero ese puntaje");
            }
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Ranking")]
    public IActionResult ranking()
    {
        try
        {
            if (!participantes.Any())
            {
                return NotFound("No hay participantes cargados");
            }
            var totales = participantes.Select(x => new
            {
                Participante = x.Nombre,
                PuntajeTotal = ResolucionController.resoluciones.Where(n => n.ParticipanteId == x.Id).Sum(n => n.PuntajeObtenido)
            }).ToList();
            var resultado = totales.OrderByDescending(x => x.PuntajeTotal).ToList();
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("MayorPuntaje")]
    public IActionResult mayorPuntaje()
    {
        try
        {
            if (!participantes.Any())
            {
                return NotFound("No hay participantes cargados");
            }
            var totales = participantes.Select(x => new
            {
                Participante = x.Nombre,
                PuntajeTotal = ResolucionController.resoluciones.Where(n => n.ParticipanteId == x.Id).Sum(n => n.PuntajeObtenido)
            }).ToList();
            var resultado = totales.OrderByDescending(x => x.PuntajeTotal).FirstOrDefault();
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}
