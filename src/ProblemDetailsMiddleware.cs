using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
                        ProblemDetails = pde.Details
                    });
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var pd = new ProblemDetails
                    {
                        Title = "Exception",
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