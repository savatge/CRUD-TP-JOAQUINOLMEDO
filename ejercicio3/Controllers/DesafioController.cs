using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("[controller]")]
public class DesafioController : ControllerBase
{
    public static readonly List<Desafio> desafios = new()
    {
        new Desafio { Id = 1, Titulo = "FizzBuzz", Dificultad = "Facil", PuntajeMaximo = 100, Activo = true },
        new Desafio { Id = 2, Titulo = "Ordenamiento", Dificultad = "Media", PuntajeMaximo = 100, Activo = true },
        new Desafio { Id = 3, Titulo = "cs", Dificultad = "Dificil", PuntajeMaximo = 200, Activo = false }
    };

    [HttpPost]
    public IActionResult crearDesafio([FromBody] Desafio nuevoDesafio)
    {
        try
        {
            if (nuevoDesafio.Id <= 0 || string.IsNullOrWhiteSpace(nuevoDesafio.Titulo) || string.IsNullOrWhiteSpace(nuevoDesafio.Dificultad) || nuevoDesafio.PuntajeMaximo <= 0)
            {
                return BadRequest("Parametros nulos o no validos");
            }

            if (desafios.Any(x => x.Id == nuevoDesafio.Id))
            {
                return Conflict("Ese ID ya esta registrado.");
            }
            desafios.Add(nuevoDesafio);
            return Ok("Desafio creado con exito");
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Activos")]
    public IActionResult desafiosActivos()
    {
        try
        {
            var resultado = desafios.Where(x => x.Activo).ToList();
            if (!resultado.Any())
            {
                return NotFound("No hay desafios activos");
            }
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Dificultad/{dificultad}")]
    public IActionResult porDificultad(string dificultad)
    {
        try
        {
            var resultado = desafios.Where(x => x.Dificultad == dificultad).ToList();
            if (!resultado.Any())
            {
                return NotFound("No se encontraron desafios con esa dificultad");
            }
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("Promedio/{desafioId}")]
    public IActionResult promedio(int desafioId)
    {
        try
        {
            if (!desafios.Any(x => x.Id == desafioId))
            {
                return NotFound("El desafio no existe");
            }

            var lista = ResolucionController.resoluciones.Where(x => x.DesafioId == desafioId).ToList();
            if (!lista.Any())
            {
                return NotFound("El desafio no tiene resoluciones");
            }
            var resultado = lista.Average(x => x.PuntajeObtenido);
            return Ok(resultado);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}
