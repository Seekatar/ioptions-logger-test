using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using IOptionTest.Logging;
using IOptionTest.Models;
using IOptionTest.Attributes;
using static IOptionTest.Options.ExceptionOptions;
using System.Net.Http;
using System.Reflection;
using static System.Net.WebRequestMethods;
using IOptionTest.Options;

namespace IOptionTest.Controllers;

[Produces("application/json")]
public class ExceptionController : Controller
{
    private readonly ILogger<ExceptionController> _logger;

    public ExceptionController(ILogger<ExceptionController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("/api/throw/not-implemented/{clientId}/{marketEntityId}")]
    [ValidateModelState]
    [SwaggerOperation("ThrowNotImplemented")]
    [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
    public virtual ActionResult ThrowNotImplemented(Guid clientId, int marketEntityId)
    {
        throw new NotImplementedException("Throwing NotImplementedException");
    }

    [HttpGet]
    [Route("/api/throw/details/{clientId}/{marketEntityId}")]
    [ValidateModelState]
    [SwaggerOperation("ThrowProblemDetails")]
    [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
    public virtual ActionResult ThrowProblemDetails(Guid clientId, int marketEntityId)
    {
        var pd = new ProblemDetails()
        {
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Throwing ProblemDetailsException",
            Detail = "My detail message, look for a and status of 500",
        };
        pd.Extensions["extension_value_int"] = 1232;
        pd.Extensions["extension_value_string"] = "Some value";
        pd.Extensions["method_name"] = MethodBase.GetCurrentMethod()?.Name ?? "Unknown";

        if (ExceptionHandler == ExceptionHandlerEnum.DotNet7)
            throw new Seekatar.ProblemDetails.ProblemDetailsException(pd);
        else
            throw new Hellang.Middleware.ProblemDetails.ProblemDetailsException(pd);
    }

    [HttpGet]
    [Route("/api/throw/details-scope/{clientId}/{marketEntityId}")]
    [ValidateModelState]
    [SwaggerOperation("ThrowProblemDetailsScoped")]
    [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
    public virtual ActionResult ThrowProblemDetailsScoped(Guid clientId, int marketEntityId)
    {
        return _logger.InvokeLogged<ActionResult>(() =>
        {
            _logger.LogInformation("Hi with scopes");

            var pd = new ProblemDetails()
            {
                Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Throwing 'catch when' ProblemDetailsException",
                Detail = "My detail message, logged in when, then throw, look for a and status of 500",
            };
            pd.Extensions["extension_value_int"] = 1232;
            pd.Extensions["extension_value_string"] = "Some value";
            pd.Extensions["method_name"] = MethodBase.GetCurrentMethod()?.Name ?? "Unknown";

            if (ExceptionHandler == ExceptionHandlerEnum.DotNet7)
                throw new Seekatar.ProblemDetails.ProblemDetailsException(pd);
            else
                throw new Hellang.Middleware.ProblemDetails.ProblemDetailsException(pd);
        });
    }

    [HttpGet]
    [Route("/api/throw/details-log/{clientId}/{marketEntityId}")]
    [ValidateModelState]
    [SwaggerOperation("ThrowLogProblemDetails")]
    [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
    public virtual void ThrowLogProblemDetails(Guid clientId, int marketEntityId)
    {
        var pd = new ProblemDetails()
        {
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Status = (int)HttpStatusCode.BadRequest,
            Title = "_logger.LogProblemDetails",
            Detail = "Logging my detail message, look for a and status of 400",
        };
        pd.Extensions["extension_value_int"] = 1232;
        pd.Extensions["extension_value_string"] = "Some value";
        pd.Extensions["method_name"] = MethodBase.GetCurrentMethod()?.Name ?? "Unknown";

        throw _logger.LogProblemDetails(LogLevel.Warning, pd);
        
    }
}
