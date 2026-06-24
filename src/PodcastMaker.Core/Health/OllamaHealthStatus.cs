namespace PodcastMaker.Core.Health;

public sealed record OllamaHealthStatus(bool IsAvailable, string? Version, string? Error);
