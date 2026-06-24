using System;

namespace PodcastMaker.Core.Models;

public class GenerationJob
{
    public string Status { get; set; } = "Draft"; // Draft, Queued, Running, Completed, Failed, Cancelled
    public int ProgressPercent { get; set; }
    public string? CurrentMessage { get; set; }
    public string? ErrorDetails { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
