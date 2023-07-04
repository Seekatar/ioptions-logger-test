using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Net.WebRequestMethods;

namespace Seekatar.ProblemDetails;

public class ProblemDetailsException : Exception
{
    public Microsoft.AspNetCore.Mvc.ProblemDetails ProblemDetails { get; init; }

    public const string Status400Url = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1";
    public const string Status500Url = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1";
    
    public ProblemDetailsException(HttpStatusCode statusCode)
    {
        ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails { 
            Status = (int)statusCode, 
            Type = SetType((int)statusCode),
            Title = nameof(Seekatar.ProblemDetails.ProblemDetailsException)
        };
    }
    public ProblemDetailsException(Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }
    
    public ProblemDetailsException(Exception e, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = (int)statusCode,
            Type = SetType((int)statusCode),
            Title = nameof(HttpStatusCode.InternalServerError),
            Detail = e.Message
        };
    }

    public ProblemDetailsException(string title, Exception e, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = (int)statusCode,
            Type = SetType((int)statusCode),
            Title = title,
            Detail = e.Message
        };
    }

    private static string SetType(int statusCode) => statusCode >= 500 ? Status500Url : Status400Url;
}

/// <summary>
/// Use the .NET 7 problemDetailsService.WriteAsync to write problem details out
/// This writes details, even if developer exceptions are turned off, otherwise
/// the default middleware just returns minimal details
/// </summary>
public class ProblemDetailsMiddleware
{
    private readonly RequestDelegate _next;

    public ProblemDetailsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            if (ex is AggregateException aggr && aggr.InnerExceptions.Count == 1)
            {
                ex = aggr.InnerExceptions[0];
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
            {
                if (ex is ProblemDetailsException pde)
                {
                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = pde.ProblemDetails
                    });
                }
                else if (ex is Hellang.Middleware.ProblemDetails.ProblemDetailsException pde2)
                {
                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = pde2.Details
                    });
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var pd = new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Type = ProblemDetailsException.Status500Url,
                        Title = "Unhandled exception",
                        Detail = ex.Message,
                        Status = context.Response.StatusCode
                    };
                    await context.Response.WriteAsJsonAsync(pd);

                    // throw; // logs this, but also two other errors about response already started
                }
            }
            else
            {
                throw;
            }
        }
    }
}