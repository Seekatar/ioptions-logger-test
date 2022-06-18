using Microsoft.AspNetCore.Mvc;

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
            var pd = new ProblemDetails
            {
                Title = "Exception",
                Detail = ex.Message
            };
            context.Response.StatusCode = 402;
            await context.Response.WriteAsJsonAsync(pd);
        }
    }
}