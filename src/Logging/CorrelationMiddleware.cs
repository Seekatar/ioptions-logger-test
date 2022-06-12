using Serilog.Context;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            using (LogContext.PushProperty("CorrelationId", correlationId.FirstOrDefault()))
            {
                return _next.Invoke(context);
            }
        }
        return _next.Invoke(context);
    }
}