using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PodcastMaker.Core.Configuration;
using PodcastMaker.Core.Health;
using PodcastMaker.Infrastructure.Ollama;

namespace PodcastMaker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPodcastMakerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(PodcastMakerOptions.SectionName).Get<PodcastMakerOptions>() ?? new PodcastMakerOptions();

        services.AddHttpClient<IOllamaHealthClient, OllamaHealthClient>(client =>
        {
            client.BaseAddress = options.OllamaUrl;
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
