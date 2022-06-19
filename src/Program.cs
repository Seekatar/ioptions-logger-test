using Hellang.Middleware.ProblemDetails;
using IOptionTest;
using IOptionTest.Interfaces;
using IOptionTest.Options;
using IOptionTest.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Seekatar.Tools;
using Serilog;
using System.Net;

var exceptionHandler = ExceptionHandlerEnum.UseHellang;
bool useExceptionLoggingFilter = false; // this will cause some redundant logging, but you get the idea

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.InsertSharedDevSettings();

#region Add Serilog
builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(builder.Configuration));
#endregion

if (exceptionHandler == ExceptionHandlerEnum.UseHellang)
{
    if (useExceptionLoggingFilter)
        builder.Services.AddProblemDetails(opts =>
        {
            opts.ShouldLogUnhandledException = (context, exception, details) =>
            {
                return false;
            };
        });
    else
        builder.Services.AddProblemDetails();
}

builder.Services.AddControllers(options =>
{
    options.Filters.Add<LoggingContextFilter>();
    if (useExceptionLoggingFilter)
        options.Filters.Add<ProblemDetailsExceptionFilter>();
});

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


if (exceptionHandler == ExceptionHandlerEnum.UseHellang)
{
    app.UseProblemDetails(); // Add the middleware
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (exceptionHandler == ExceptionHandlerEnum.UseExceptionHandler)
{
    // using this by itself converts to Problem details, but still logs w/o context {application/json; charset=utf-8}
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            ProblemDetails pd;
            if (ex is ProblemDetailsException pde)
            {
                pd = pde.Details;
            }
            else
            {
                pd = new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "An unhandled exception has occurred while executing the request (in UseExceptionHandler)!",
                    Detail = ex?.StackTrace
                };
            }
            await context.Response.WriteAsJsonAsync(pd);
        });
    });
}

// kinda works, but sends it  {application/problem+json; charset=utf-8} encoded and logs w/o context
if (exceptionHandler == ExceptionHandlerEnum.UsePages)
{
    if (app.Environment.IsDevelopment())
    {
        // app.UseDeveloperExceptionPage();
        app.UseExceptionHandler("/error-local-development");
    }
    else
    {
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CorrelationMiddleware>(); // pull correlation from headers in middleware to add to scope

// this changes it to a ProblemDetails, but doesn't log
if (exceptionHandler == ExceptionHandlerEnum.UseMyMiddleWare)
{
    app.UseMiddleware<MyExceptionMiddleware>();
}

app.Run();

#pragma warning restore CA2254
enum ExceptionHandlerEnum
{
    UseExceptionHandler,
    UsePages,
    UseMyMiddleWare,
    UseHellang
};
