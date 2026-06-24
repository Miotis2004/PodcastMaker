namespace PodcastMaker.Core.Health;

public interface IOllamaHealthClient
{
    Task<OllamaHealthStatus> CheckAsync(CancellationToken cancellationToken = default);
}
