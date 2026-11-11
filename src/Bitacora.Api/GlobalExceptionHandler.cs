using Bitacora.Application;
using Microsoft.AspNetCore.Diagnostics;

namespace Bitacora.Api;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is RegistroValidacionException validacion)
        {
            var errores = validacion.Errores.ToDictionary(error => error.Key, error => error.Value);
            await Results.ValidationProblem(
                errores,
                title: "Revisa los datos del registro.",
                instance: httpContext.Request.Path).ExecuteAsync(httpContext);
            return true;
        }

        _logger.LogError(exception, "Error al atender {Metodo} {Ruta}. TraceId: {TraceId}",
            httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "No se pudo completar la solicitud.",
            detail: "Ocurrió un error interno. Intenta nuevamente más tarde.",
            instance: httpContext.Request.Path,
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = httpContext.TraceIdentifier
            }).ExecuteAsync(httpContext);
        return true;
    }
}
