using FlowingBot.Core;
using FlowingBot.Core.Infrastructure;
using FlowingBot.Api.Middleware;
using FlowingBot.Api.Filters;
using FlowingBot.Core.Services;
using Serilog;
using Microsoft.EntityFrameworkCore;

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
path = Path.Combine(path, "FlowingBot", "Logs");

if (!Directory.Exists(path))
    Directory.CreateDirectory(path);

path = Path.Combine(path, "log-.log");

Log.Logger = new LoggerConfiguration()
    .Filter.ByExcluding("StartsWith(SourceContext, 'Microsoft.')")
    //.Filter.ByIncludingOnly("StartsWith(SourceContext, 'FlowingBot.')")
    .WriteTo.File(
        path: path,                             // file Path
        rollingInterval: RollingInterval.Day,   // Creates a new file per day
        retainedFileCountLimit: 7,              // Keeps just the 7 more recent files
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

Log.Information("Starting the application...");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Register the LoggingActionFilter for dependency injection
builder.Services.AddScoped<LoggingActionFilter>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructuralServices();

builder.Services.AddDbContext<FlowingBotDbContext>();

var app = builder.Build();

// Apply database migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlowingBotDbContext>();
    Log.Information("Applying database migrations...");
    try
    {
        dbContext.Database.Migrate();
        Log.Information("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while applying database migrations.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add request logging middleware (alternative to base controller approach)
app.UseRequestLogging();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint for container orchestration
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health");

app.Run();
