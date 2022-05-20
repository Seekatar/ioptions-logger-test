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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IOptionsService, OptionsService>();

#region Add Options
builder.Services.AddOptions<OneTimeOptions>()
        .Bind(builder.Configuration.GetSection(OneTimeOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

builder.Services.AddOptions<MonitoredOptions>()
        .Bind(builder.Configuration.GetSection(MonitoredOptions.SectionName))
        .ValidateDataAnnotations();

builder.Services.AddOptions<SnapshotOptions>()
        .Bind(builder.Configuration.GetSection(SnapshotOptions.SectionName))
        .ValidateDataAnnotations();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
