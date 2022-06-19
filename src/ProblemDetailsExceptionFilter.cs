using Hellang.Middleware.ProblemDetails;
using IOptionTest.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IOptionTest;

#pragma warning disable CA2254 // inconsistent use of Logger

// based on MS Doc https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0

/// <summary>
/// Filter for catching exceptions and returning ProblemDetails object, as well as optionally logging the exception.
/// </summary>
/// <example>
/// <code>
///  builder.Services.AddControllers(options =>
///  {
///      options.Filters.Add&lt;ProblemDetailsExceptionFilter&gt;();
///  });
/// </code>
/// </example>
// ReSharper disable once ClassNeverInstantiated.Global
public class ProblemDetailsExceptionFilter : IActionFilter, IOrderedFilter
{
    private readonly ILogger<ProblemDetailsExceptionFilter> _logger;

    public ProblemDetailsExceptionFilter(ILogger<ProblemDetailsExceptionFilter> logger)
    {
        _logger = logger;
    }

    public int Order => int.MaxValue - 10; // << per doc: filter specifies an Order of the maximum integer value minus 10. This Order allows other filters to run at the end of the pipeline.

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null) return;

        if (context.Exception is not ProblemDetailsException problemDetailException)
        {
            _logger.LogError(context.Exception, "An unhandled exception has occurred while executing the request (in PDE Filter)!");
            return;
        }

        context.Result = new ObjectResult(problemDetailException.Details)
        {
            StatusCode = problemDetailException.Details.Status
        };

        _logger.LogException(problemDetailException.Details.Status >= (int)HttpStatusCode.InternalServerError ? LogLevel.Error : LogLevel.Warning, problemDetailException);

        context.ExceptionHandled = true; // prevent double logging
    }
}
#pragma warning restore CA2254