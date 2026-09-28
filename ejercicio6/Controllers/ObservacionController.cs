using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("api/observaciones")]
public class ObservacionController : ControllerBase
{
    public static readonly List<Observacion> observaciones = new()
    {
        new Observacion { Id = 1, ObjetoEspacialId = 1, Fecha = DateTime.Now.AddDays(-3), DistanciaMedida = 805, Velocidad = 30.5, Comentario = "Trayectoria estable" },
        new Observacion { Id = 2, ObjetoEspacialId = 1, Fecha = DateTime.Now.AddDays(-2), DistanciaMedida = 802, Velocidad = 30.7, Comentario = "Sin cambios" },
        new Observacion { Id = 3, ObjetoEspacialId = 2, Fecha = DateTime.Now.AddDays(-5), DistanciaMedida = 3010, Velocidad = 54.2, Comentario = "Cola visible" },
        new Observacion { Id = 4, ObjetoEspacialId = 3, Fecha = DateTime.Now.AddDays(-1), DistanciaMedida = 400, Velocidad = 7.6, Comentario = "Orbita normal" },
        new Observacion { Id = 5, ObjetoEspacialId = 5, Fecha = DateTime.Now.AddDays(-4), DistanciaMedida = 1500, Velocidad = 28.1, Comentario = "Monitoreo continuo" }
    };

    [HttpPost]
    public IActionResult crearObservacion([FromBody] Observacion nuevaObservacion)
    {
        if (!ObjetoEspacialController.objetos.Any(x => x.Id == nuevaObservacion.ObjetoEspacialId))
        {
            return NotFound("El objeto espacial no existe");
        }

        if (nuevaObservacion.Id <= 0 || nuevaObservacion.DistanciaMedida <= 0 || nuevaObservacion.Velocidad < 0)
        {
            return BadRequest("Parametros nulos o no validos");
        }

        if (nuevaObservacion.Fecha > DateTime.Now)
        {
            return BadRequest("Fecha de observacion no valida");
        }

        if (observaciones.Any(x => x.Id == nuevaObservacion.Id))
        {
            return Conflict("Ese ID ya esta registrado.");
        }
        observaciones.Add(nuevaObservacion);
        return Ok("Observacion registrada con exito");
    }

    [HttpGet("Objeto/{objetoId}")]
    public IActionResult porObjeto(int objetoId)
    {
        if (!ObjetoEspacialController.objetos.Any(x => x.Id == objetoId))
        {
            return NotFound("El objeto espacial no existe");
        }

        var resultado = observaciones.Where(x => x.ObjetoEspacialId == objetoId).ToList();
        if (!resultado.Any())
        {
            return NotFound("El objeto no tiene observaciones");
        }
        return Ok(resultado);
    }

    [HttpGet("PromedioVelocidad")]
    public IActionResult promedioVelocidad()
    {
        if (!observaciones.Any())
        {
            return NotFound("No hay observaciones cargadas");
        }
        var resultado = observaciones.Average(x => x.Velocidad);
        return Ok(resultado);
    }
}
