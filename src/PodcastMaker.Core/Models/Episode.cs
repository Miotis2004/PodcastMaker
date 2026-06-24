using System;

namespace PodcastMaker.Core.Models;

public class Episode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public int LengthMinutes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Draft"; // Draft, OutlineGenerated, GeneratingTranscript, TranscriptGenerated, Failed
}
