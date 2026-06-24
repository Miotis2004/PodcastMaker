using System.Net.Http.Json;
using PodcastMaker.Core.Health;

namespace PodcastMaker.Infrastructure.Ollama;

public sealed class OllamaHealthClient(HttpClient httpClient) : IOllamaHealthClient
{
    public async Task<OllamaHealthStatus> CheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync("/api/version", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new OllamaHealthStatus(false, null, $"Ollama returned {(int)response.StatusCode}.");
            }

            var payload = await response.Content.ReadFromJsonAsync<OllamaVersionResponse>(cancellationToken);
            return new OllamaHealthStatus(true, payload?.Version, null);
        }
        catch (HttpRequestException ex)
        {
            return new OllamaHealthStatus(false, null, ex.Message);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            return new OllamaHealthStatus(false, null, ex.Message);
        }
    }

    private sealed record OllamaVersionResponse(string? Version);
}
