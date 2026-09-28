using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class EstacionController : ControllerBase
{
    public static readonly List<Estacion> estaciones = new()
    {
        new Estacion { Id = 1, Nombre = "Estacion Centro", Localidad = "Cordoba Capital", Activa = true },
        new Estacion { Id = 2, Nombre = "Estacion Norte", Localidad = "Villa Carls Paz", Activa = false }
    };

    [HttpPost]
    public IActionResult crearEstacion ([FromBody] Estacion NuevaEstacion)
    {
        try
        {
            if (NuevaEstacion.Id <= 0 || string.IsNullOrWhiteSpace(NuevaEstacion.Localidad) || string.IsNullOrWhiteSpace(NuevaEstacion.Nombre))
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (estaciones.Any(x => x.Nombre == NuevaEstacion.Nombre))
            {
                return Conflict("Ese nombre de estacion ya esta registrado.");
            }
            estaciones.Add(NuevaEstacion);
            return Ok("Estacion registrada con exito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Localidad")]
    public IActionResult PorLocalidad (string localidad)
    {
        try
        {
            if(!estaciones.Any())
            {
                return NotFound("Lista vacia");
            }
            var resultado = estaciones.Where(x => x.Localidad == localidad).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se encontraron estaciones para esa localidad");
            }
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}

