using System;

namespace PodcastMaker.Core.Models;

public class DialogueLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SegmentId { get; set; }
    public string SpeakerName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? AudioPath { get; set; }
    public int SortOrder { get; set; }
}
