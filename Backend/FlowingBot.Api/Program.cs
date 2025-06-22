using FlowingBot.Core;
using FlowingBot.Core.Infrastructure;
using Serilog;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructuralServices();

builder.Services.AddDbContext<FlowingBotDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
