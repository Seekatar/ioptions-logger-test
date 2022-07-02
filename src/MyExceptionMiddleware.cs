using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class MyExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public MyExceptionMiddleware(RequestDelegate next)
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

            if (ex is ProblemDetailsException pde)
            {
                context.Response.StatusCode = pde.Details.Status ?? (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(pde.Details);
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
    }
}