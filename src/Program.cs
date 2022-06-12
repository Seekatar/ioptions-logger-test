using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OptionLoggerTest;
using OptionsLoggerTest.Interfaces;
using OptionsLoggerTest.Services;
using Seekatar.Tools;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.InsertSharedDevSettings();

#region Add Serilog
builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(builder.Configuration));
#endregion

builder.Services.AddControllers(options => { options.Filters.Add<ProblemDetailExceptionFilter>(); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IOptionsService, OptionsService>();

#region Add Options
builder.Services.AddOptions<OneTimeOptions>()
        .Bind(builder.Configuration.GetSection(OneTimeOptions.SectionName))
        .Configure(o => o.OverriddenInCode = "OverriddenInCode")
        .ValidateDataAnnotations()
        .ValidateOnStart();

builder.Services.AddOptions<MonitoredOptions>()
        .Bind(builder.Configuration.GetSection(MonitoredOptions.SectionName))
        .Configure(o => o.OverriddenInCode = "OverriddenInCode")
        .ValidateDataAnnotations();

builder.Services.AddOptions<SnapshotOptions>()
        .Bind(builder.Configuration.GetSection(SnapshotOptions.SectionName))
        .Configure(o => o.OverriddenInCode = "OverriddenInCode")
        .ValidateDataAnnotations();
#endregion

var app = builder.Build();


// var logger = app.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger>();
// var logger = app.Services.GetRequiredService<ILogger<TestLogger>>();
// app.ConfigureExceptionHandler(logger);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


    // middleware handling (preferred), but don't have scope

    // send to controller that will log
    // app.UseExceptionHandler("/error-local-development");

    //app.UseExceptionHandler(exceptionApp => {

    //    exceptionApp.Run(async context => {
    //        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //        context.Response.ContentType = "application/json";

    //        var contextFeature = context.Features
    //            .Get<IExceptionHandlerPathFeature>();

    //        if (contextFeature != null)
    //        {
    //            try
    //            {
    //                var pd = new ProblemDetails
    //                {
    //                    Title = "Exception",
    //                    Detail = contextFeature.Error.Message
    //                };
    //                await context.Response.WriteAsJsonAsync(pd);
    //            } catch
    //            {
    //                // ?? if throw, then ASP.NET will log it.
    //            }
    //        }
    //        // it still logs the exception
    //    });
    //});
}
else
{
    // middleware handling (preferred), but don't have scope
    app.UseExceptionHandler("/error");
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<MyExceptionMiddleware>();

app.Run();
public class TestLogger { }

#pragma warning disable CA2254 // inconsistent use of Logger

// based on MS Doc https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0

/// <summary>
/// Filter for catching exceptions and returning ProblemDetails object
/// </summary>
/// <example>
/// <code>
///  builder.Services.AddControllers(options =>
///  {
///      options.Filters.Add<ProblemDetailExceptionFilter>();
///  });
/// </code>
/// </example>
public class ProblemDetailExceptionFilter : IActionFilter, IOrderedFilter, IDisposable
{
    private readonly ILogger<ProblemDetailExceptionFilter> _logger;
    private IDisposable? _serilogContext;
    private IDisposable? _loggerScope;

    public ProblemDetailExceptionFilter(ILogger<ProblemDetailExceptionFilter> logger)
    {
        _logger = logger;
    }

    public int Order => int.MaxValue - 10; // order this filter executes on (IOrderedFilter impl)

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // without using, this works, but with using, it goes out of scope

        //_serilogContext = LogContext.PushProperty("test", "QQQQQQQQQQQQQQQQQQQQQQQQQ");

        var scope = new Dictionary<string, object>();
        if (context.ActionArguments.TryGetValue("clientId", out var clientId) && clientId is Guid)
            scope.Add("clientId", clientId);
        if (context.ActionArguments.TryGetValue("marketEntityId", out var marketEntityId) && marketEntityId is int)
            scope.Add("marketEntityId", marketEntityId);
        if (scope.Any())
            _loggerScope = _logger.BeginScope(scope);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null)
        {
            // this will have CorrelationId from Middleware context, but not scope in controller.
            _logger.LogError(context.Exception, "InFilter {message}", context.Exception.Message);
            
            context.ExceptionHandled = false; // if true a 200 is sent to client
        }
    }

    public void Dispose()
    {
        if (_serilogContext is not null)
        {
            _serilogContext.Dispose();
        }
        if (_loggerScope is not null)
        {
            _loggerScope.Dispose();
        }
    }
}
#pragma warning restore CA2254
public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, Microsoft.Extensions.Logging.ILogger logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    logger.LogError($"Something went wrong: {contextFeature.Error}");
                    await context.Response.WriteAsync((new ProblemDetails()
                    {
                        Status = context.Response.StatusCode,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                    }.ToString()) ?? "");


                }
            });
        });
    }
}