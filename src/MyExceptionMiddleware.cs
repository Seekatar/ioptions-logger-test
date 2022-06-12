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
        
        var ret = _next.Invoke(context);
        if (ret.Exception != null)
        {
            try
            {
                Exception ex = ret.Exception;
                if (ret.Exception is AggregateException aggr && aggr.InnerExceptions.Count == 1)
                {
                    ex = aggr.InnerExceptions[0];
                }
                var pd = new ProblemDetails
                {
                    Title = "Exception",
                    Detail = ex.Message
                };
                await context.Response.WriteAsJsonAsync(pd);
            }
            catch
            {
                // ??
            }
        }
    }
}