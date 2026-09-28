using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace Testing.Controllers;

[ApiController]
[Route("[controller]")]
public class objetoPerdidoController : ControllerBase
{
    private readonly ILogger<objetoPerdidoController> _logger;

    private static readonly List<ObjetoPerdido> objetoPerdidos = new List<ObjetoPerdido>(){
        new ObjetoPerdido { Id = 1, Descripcion = "Llave", Categoria = "Herramientas", LugarEncontrado = "Sala de conferencias", NombrePersonaQueRetiro = "Juan Pérez", Reclamado = false },
    };

    public objetoPerdidoController(ILogger<objetoPerdidoController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult create([FromBody] ObjetoPerdido objetoPerdido)
    {
        try
        {
            foreach(ObjetoPerdido o in objetoPerdidos)
            {
                if(o.Id == objetoPerdido.Id)
                {
                    return NotFound("id ya existente");
                }
            }
            objetoPerdidos.Add(objetoPerdido);
            return Ok(objetoPerdido);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        try
        {
            if(objetoPerdidos == null)
            {
                NotFound("no hay objetos perdidos");
            }
            return Ok(objetoPerdidos);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        try
        {
            var objetoPerdido = objetoPerdidos.FirstOrDefault(u => u.Id == id);

            if (objetoPerdido == null)
            {
                return NotFound("Objeto no encontrado");
            }

            return Ok(objetoPerdido);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpPut]
    public IActionResult update(int id,[FromBody] ObjetoPerdido objetoPerdido)
    {
        try
        {
            var objeto = objetoPerdidos.FirstOrDefault(u => u.Id == id);
            if(objeto is null)
            {
                return NotFound("No se encontro ese objeto");
            }
            objeto.Descripcion = objetoPerdido.Descripcion;
            objeto.Categoria = objetoPerdido.Categoria;
            objeto.LugarEncontrado = objetoPerdido.LugarEncontrado;
            objeto.NombrePersonaQueRetiro = objetoPerdido.NombrePersonaQueRetiro;
            objeto.Reclamado = objetoPerdido.Reclamado;

            return Ok("Objeto modificado");
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult delete(int id)
    {
        try
        {
            var objeto = objetoPerdidos.FirstOrDefault(u => u.Id == id);
            if(objeto is null)
            {
                return NotFound("No se encontro ese objeto");
            }
            objetoPerdidos.Remove(objeto);
            return Ok("Objeto eliminado");
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("descripcion/{descripcion}")]
    public IActionResult GetByDescripcion(string descripcion)
    {
        try
        {
            var objeto = objetoPerdidos.FirstOrDefault(u => u.Descripcion == descripcion);
            if(objetoPerdidos == null)
            {
                return NotFound("No hay objetos");
            }
            else if(objeto is null)
            {
                return NotFound("No se encontro ese objeto");
            }
            return Ok(objeto);   
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("categoria/{categoria}")]
    public IActionResult GetByCategoria(string categoria)
    {
        try
        {
            var objetos = objetoPerdidos.Where(n => n.Categoria == categoria).ToList();
            if(objetoPerdidos == null)
            {
                return NotFound("No hay objetos");
            }
            else if(objetos is null)
            {
                return NotFound("No se encontro ese objeto");
            }
            return Ok(objetos);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("reclamados")]
    public IActionResult GetByReclamado()
    {
        try
        {
            var objetos = objetoPerdidos.Where(x => x.Reclamado == true).ToList();
            if(!objetos.Any())
            {
                return NotFound("No hay objetos reclamados");
            }
            return Ok(objetos);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("fecha/{fecha}")]
    public IActionResult GetByFecha(DateTime fecha)
    {
        try
        {
            var objetos = objetoPerdidos.Where(x => x.FechaEncontrado > fecha).ToList();
            if(objetos is null)
            {
                return NotFound("No hay objetos");
            }
            return Ok (objetos);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }

    [HttpGet("fechaEncontrado")]
    public IActionResult GetByFechaEncontrado()
    {
        try
        {
            var objetos = objetoPerdidos.OrderByDescending(x => x.FechaEncontrado).ToList();
            if(objetos is null)
            {
                return NotFound("No hay objetos");
            }
            return Ok (objetos);
        }
        catch
        {
            return NotFound("Error del servidor...");
        }
    }
    
    [HttpPut("{id}")]
    public IActionResult update(int id)
    {
        try
        {
            var objeto = objetoPerdidos.FirstOrDefault(x => x.Id == id);
            if (objeto == null)
            {
                return NotFound("Objeto no encontrado");
            }
            objeto.Reclamado = true;
            return Ok(objeto);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Ocurrio un error en el servidor: {ex.Message}");
        }
    }
}
