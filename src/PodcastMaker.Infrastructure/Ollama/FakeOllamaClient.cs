using System.Threading;
using System.Threading.Tasks;
using PodcastMaker.Core.Interfaces;

namespace PodcastMaker.Infrastructure.Ollama;

public class FakeOllamaClient : IOllamaClient
{
    public async Task<string> GenerateCompletionAsync(string prompt, string systemPrompt = "", CancellationToken cancellationToken = default)
    {
        await Task.Delay(500, cancellationToken);
        return "Host: This is a fake transcript generated because Ollama is not running.\nGuest: Yes, that is correct.";
    }

    public async Task<string> GenerateJsonAsync(string prompt, string systemPrompt = "", CancellationToken cancellationToken = default)
    {
        await Task.Delay(500, cancellationToken);
        return @"{
            ""episode title"": ""Fake Episode Title"",
            ""short description"": ""A fake episode for testing"",
            ""segments"": [
                { ""title"": ""Segment 1"", ""purpose"": ""Intro"", ""estimated duration for each segment"": 60, ""key talking points"": [""point 1""] },
                { ""title"": ""Segment 2"", ""purpose"": ""Main"", ""estimated duration for each segment"": 120, ""key talking points"": [""point 2""] }
            ]
        }";
    }
}
