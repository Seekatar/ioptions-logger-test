using Microsoft.AspNetCore.Mvc;

namespace IOptionTest.Services;

#pragma warning disable CS8603 // Possible null return value.
public static class ScopedLogging
{
   public static T Invoke<T>(this ILogger logger, Func<T> func, IDictionary<string, object> context)
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

   public static async Task<T> InvokeAsync<T>(this ILogger logger, Func<Task<T>> func, IDictionary<string, object> context)
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
   public delegate void LogException(ILogger logger, Exception ex);
   public static LogException DefaultLogException = (logger, ex) => logger.LogError(ex, "Exception");

   private static bool LogCaughtException(ILogger logger, Exception ex)
   {
       DefaultLogException(logger, ex);
       return false;
   }
}
#pragma warning restore CS8603 // Possible null return value.
