using System.Threading;
using System.Threading.Tasks;

namespace PodcastMaker.Core.Interfaces;

public interface IOllamaClient
{
    Task<string> GenerateCompletionAsync(string prompt, string systemPrompt = "", CancellationToken cancellationToken = default);
    Task<string> GenerateJsonAsync(string prompt, string systemPrompt = "", CancellationToken cancellationToken = default);
}
