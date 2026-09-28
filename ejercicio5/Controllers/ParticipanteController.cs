using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipanteController : ControllerBase
{
    public static readonly List<Participante> participantes = new()
    {
        new Participante { Id = 1, Nombre = "Ana", Email = "ana@mail.com", Edad = 25 },
        new Participante { Id = 2, Nombre = "Bruno", Email = "bruno@mail.com", Edad = 30 },
        new Participante { Id = 3, Nombre = "Carla", Email = "carla@mail.com", Edad = 22 }
    };

    [HttpPost]
    public IActionResult crearParticipante([FromBody] Participante nuevoParticipante)
    {
        try
        {
            if (nuevoParticipante.Id <= 0 || string.IsNullOrWhiteSpace(nuevoParticipante.Nombre) || string.IsNullOrWhiteSpace(nuevoParticipante.Email) || nuevoParticipante.Edad <= 0)
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}