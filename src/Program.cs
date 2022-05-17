using OptionLoggerTest;
using OptionsLoggerTest.Interfaces;
using OptionsLoggerTest.Services;
using Seekatar.Tools;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.InsertSharedDevSettings();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IOptionsService, OptionsService>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
