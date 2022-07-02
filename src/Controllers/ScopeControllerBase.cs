using Microsoft.AspNetCore.Mvc;
using IOptionTest.Services;
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
       ScopedLogging.DefaultLogException = (logger, ex) => logger.LogError(ex, ">>>>>>>>Exception");
       _logger = logger;
   }

   protected T Invoke<T>(Func<T> func, IDictionary<string, object> context)
   {
       return _logger.Invoke(func,context);
   }
   protected async Task<T> InvokeAsync<T>(Func<Task<T>> func, IDictionary<string, object> context)
   {
       return await _logger.InvokeAsync(func,context);

   }
}
