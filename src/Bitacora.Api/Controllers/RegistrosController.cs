using Bitacora.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bitacora.Api.Controllers;

[ApiController]
[Route("api/registros")]
[Authorize] 
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class RegistrosController : ControllerBase
{
    private readonly RegistroService _service;

    public RegistrosController(RegistroService service)
    {
        _service = service;
    }

    private int UsuarioIdActual => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    private bool EsAdmin => User.IsInRole("Admin");

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroListadoDto>>> Listar(CancellationToken ct)
    {
        return Ok(await _service.ListarAsync(UsuarioIdActual, EsAdmin, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistroDetalleDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var registro = await _service.ObtenerPorIdAsync(id, ct);
        if (registro is null) return RegistroNoEncontrado(id);

        if (registro.UsuarioId != UsuarioIdActual && !EsAdmin) 
            return Forbid();

        return Ok(registro);
    }

    [HttpPost]
    [Consumes("application/json")]
    public async Task<ActionResult<RegistroDetalleDto>> Crear([FromBody] GuardarRegistroDto entrada, CancellationToken ct)
    {
        var registro = await _service.CrearAsync(entrada, UsuarioIdActual, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = registro.Id }, registro);
    }

    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] GuardarRegistroDto entrada, CancellationToken ct)
    {
        try
        {
            var actualizado = await _service.ActualizarAsync(id, entrada, UsuarioIdActual, EsAdmin, ct);
            return actualizado ? NoContent() : RegistroNoEncontrado(id);
        }
        catch (UnauthorizedAccessException)
        {
        
            return Forbid();
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        var eliminado = await _service.EliminarAsync(id, ct);
        return eliminado ? NoContent() : RegistroNoEncontrado(id);
    }

    private ObjectResult RegistroNoEncontrado(int id)
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Registro no encontrado.",
            detail: $"No existe un registro con el identificador {id}.",
            instance: HttpContext.Request.Path);
    }
}