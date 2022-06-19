using Microsoft.AspNetCore.Mvc.Filters;
#pragma warning disable CA2254 // inconsistent use of Logger

// based on MS Doc https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0

class DisposeTest : IDisposable
{
    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~test()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
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
