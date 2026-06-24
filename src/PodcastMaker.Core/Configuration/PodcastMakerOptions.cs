namespace PodcastMaker.Core.Configuration;

public sealed class PodcastMakerOptions
{
    public const string SectionName = "PodcastMaker";

    public Uri OllamaUrl { get; init; } = new("http://localhost:11434");

    public string DefaultModel { get; init; } = "llama3.1";

    public string ProjectStoragePath { get; init; } = "./projects";

    public string SQLitePath { get; init; } = "./data/podcastmaker.db";

    public string TtsEngine { get; init; } = "piper";

    public string FFmpegPath { get; init; } = "ffmpeg";

    public string[] AllowedExportTypes { get; init; } = ["txt", "md"];
}
