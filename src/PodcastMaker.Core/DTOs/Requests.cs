using System;

namespace PodcastMaker.Core.DTOs;

public class CreateEpisodeRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public int LengthMinutes { get; set; }
    public string HostName { get; set; } = "Host";
    public string GuestName { get; set; } = "Guest";
}

public class GenerateOutlineRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public int LengthMinutes { get; set; }
    public string HostName { get; set; } = "Host";
    public string GuestName { get; set; } = "Guest";
}
