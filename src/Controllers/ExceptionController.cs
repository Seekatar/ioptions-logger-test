using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using IOptionTest.Logging;
using IOptionTest.Models;
using IOptionTest.Attributes;
using Hellang.Middleware.ProblemDetails;

namespace IOptionTest.Controllers;

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
        throw new NotImplementedException("My function is not implemented");
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
            Type = "about:blank", 
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Throwing Problem Details",
            Detail = "My detail message, look for a and status of 500",
        };
        pd.Extensions["a"] = 1232;

        throw new ProblemDetailsException(pd);
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
                Type = "about:blank",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Throwing 'catch when' Problem Details",
                Detail = "My detail message, logged in when, then throw, look for a and status of 500",
            };
            pd.Extensions["a"] = 1232;

            throw new ProblemDetailsException(pd);
        });
    }

    [HttpGet]
    [Route("/api/throw/details-log/{clientId}/{marketEntityId}")]
    [ValidateModelState]
    [SwaggerOperation("ThrowLogProblemDetails")]
    [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
    public virtual ActionResult ThrowLogProblemDetails(Guid clientId, int marketEntityId)
    {
        var pd = new ProblemDetails()
        {
            Type = "about:blank",
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Throwing And Logging Problem Details",
            Detail = "Logging my detail message, look for a and status of 400",
        };
        pd.Extensions["a"] = 1232;

        throw _logger.LogProblemDetails(LogLevel.Warning, pd);
    }
}
