using Microsoft.AspNetCore.Mvc.Filters;
#pragma warning disable CA2254 // inconsistent use of Logger

// based on MS Doc https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0

/// <summary>
/// Filter for catching exceptions and returning ProblemDetails object
/// </summary>
/// <example>
/// <code>
///  builder.Services.AddControllers(options =>
///  {
///      options.Filters.Add<ProblemDetailExceptionFilter>();
///  });
/// </code>
/// </example>
public class LoggingContextFilter : IActionFilter, IOrderedFilter, IDisposable
{
    private readonly ILogger<LoggingContextFilter> _logger;
    private IDisposable? _serilogContext;
    private IDisposable? _loggerScope;

    public LoggingContextFilter(ILogger<LoggingContextFilter> logger)
    {
        _logger = logger;
    }

    public int Order => int.MaxValue - 10; // order this filter executes on (IOrderedFilter impl)

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // without using, this works, but with using, it goes out of scope

        //_serilogContext = LogContext.PushProperty("test", "QQQQQQQQQQQQQQQQQQQQQQQQQ");

        var scope = new Dictionary<string, object>();
        if (context.ActionArguments.TryGetValue("clientId", out var clientId) && clientId is Guid)
            scope.Add("clientId", clientId);
        if (context.ActionArguments.TryGetValue("marketEntityId", out var marketEntityId) && marketEntityId is int)
            scope.Add("marketEntityId", marketEntityId);
        if (scope.Any())
            _loggerScope = _logger.BeginScope(scope);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null)
        {
            // this will have CorrelationId from Middleware context, but not scope in controller.
            _logger.LogError(context.Exception, "InFilter {message}", context.Exception.Message);

            context.ExceptionHandled = false; // if true a 200 is sent to client
        }
    }

    public void Dispose()
    {
        if (_serilogContext is not null)
        {
            _serilogContext.Dispose();
        }
        if (_loggerScope is not null)
        {
            _loggerScope.Dispose();
        }
    }
}
#pragma warning restore CA2254
