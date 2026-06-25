using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PodcastMaker.Core.Health;
using PodcastMaker.Infrastructure.Ollama;
using PodcastMaker.Core.Interfaces;
using PodcastMaker.Infrastructure.Storage;

namespace PodcastMaker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IOllamaHealthClient, OllamaHealthClient>(client =>
        {
            var url = configuration["Ollama:Url"] ?? "http://localhost:11434";
            client.BaseAddress = new System.Uri(url);
        });

        if (configuration["PodcastMaker:OllamaUrl"] == "none")
        {
            services.AddSingleton<IOllamaClient, FakeOllamaClient>();
        }
        else
        {
            services.AddHttpClient<IOllamaClient, OllamaClient>();
        }

        services.AddSingleton<IProjectStorage, ProjectStorageService>();

        return services;
    }
}
