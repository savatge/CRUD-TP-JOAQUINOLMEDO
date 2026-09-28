using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class MedicionController : ControllerBase
{
    public static readonly List<Medicion> mediciones = new()
    {
        new Medicion { Id = 1, EstacionId = 1, Temperatura = 22.5, Humedad = 60, VelocidadViento = 10, FechaHora = DateTime.Now }
    };

    [HttpPost]
    public IActionResult crearMedicion([FromBody] Medicion nuevaMedicion)
    {
        try
        {
            var estacion = EstacionController.estaciones.FirstOrDefault(x => x.Id == nuevaMedicion.EstacionId);
            if (estacion is null)
            {
                return NotFound("La estación no existe.");
            }

            if (!estacion.Activa)
            {
                return BadRequest("La estacion no esta activa");
            }

            if(nuevaMedicion.Id <= 0 || nuevaMedicion.EstacionId <= 0 || nuevaMedicion.VelocidadViento < 0)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (nuevaMedicion.Humedad < 0 || nuevaMedicion.Humedad > 100)
            {
                return BadRequest("La humedad debe estar entre 0 y 100");
            }

            if(nuevaMedicion.FechaHora > DateTime.Now)
            {
                return BadRequest("Fecha de medicion no valida ");
            }
            mediciones.Add(nuevaMedicion);
            return Ok(nuevaMedicion);
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
            if(mediciones.Count == 0)
            {
                return NotFound("No hay mediciones cargadas");
            }
            return Ok(mediciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Estacion/{estacionId}")]
    public IActionResult porEstacion(int estacionId)
    {
        try
        {
            var resultado = mediciones.Where(x => x.EstacionId == estacionId).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se ha encontrado una medicion con esa estacion");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
    
    [HttpGet("Temperatura/{Temperatura}")]
    public IActionResult porMayorTemperatura(double Temperatura)
    {
        try
        {
            var resultado = mediciones.Where(x => x.Temperatura > Temperatura).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se ha encontrado una medicion con temperatura mayor a la ingresada");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Temperatura/Orden")]
    public IActionResult porOrden()
    {
        try
        {
            if (!mediciones.Any())
            {
                return NotFound("No hay mediciones cargadas");
            }
            var resultado = mediciones.OrderByDescending(x => x.Temperatura).ToList();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Temperatura/Promedio")]
    public IActionResult promedio()
    {
        try
        {
            if (!mediciones.Any())
            {
                return NotFound("No hay mediciones cargadas");
            }
            var resultado = mediciones.Average(x => x.Temperatura);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Temperatura/Max")]
    public IActionResult maximaTemp()
    {
        try
        {
            if (!mediciones.Any())
            {
                return NotFound("No hay mediciones cargadas");
            }
            var resultado = mediciones.Max(x => x.Temperatura);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Temperatura/Min")]
    public IActionResult minimaTemp()
    {
        try
        {
            if (!mediciones.Any())
            {
                return NotFound("No hay mediciones cargadas");
            }
            var resultado = mediciones.Min(x => x.Temperatura);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("conteo")]
    public IActionResult ConteoMedicionesPorEstacion()
    {
        try
        {
            var resultado = EstacionController.estaciones.Select(x => new
            {
                Estacion = x.Nombre,
                CantidadMediciones = mediciones.Count(n => n.EstacionId == x.Id)

            }).ToList();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}