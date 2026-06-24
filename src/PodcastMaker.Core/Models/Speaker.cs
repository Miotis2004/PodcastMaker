using System;

namespace PodcastMaker.Core.Models;

public class Speaker
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // e.g., Host, Guest
    public string Personality { get; set; } = string.Empty;
    public string SpeakingStyle { get; set; } = string.Empty;
    public string? VoiceId { get; set; }
}
