using IOptionTest;
using IOptionTest.Interfaces;
using IOptionTest.Options;
using IOptionTest.Services;
using IOptionTest.Auth;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Seekatar.Tools;
using Serilog;
using System.Net;
using static IOptionTest.Options.ExceptionOptions;
using static IOptionTest.Auth.AuthConstants;

ExceptionHandler = ExceptionHandlerEnum.DotNet7;
bool useExceptionLoggingFilter = false; // this will cause some redundant logging, but you get the idea

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.InsertSharedDevSettings();

#region Add Serilog
builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(builder.Configuration));
#endregion

if (ExceptionHandler == ExceptionHandlerEnum.UseHellang)
{
    if (useExceptionLoggingFilter)
        Hellang.Middleware.ProblemDetails.ProblemDetailsExtensions.AddProblemDetails(builder.Services, opts =>
        {
            opts.ShouldLogUnhandledException = (context, exception, details) =>
            {
                return false;
            };
        });
    else
    {
        Hellang.Middleware.ProblemDetails.ProblemDetailsExtensions.AddProblemDetails(builder.Services);
    }
}
else if (ExceptionHandler == ExceptionHandlerEnum.DotNet7)
{
    builder.Services.AddProblemDetails();
    // setting CustomizeProblemDetails breaks our middleware
    //builder.Services.AddProblemDetails( opt => {
    //    opt.CustomizeProblemDetails = (problemDetailsCtx) =>
    //    {
    //        Console.WriteLine("Hi");
    //        problemDetailsCtx.AdditionalMetadata.Append(999);
    //        problemDetailsCtx.ProblemDetails.Type = "set in customproblemdetails";
    //    };

    //});
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

#region Add Auth Test Services
if (args.Length > 0 && string.Equals(args[0], "forward", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Using Forwarding Auth");
    AddForardingAuth(builder);
}
else 
{
    string? defaultScheme = (args.Length > 0 && args[0].StartsWith("Scheme")) ? args[0] : "";
    Console.WriteLine($"Using default scheme of '{defaultScheme}'");
    AddAuth(builder, defaultScheme);
}
#endregion

var app = builder.Build();


if (ExceptionHandler == ExceptionHandlerEnum.UseHellang)
{
    Hellang.Middleware.ProblemDetails.ProblemDetailsExtensions.UseProblemDetails(app); // Add the middleware
}
else if (ExceptionHandler == ExceptionHandlerEnum.DotNet7)
{
    // add default exception handler
    app.UseExceptionHandler();

    // this returns problemDetails if caller sets accept to application/json for responses with status codes between 400 and 599 that do not have a body
    app.UseStatusCodePages();

    // add our middleware to call WriteAsync so we get contents or a ProblemDetailsException instead of 500
    //app.UseMiddleware<Seekatar.ProblemDetails.ProblemDetailsMiddleware>();

    //if (app.Environment.IsDevelopment())
    //    app.UseDeveloperExceptionPage(); // with this dumps all details, including stack, otherwise this just get a 500
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (ExceptionHandler == ExceptionHandlerEnum.UseExceptionHandler)
{
    // using this by itself converts to Problem details, but still logs w/o context {application/json; charset=utf-8}
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            ProblemDetails pd;
            if (ex is Hellang.Middleware.ProblemDetails.ProblemDetailsException pde)
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
if (ExceptionHandler == ExceptionHandlerEnum.UsePages)
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

app.UseMiddleware<AuthLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CorrelationMiddleware>(); // pull correlation from headers in middleware to add to scope

// this changes it to a ProblemDetails, but doesn't log
if (ExceptionHandler == ExceptionHandlerEnum.UseMyMiddleWare)
{
    app.UseMiddleware<MyExceptionMiddleware>();
}
app.Run();

#pragma warning restore CA2254

static void AddAuth(WebApplicationBuilder builder, string defaultScheme)
{
    builder.Services
        .AddAuthentication(defaultScheme)
        .AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(SchemeA, options => options.Name = NameClaimA)
        .AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(SchemeB, options => options.Name = NameClaimB)
        .AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(SchemeC, options => options.Name = NameClaimC);

    builder.Services.AddAuthorization(options =>
    {
        // UserA and RoleA required
        options.AddPolicy(PolicyA, policy =>
            {
                policy.AddAuthenticationSchemes(SchemeA)
                      .RequireRole(RoleA);
            });
        // UserB required, no scheme specified here so must be specified in [Authorize] attribute if no default
        options.AddPolicy(PolicyB, policy =>
            {
                policy.RequireRole(RoleB);
            });
        options.AddPolicy(PolicyAorB, policy =>
            {
                policy.AddAuthenticationSchemes(SchemeA, SchemeB)
                      .RequireRole(RoleA, RoleB);
            });
        // UserA,B,C any role
        options.AddPolicy(PolicyAnyRole, policy =>
        {
            policy.RequireAuthenticatedUser() // needed 
              .AddAuthenticationSchemes(SchemeA, SchemeB, SchemeC);
        });
        // UserA and RoleC required
        options.AddPolicy(PolicyUserAandRoleC, policy =>
        {
            policy.AddAuthenticationSchemes(SchemeA)
              .RequireRole(RoleC);
        });
    });
}

static void AddForardingAuth(WebApplicationBuilder builder)
{
    builder.Services
        .AddAuthentication(SchemeForwarding)
        .AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(SchemeA, options => options.Name = NameClaimA)
        .AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(SchemeB, options => options.Name = NameClaimB)
        .AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(SchemeC, options => options.Name = NameClaimC)
        .AddPolicyScheme(SchemeForwarding, SchemeForwarding, options =>
        {
            options.ForwardDefaultSelector = (context) =>
            {
                var user = context.Request.Headers["X-Test-User"].ElementAtOrDefault(0);
                var scheme = SchemeA; // default
                if (user is not null && user.StartsWith("User") && user.Length > 4 && user[4] is >= 'A' and <= 'C')
                {
                    scheme = $"Scheme{user[4..]}";
                }
                Console.WriteLine($"ForwardSelectorFromUser returning {scheme}");
                return scheme;
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        // UserA and RoleA required
        options.AddPolicy(PolicyA, policy =>
            {
                policy.RequireRole(RoleA);
            });
        // UserB required, no scheme specified here so must be specified in [Authorize] attribute if no default
        options.AddPolicy(PolicyB, policy =>
            {
                policy.RequireRole(RoleB);
            });
        // UserA or UserB required in RoleA or RoleB
        options.AddPolicy(PolicyAorB, policy =>
            {
                policy.AddAuthenticationSchemes(SchemeA, SchemeB)
                      .RequireRole(RoleA, RoleB);
            });
        // UserA,B,C any role
        options.AddPolicy(PolicyAnyRole, policy =>
        {
            policy.RequireAuthenticatedUser();
        });
        // UserA and RoleC required
        options.AddPolicy(PolicyUserAandRoleC, policy =>
        {
            policy.AddAuthenticationSchemes(SchemeA)
                  .RequireRole(RoleC);
        });
    });
}

