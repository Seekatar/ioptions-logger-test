using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OptionLoggerTest;
using OptionsLoggerTest.Interfaces;
using OptionsLoggerTest.Services;
using Seekatar.Tools;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.InsertSharedDevSettings();

#region Add Serilog
builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(builder.Configuration));
#endregion

builder.Services.AddControllers(options => { options.Filters.Add<LoggingContextFilter>(); });
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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var exceptionHandler = ExceptionHanderEnum.UseExceptionHandler;
if (exceptionHandler == ExceptionHanderEnum.UseExceptionHandler)
{
    // using this by itself converts to Problem details, but still logs w/o context {application/json; charset=utf-8}
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            var pd = new ProblemDetails
            {
                Title = "Exception",
                Detail = ex?.Message ?? "Huh?"
            };
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(pd);
        });
    });
}

// kinda works, but sends it  {application/problem+json; charset=utf-8} encoded and logs w/o context
if (exceptionHandler == ExceptionHanderEnum.UsePages) 
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

app.UseMiddleware<CorrelationMiddleware>(); // pull correlation from middleware to add to scope

// this changes it to a ProblemDetails, but doesn't log
if (exceptionHandler == ExceptionHanderEnum.UseMyMiddleWare)
{
    app.UseMiddleware<MyExceptionMiddleware>();
}

app.Run();

#pragma warning restore CA2254
enum ExceptionHanderEnum
{
    UseExceptionHandler,
    UsePages,
    UseMyMiddleWare
};
