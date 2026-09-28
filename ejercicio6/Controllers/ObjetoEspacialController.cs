using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace testing.Controllers;

[ApiController]
[Route("api/objetosespaciales")]
public class ObjetoEspacialController : ControllerBase
{
    public static readonly List<ObjetoEspacial> objetos = new()
    {
        new ObjetoEspacial { Id = 1, Nombre = "Apophis", Tipo = "Asteroide", Distancia = 800, NivelRiesgo = 5, FechaDescubrimiento = DateTime.Parse("2025-01-10"), Activo = true },
        new ObjetoEspacial { Id = 2, Nombre = "Halley", Tipo = "Cometa", Distancia = 3000, NivelRiesgo = 2, FechaDescubrimiento = DateTime.Parse("2025-02-15"), Activo = true },
        new ObjetoEspacial { Id = 3, Nombre = "Satelite Alfa", Tipo = "Satelite", Distancia = 400, NivelRiesgo = 1, FechaDescubrimiento = DateTime.Parse("2025-03-20"), Activo = true },
        new ObjetoEspacial { Id = 4, Nombre = "Objeto X", Tipo = "Desconocido", Distancia = 900, NivelRiesgo = 4, FechaDescubrimiento = DateTime.Parse("2025-04-05"), Activo = false },
        new ObjetoEspacial { Id = 5, Nombre = "Bennu", Tipo = "Asteroide", Distancia = 1500, NivelRiesgo = 4, FechaDescubrimiento = DateTime.Parse("2025-05-12"), Activo = true },
        new ObjetoEspacial { Id = 6, Nombre = "Objeto Y", Tipo = "Desconocido", Distancia = 2200, NivelRiesgo = 3, FechaDescubrimiento = DateTime.Parse("2025-06-30"), Activo = true }
    };

    [HttpPost]
    public IActionResult crearObjeto([FromBody] ObjetoEspacial nuevoObjeto)
    {
        if (nuevoObjeto.Id <= 0 || string.IsNullOrWhiteSpace(nuevoObjeto.Nombre) || string.IsNullOrWhiteSpace(nuevoObjeto.Tipo) || nuevoObjeto.Distancia <= 0 || nuevoObjeto.NivelRiesgo < 1 || nuevoObjeto.NivelRiesgo > 5)
        {
            return BadRequest("Parametros nulos o no validos");
        }

        if (nuevoObjeto.Tipo != "Asteroide" && nuevoObjeto.Tipo != "Cometa" && nuevoObjeto.Tipo != "Satelite" && nuevoObjeto.Tipo != "Desconocido")
        {
            return BadRequest("Tipo no valido");
        }

        if (nuevoObjeto.FechaDescubrimiento > DateTime.Now)
        {
            return BadRequest("Fecha de descubrimiento no valida");
        }

        if (objetos.Any(x => x.Id == nuevoObjeto.Id))
        {
            return Conflict("Ese ID ya esta registrado.");
        }
        objetos.Add(nuevoObjeto);
        return Ok("Objeto espacial registrado con exito");
    }

    [HttpGet("Nombre/{nombre}")]
    public IActionResult porNombre(string nombre)
    {
        var resultado = objetos.Where(x => x.Nombre.Contains(nombre)).ToList();
        if (!resultado.Any())
        {
            return NotFound("No se encontraron objetos con ese nombre");
        }
        return Ok(resultado);
    }

    [HttpGet("Tipo/{tipo}")]
    public IActionResult porTipo(string tipo)
    {
        var resultado = objetos.Where(x => x.Tipo == tipo).ToList();
        if (!resultado.Any())
        {
            return NotFound("No se encontraron objetos de ese tipo");
        }
        return Ok(resultado);
    }

    [HttpGet("Riesgo/{nivel}")]
    public IActionResult porRiesgo(int nivel)
    {
        var resultado = objetos.Where(x => x.NivelRiesgo == nivel).ToList();
        if (!resultado.Any())
        {
            return NotFound("No se encontraron objetos con ese nivel de riesgo");
        }
        return Ok(resultado);
    }

    [HttpGet("Distancia/{distancia}")]
    public IActionResult masCercanos(double distancia)
    {
        var resultado = objetos.Where(x => x.Distancia < distancia).ToList();
        if (!resultado.Any())
        {
            return NotFound("No se encontraron objetos con distancia inferior a la ingresada");
        }
        return Ok(resultado);
    }

    [HttpGet("MasObservaciones")]
    public IActionResult masObservaciones()
    {
        if (!objetos.Any())
        {
            return NotFound("No hay objetos cargados");
        }
        var totales = objetos.Select(x => new
        {
            Objeto = x.Nombre,
            Observaciones = ObservacionController.observaciones.Count(n => n.ObjetoEspacialId == x.Id)
        }).ToList();
        var resultado = totales.OrderByDescending(x => x.Observaciones).FirstOrDefault();
        return Ok(resultado);
    }

    [HttpGet("SinObservaciones")]
    public IActionResult sinObservaciones()
    {
        var resultado = objetos.Where(x => !ObservacionController.observaciones.Any(n => n.ObjetoEspacialId == x.Id)).ToList();
        if (!resultado.Any())
        {
            return NotFound("Todos los objetos tienen observaciones");
        }
        return Ok(resultado);
    }

    [HttpGet("Ordenados")]
    public IActionResult ordenadosPorRiesgo()
    {
        if (!objetos.Any())
        {
            return NotFound("No hay objetos cargados");
        }
        var resultado = objetos.OrderByDescending(x => x.NivelRiesgo).ToList();
        return Ok(resultado);
    }

    [HttpGet("Estadisticas")]
    public IActionResult estadisticas()
    {
        if (!objetos.Any() || !ObservacionController.observaciones.Any())
        {
            return NotFound("No hay datos suficientes para calcular estadisticas");
        }
        var resultado = new
        {
            TotalObjetos = objetos.Count,
            ObjetosActivos = objetos.Count(x => x.Activo),
            Asteroides = objetos.Count(x => x.Tipo == "Asteroide"),
            Cometas = objetos.Count(x => x.Tipo == "Cometa"),
            Satelites = objetos.Count(x => x.Tipo == "Satelite"),
            Desconocidos = objetos.Count(x => x.Tipo == "Desconocido"),
            PromedioRiesgo = objetos.Average(x => x.NivelRiesgo),
            DistanciaMinima = objetos.Min(x => x.Distancia),
            TotalObservaciones = ObservacionController.observaciones.Count,
            PromedioVelocidad = ObservacionController.observaciones.Average(x => x.Velocidad)
        };
        return Ok(resultado);
    }

    [HttpGet("alertas")]
    public IActionResult alertas()
    {
        var nivelMinimo = 4;
        var distanciaMaxima = 1000;
        var resultado = objetos.Where(x => x.NivelRiesgo >= nivelMinimo && x.Distancia < distanciaMaxima && x.Activo).ToList();
        if (!resultado.Any())
        {
            return NotFound("No hay objetos en alerta");
        }
        return Ok(resultado);
    }
}