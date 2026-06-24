using System;

namespace PodcastMaker.Core.Models;

public class Segment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EpisodeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Transcript { get; set; }
    public int EstimatedDurationSeconds { get; set; }
}
