using Microsoft.Extensions.Options;
using PodcastMaker.Core.Configuration;
using PodcastMaker.Core.Health;
using PodcastMaker.Core.DependencyInjection;
using PodcastMaker.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins("http://localhost:4200", "http://localhost:4201")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.Configure<PodcastMakerOptions>(builder.Configuration.GetSection(PodcastMakerOptions.SectionName));
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "PodcastMaker.Api",
    timestamp = DateTimeOffset.UtcNow
}));

api.MapGet("/config", (IOptions<PodcastMakerOptions> options) => Results.Ok(new
{
    options.Value.OllamaUrl,
    options.Value.DefaultModel,
    options.Value.ProjectStoragePath,
    options.Value.SQLitePath,
    options.Value.TtsEngine,
    options.Value.FFmpegPath,
    options.Value.AllowedExportTypes
}));

api.MapGet("/health/ollama", async (IOllamaHealthClient ollamaHealthClient, CancellationToken cancellationToken) =>
{
    var status = await ollamaHealthClient.CheckAsync(cancellationToken);
    return status.IsAvailable ? Results.Ok(status) : Results.Problem(status.Error, statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.Run();

public partial class Program;
