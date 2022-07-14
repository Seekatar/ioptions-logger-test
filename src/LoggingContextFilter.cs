using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Filter for adding Client data to logging via Scope
/// </summary>
/// <example>
/// <code>
///  builder.Services.AddControllers(options =>
///  {
///      options.Filters.Add<LoggingContextFilter>();
///  });
/// </code>
/// </example>
public class LoggingContextFilter : IActionFilter, IDisposable
{
    private readonly ILogger<LoggingContextFilter> _logger;
    private IDisposable? _loggerScope;

    public LoggingContextFilter(ILogger<LoggingContextFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var scope = new Dictionary<string, object>();
        if (context.ActionArguments.TryGetValue("clientId", out var clientId) && (clientId is string or Guid))
            scope.Add("clientId", clientId);
        if (context.ActionArguments.TryGetValue("marketEntityId", out var marketEntityId) && marketEntityId is int)
            scope.Add("marketEntityId", marketEntityId);
        if (scope.Any())
            _loggerScope = _logger.BeginScope(scope);

        // dispose never called on DisposeTest, but default ExceptionHandling middleware doesn't use scope
        context.HttpContext.Items["scope"] = _loggerScope;
        context.HttpContext.Items["test"] = new DisposeTest();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _loggerScope?.Dispose();
        _loggerScope = null;
    }

    public void Dispose()
    {
        _loggerScope?.Dispose();
    }
}
#pragma warning restore CA2254
