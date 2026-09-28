using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class DroneController : ControllerBase
{
    public static readonly List<Drone> drones = new()
    {
        new Drone { Id = 1, Codigo = "DR-001", Modelo = "Phantom", Bateria = 80, Estado = "Disponible" },
        new Drone { Id = 2, Codigo = "DR-002", Modelo = "Mavic", Bateria = 25, Estado = "Disponible" },
        new Drone { Id = 3, Codigo = "DR-003", Modelo = "Phantom", Bateria = 90, Estado = "EnMision" },
        new Drone { Id = 4, Codigo = "DR-004", Modelo = "Inspire", Bateria = 60, Estado = "Mantenimiento" }
    };

    [HttpPost]
    public IActionResult crearDrone([FromBody] Drone nuevoDrone)
    {
        try
        {
            if (nuevoDrone.Id <= 0 || string.IsNullOrWhiteSpace(nuevoDrone.Codigo) || string.IsNullOrWhiteSpace(nuevoDrone.Modelo) || nuevoDrone.Bateria < 0 || nuevoDrone.Bateria > 100)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (nuevoDrone.Estado != "Disponible" && nuevoDrone.Estado != "EnMision" && nuevoDrone.Estado != "Mantenimiento")
            {
                return BadRequest("Estado no valido");
            }

            if (drones.Any(x => x.Id == nuevoDrone.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }

            if (drones.Any(x => x.Codigo == nuevoDrone.Codigo))
            {
                return Conflict("Ese codigo ya esta registrado.");
            }
            drones.Add(nuevoDrone);
            return Ok("Drone registrado con exito");
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
            if (!drones.Any())
            {
                return NotFound("No hay drones cargados");
            }
            return Ok(drones);
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
            var drone = drones.FirstOrDefault(x => x.Id == id);
            if (drone is null)
            {
                return NotFound("El drone no existe");
            }
            return Ok(drone);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpPut]
    public IActionResult actualizarDrone([FromBody] Drone droneActualizado)
    {
        try
        {
            var drone = drones.FirstOrDefault(x => x.Id == droneActualizado.Id);
            if (drone is null)
            {
                return NotFound("El drone no existe");
            }

            if (string.IsNullOrWhiteSpace(droneActualizado.Codigo) || string.IsNullOrWhiteSpace(droneActualizado.Modelo) || droneActualizado.Bateria < 0 || droneActualizado.Bateria > 100)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (droneActualizado.Estado != "Disponible" && droneActualizado.Estado != "EnMision" && droneActualizado.Estado != "Mantenimiento")
            {
                return BadRequest("Estado no valido");
            }

            if (drones.Any(x => x.Codigo == droneActualizado.Codigo && x.Id != droneActualizado.Id))
            {
                return Conflict("Ese codigo ya esta registrado.");
            }
            drone.Codigo = droneActualizado.Codigo;
            drone.Modelo = droneActualizado.Modelo;
            drone.Bateria = droneActualizado.Bateria;
            drone.Estado = droneActualizado.Estado;
            return Ok("Drone modificado con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult eliminarDrone(int id)
    {
        try
        {
            var drone = drones.FirstOrDefault(x => x.Id == id);
            if (drone is null)
            {
                return NotFound("El drone no existe");
            }
            drones.Remove(drone);
            return Ok("Drone eliminado con exito");
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
            var resultado = drones.Where(x => x.Estado == "Disponible").ToList();
            if (!resultado.Any())
            {
                return NotFound("No hay drones disponibles");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Bateria/{porcentaje}")]
    public IActionResult bateriaBaja(int porcentaje)
    {
        try
        {
            var resultado = drones.Where(x => x.Bateria < porcentaje).ToList();
            if (!resultado.Any())
            {
                return NotFound("No hay drones con bateria inferior a la ingresada");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Modelo/{modelo}")]
    public IActionResult porModelo(string modelo)
    {
        try
        {
            var resultado = drones.Where(x => x.Modelo == modelo).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se encontraron drones de ese modelo");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("PromedioBateria")]
    public IActionResult promedioBateria()
    {
        try
        {
            var disponibles = drones.Where(x => x.Estado == "Disponible").ToList();
            if (!disponibles.Any())
            {
                return NotFound("No hay drones disponibles");
            }
            var resultado = disponibles.Average(x => x.Bateria);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Kilometros")]
    public IActionResult kilometrosPorDrone()
    {
        try
        {
            if (!drones.Any())
            {
                return NotFound("No hay drones cargados");
            }
            var resultado = drones.Select(x => new
            {
                Drone = x.Codigo,
                KmRecorridos = MisionController.misiones.Where(n => n.DroneId == x.Id && n.Completada).Sum(n => n.DistanciaKm)
            }).ToList();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("MasKilometros")]
    public IActionResult masKilometros()
    {
        try
        {
            if (!drones.Any())
            {
                return NotFound("No hay drones cargados");
            }
            var totales = drones.Select(x => new
            {
                Drone = x.Codigo,
                KmRecorridos = MisionController.misiones.Where(n => n.DroneId == x.Id && n.Completada).Sum(n => n.DistanciaKm)
            }).ToList();
            var resultado = totales.OrderByDescending(x => x.KmRecorridos).FirstOrDefault();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}