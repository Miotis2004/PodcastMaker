using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PodcastMaker.Core.Interfaces;

namespace PodcastMaker.Infrastructure.Ollama;

public class OllamaClient : IOllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OllamaClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var url = configuration["Ollama:Url"] ?? "http://localhost:11434";
        _httpClient.BaseAddress = new Uri(url);
        _model = configuration["Ollama:DefaultModel"] ?? "llama3";
    }

    public async Task<string> GenerateCompletionAsync(string prompt, string systemPrompt = "", CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = _model,
            prompt = prompt,
            system = systemPrompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: cancellationToken);
        return result?.Response ?? string.Empty;
    }

    public async Task<string> GenerateJsonAsync(string prompt, string systemPrompt = "", CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = _model,
            prompt = prompt,
            system = systemPrompt,
            format = "json",
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: cancellationToken);
        return result?.Response ?? string.Empty;
    }

    private class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}
