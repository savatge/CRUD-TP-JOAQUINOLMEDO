using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class MisionController : ControllerBase
{
    public static readonly List<Mision> misiones = new()
    {
        new Mision { Id = 1, DroneId = 3, Descripcion = "Inspeccion de torre norte", DistanciaKm = 12.5, Fecha = DateTime.Now.AddDays(1), Completada = false },
        new Mision { Id = 2, DroneId = 1, Descripcion = "Inspeccion de techo", DistanciaKm = 5.0, Fecha = DateTime.Now.AddDays(-2), Completada = true },
        new Mision { Id = 3, DroneId = 1, Descripcion = "Inspeccion de puente", DistanciaKm = 8.3, Fecha = DateTime.Now.AddDays(-1), Completada = true },
        new Mision { Id = 4, DroneId = 2, Descripcion = "Inspeccion de campo", DistanciaKm = 3.2, Fecha = DateTime.Now.AddDays(-3), Completada = true }
    };

    [HttpPost]
    public IActionResult asignarMision([FromBody] Mision nuevaMision)
    {
        try
        {
            var drone = DroneController.drones.FirstOrDefault(x => x.Id == nuevaMision.DroneId);
            if (drone is null)
            {
                return NotFound("El drone no existe");
            }

            if (drone.Estado != "Disponible")
            {
                return BadRequest("El drone no esta disponible");
            }

            if (drone.Bateria < 30)
            {
                return BadRequest("El drone no tiene bateria suficiente");
            }

            if (misiones.Any(x => x.DroneId == nuevaMision.DroneId && !x.Completada))
            {
                return Conflict("El drone ya tiene una mision activa");
            }

            if (nuevaMision.Id <= 0 || string.IsNullOrWhiteSpace(nuevaMision.Descripcion) || nuevaMision.DistanciaKm <= 0)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (misiones.Any(x => x.Id == nuevaMision.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }
            nuevaMision.Completada = false;
            drone.Estado = "EnMision";
            misiones.Add(nuevaMision);
            return Ok("Mision asignada con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpPut("Finalizar/{id}")]
    public IActionResult finalizarMision(int id)
    {
        try
        {
            var mision = misiones.FirstOrDefault(x => x.Id == id);
            if (mision is null)
            {
                return NotFound("La mision no existe");
            }

            if (mision.Completada)
            {
                return BadRequest("La mision ya esta finalizada");
            }

            var drone = DroneController.drones.FirstOrDefault(x => x.Id == mision.DroneId);
            if (drone is null)
            {
                return NotFound("El drone no existe");
            }
            mision.Completada = true;
            drone.Estado = "Disponible";
            return Ok("Mision finalizada con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            if (!misiones.Any())
            {
                return NotFound("No hay misiones cargadas");
            }
            return Ok(misiones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        try
        {
            var mision = misiones.FirstOrDefault(x => x.Id == id);
            if (mision is null)
            {
                return NotFound("La mision no existe");
            }
            return Ok(mision);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Drone/{droneId}")]
    public IActionResult porDrone(int droneId)
    {
        try
        {
            if (!DroneController.drones.Any(x => x.Id == droneId))
            {
                return NotFound("El drone no existe");
            }

            var resultado = misiones.Where(x => x.DroneId == droneId).ToList();
            if (!resultado.Any())
            {
                return NotFound("El drone no tiene misiones");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Pendientes")]
    public IActionResult pendientes()
    {
        try
        {
            var lista = misiones.Where(x => !x.Completada).ToList();
            if (!lista.Any())
            {
                return NotFound("No hay misiones pendientes");
            }
            var resultado = lista.OrderBy(x => x.Fecha).ToList();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}
