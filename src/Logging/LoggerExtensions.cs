using Serilog.Context;

namespace Seekatar.Tools;

#pragma warning disable CS8603 // Possible null return value.
public static class LoggerExtensions
{
    public static T Invoke<T>(this ILogger logger, object state, Func<T> func) where T : class
    {
        // this adds "scope" poperty with sub properties under it
        using var scope = LogContext.PushProperty("scope", state, destructureObjects: true);
        //using var scope = logger.BeginScope(state);
        return func();
    }
    public static T InvokeWorks<T>(this ILogger logger, object state, Func<T> func) where T : class
    {
        try
        {
            // this adds "scope" poperty with sub properties under it
            using var scope = LogContext.PushProperty("scope", state, destructureObjects: true);
            //using var scope = logger.BeginScope(state);
            return func();
        }
        catch (Exception ex) when (LogCaughtException(logger, ex))
        {
            // never get here since LogCaughtException returns false
        }
        // or here
        return default;
    }

    public static T InvokeDoesntLogScopeOnException<T>(this ILogger logger, object state, Func<T> func) where T : class
    {
        try
        {
            using var scope = LogContext.PushProperty("scope", state, destructureObjects: true);
            //using var scope = logger.BeginScope(state);
            return func();
        }
        catch (Exception ex)
        {
            LogCaughtException(logger, ex);
            throw;
        }
    }

    public static async Task<T> InvokeAsync<T>(this ILogger logger, object state, Func<Task<T>> func) where T : class
    {
        try
        {

            using var scope = LogContext.PushProperty("scope", state, destructureObjects: true);
            //using var scope = logger.BeginScope(state);
            return await func();
        }
        catch (Exception ex) when (LogCaughtException(logger, ex))
        {
            // never get here since LogCaughtException returns false
        }
        // or here
        return default;
    }

    public static T Invoke<T>(this ILogger logger, IDictionary<string, object> context, Func<T> func)
    {
        try
        {
            using var scope = logger.BeginScope(context);
            return func();
        }
        catch (Exception ex) when (LogCaughtException(logger, ex))
        {
            // never get here since LogCaughtException returns false
        }
        // or here
        return default;
    }

    public static async Task<T> InvokeAsync<T>(this ILogger logger, IDictionary<string, object> context, Func<Task<T>> func)
    {
        try
        {
            using var scope = logger.BeginScope(context);
            return await func();
        }
        catch (Exception ex) when (LogCaughtException(logger, ex))
        {
            // never get here since LogCaughtException returns false
        }
        // or here
        return default;
    }

    static bool LogCaughtException(ILogger logger, Exception ex)
    {
        logger.LogError(ex, "Exception");
        return false;
    }
}
#pragma warning restore CS8603 // Possible null return value.
