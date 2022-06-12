using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IOptionTest.Controllers
{
    /// <summary>
    /// Called when an error occurs
    /// </summary>
    /// <remarks>
    /// if (app.Environment.IsDevelopment())
    /// {
    ///     app.UseExceptionHandler("/error-local-development");
    /// }
    /// else
    /// {
    ///     app.UseExceptionHandler("/error");
    /// }
    /// </remarks>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors"/>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling"/>
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError()
        {
            return Problem();
        }

        /// <summary>
        /// For development, add on stack trace to the details
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error-local-development")]
        public IActionResult HandleDevError([FromServices] IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        }

    }
}
