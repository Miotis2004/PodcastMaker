using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PodcastMaker.Core.Models;

public class OutlineJsonResponse
{
    [JsonPropertyName("episode title")]
    public string EpisodeTitle { get; set; } = string.Empty;

    [JsonPropertyName("short description")]
    public string ShortDescription { get; set; } = string.Empty;

    [JsonPropertyName("segments")]
    public List<OutlineSegment> Segments { get; set; } = new();
}

public class OutlineSegment
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("purpose of each segment")]
    public string Purpose { get; set; } = string.Empty;

    [JsonPropertyName("estimated duration for each segment")]
    public int EstimatedDuration { get; set; }

    [JsonPropertyName("key talking points")]
    public List<string> TalkingPoints { get; set; } = new();
}
