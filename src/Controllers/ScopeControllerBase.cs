using Microsoft.AspNetCore.Mvc;

namespace IOptionTest.Controllers;

/// <summary>
/// base class for scoped logging, this works to keep the scope when an exception is thrown
/// by logging in the 'when' clause of the catch statement since it will still have the Scope
/// object, but catch will not
/// </summary>
public class ScopeControllerBase : ControllerBase
{
    private readonly ILogger _logger;

    protected ILogger Logger => _logger;

    protected ScopeControllerBase(ILogger logger)
    {
        _logger = logger;
    }

#pragma warning disable CS8603 // Possible null return value.
    protected T Invoke<T>(Func<T> func, IDictionary<string, object> context)
    {
        try
        {
            using var scope = _logger.BeginScope(context);
            return func();
        }
        catch (Exception ex) when (LogCaughtException(ex))
        {
            // never get here since LogCaughtException returns false
        }
        // or here
        return default;
    }
    protected async Task<T> InvokeAsync<T>(Func<Task<T>> func, IDictionary<string, object> context)
    {
        try
        {
            using var scope = _logger.BeginScope(context);
            return await func();
        }
        catch (Exception ex) when (LogCaughtException(ex))
        {
            // never get here since LogCaughtException returns false
        }
        // or here
        return default;

    }
#pragma warning restore CS8603 // Possible null return value.

    protected virtual void LogException(Exception ex)
    {
        _logger.LogError(ex, "Exception");
    }

    protected virtual bool LogCaughtException(Exception ex)
    {
        LogException(ex);
        return false;
    }
}

