using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace PoLingual.Web.Middleware;

/// <summary>
/// Global exception handler implementing IExceptionHandler for centralized error handling.
/// Logs structured errors via Serilog and returns consistent error responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            FileNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
            OperationCanceledException => (HttpStatusCode.RequestTimeout, "Operation was cancelled."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new
        {
            error = message,
            statusCode = (int)statusCode,
            timestamp = DateTimeOffset.UtcNow
        }, cancellationToken);

        return true;
    }
}
