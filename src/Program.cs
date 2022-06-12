using System.Net;
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
        .Configure( o => o.OverriddenInCode = "OverriddenInCode")
        .ValidateDataAnnotations()
        .ValidateOnStart();

builder.Services.AddOptions<MonitoredOptions>()
        .Bind(builder.Configuration.GetSection(MonitoredOptions.SectionName))
        .Configure( o => o.OverriddenInCode = "OverriddenInCode")
        .ValidateDataAnnotations();

builder.Services.AddOptions<SnapshotOptions>()
        .Bind(builder.Configuration.GetSection(SnapshotOptions.SectionName))
        .Configure( o => o.OverriddenInCode = "OverriddenInCode")
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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CorrelationMiddleware>();

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
    private IDisposable _x;
    private IDisposable _y;

    public ProblemDetailExceptionFilter(ILogger<ProblemDetailExceptionFilter> logger)
    {
        _logger = logger;
    }

    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // without using, this works, but with using, it goes out of scope
        //_x = LogContext.PushProperty("test", "QQQQQQQQQQQQQQQQQQQQQQQQQ");

        var scope = new Dictionary<string, object>();
        if (context.ActionArguments.TryGetValue("clientId", out var clientId) && clientId is Guid)
            scope.Add("clientId", clientId);
        if (context.ActionArguments.TryGetValue("marketEntityId", out var marketEntityId) && marketEntityId is int)
            scope.Add("marketEntityId", marketEntityId);
        if (scope.Any())
            _y = _logger.BeginScope(scope);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null)
        {
            // this will have CorrelationId from Middleware context, but not scope in controller.
            _logger.LogError(context.Exception, "InFilter {message}", context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }

    public void Dispose()
    {
        if (_x is not null)
        {
            _x.Dispose();
        }
        if (_y is not null)
        {
            _y.Dispose();
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