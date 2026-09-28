using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class ActividadController : ControllerBase
{
    public static readonly List<Actividad> actividades = new()
    {
        new Actividad { Id = 1, Nombre = "Charla de IA", Tipo = "Charla", Horario = DateTime.Parse("2026-11-10 10:00"), DuracionMinutos = 60, Capacidad = 50, Activa = true },
        new Actividad { Id = 2, Nombre = "Taller de Robotica", Tipo = "Taller", Horario = DateTime.Parse("2026-11-10 10:30"), DuracionMinutos = 120, Capacidad = 2, Activa = true },
        new Actividad { Id = 3, Nombre = "Competencia de Codigo", Tipo = "Competencia", Horario = DateTime.Parse("2026-11-10 14:00"), DuracionMinutos = 90, Capacidad = 3, Activa = true },
        new Actividad { Id = 4, Nombre = "Demo de Drones", Tipo = "Demostracion", Horario = DateTime.Parse("2026-11-11 11:00"), DuracionMinutos = 45, Capacidad = 20, Activa = false }
    };

    [HttpPost]
    public IActionResult crearActividad([FromBody] Actividad nuevaActividad)
    {
        try
        {
            if (nuevaActividad.Id <= 0 || string.IsNullOrWhiteSpace(nuevaActividad.Nombre) || string.IsNullOrWhiteSpace(nuevaActividad.Tipo) || nuevaActividad.DuracionMinutos <= 0 || nuevaActividad.Capacidad <= 0)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (nuevaActividad.Tipo != "Charla" && nuevaActividad.Tipo != "Taller" && nuevaActividad.Tipo != "Competencia" && nuevaActividad.Tipo != "Demostracion")
            {
                return BadRequest("Tipo no valido");
            }

            if (actividades.Any(x => x.Id == nuevaActividad.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }
            actividades.Add(nuevaActividad);
            return Ok("Actividad creada con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Disponibles")]
    public IActionResult disponibles()
    {
        try
        {
            var resultado = actividades.Where(x => x.Activa).ToList();
            if (!resultado.Any())
            {
                return NotFound("No hay actividades disponibles");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Nombre/{nombre}")]
    public IActionResult porNombre(string nombre)
    {
        try
        {
            var resultado = actividades.Where(x => x.Nombre.Contains(nombre)).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se encontraron actividades con ese nombre");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Tipo/{tipo}")]
    public IActionResult porTipo(string tipo)
    {
        try
        {
            var resultado = actividades.Where(x => x.Tipo == tipo).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se encontraron actividades de ese tipo");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("ConLugares")]
    public IActionResult conLugares()
    {
        try
        {
            var resultado = actividades.Where(x => ReservaActividadController.reservas.Count(n => n.ActividadId == x.Id) < x.Capacidad).ToList();
            if (!resultado.Any())
            {
                return NotFound("No hay actividades con lugares disponibles");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Completas")]
    public IActionResult completas()
    {
        try
        {
            var resultado = actividades.Where(x => ReservaActividadController.reservas.Count(n => n.ActividadId == x.Id) >= x.Capacidad).ToList();
            if (!resultado.Any())
            {
                return NotFound("No hay actividades completas");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Ordenadas")]
    public IActionResult ordenadasPorInscriptos()
    {
        try
        {
            if (!actividades.Any())
            {
                return NotFound("No hay actividades cargadas");
            }
            var totales = actividades.Select(x => new
            {
                Actividad = x.Nombre,
                Inscriptos = ReservaActividadController.reservas.Count(n => n.ActividadId == x.Id)
            }).ToList();
            var resultado = totales.OrderByDescending(x => x.Inscriptos).ToList();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("MasInscriptos")]
    public IActionResult masInscriptos()
    {
        try
        {
            if (!actividades.Any())
            {
                return NotFound("No hay actividades cargadas");
            }
            var totales = actividades.Select(x => new
            {
                Actividad = x.Nombre,
                Inscriptos = ReservaActividadController.reservas.Count(n => n.ActividadId == x.Id)
            }).ToList();
            var resultado = totales.OrderByDescending(x => x.Inscriptos).FirstOrDefault();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Ocupacion")]
    public IActionResult ocupacion()
    {
        try
        {
            if (!actividades.Any())
            {
                return NotFound("No hay actividades cargadas");
            }
            var resultado = actividades.Select(x => new
            {
                Actividad = x.Nombre,
                PorcentajeOcupacion = ReservaActividadController.reservas.Count(n => n.ActividadId == x.Id) * 100.0 / x.Capacidad
            }).ToList();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}