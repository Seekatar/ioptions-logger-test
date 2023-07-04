using Serilog.Context;

namespace IOptionTest.Auth;

/// <summary>
/// Less noisy that ASP.NET logging just to get the path of a request when testing Auth
/// </summary>
public class AuthLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthLoggingMiddleware> _logger;

    public AuthLoggingMiddleware(RequestDelegate next, ILogger<AuthLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public Task Invoke(HttpContext context)
    {
        _logger.LogInformation($">>>> {context.Request.Path}");
        return _next.Invoke(context);
    }
}