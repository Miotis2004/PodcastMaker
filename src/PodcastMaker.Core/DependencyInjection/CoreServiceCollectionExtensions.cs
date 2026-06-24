using Microsoft.Extensions.DependencyInjection;
using PodcastMaker.Core.Interfaces;
using PodcastMaker.Core.Services;

namespace PodcastMaker.Core.DependencyInjection;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IGenerationService, GenerationService>();
        return services;
    }
}
