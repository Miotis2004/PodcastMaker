using System;
using System.Collections.Generic;
using PodcastMaker.Core.Models;

namespace PodcastMaker.Core.DTOs;

public class CreateEpisodeRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public int LengthMinutes { get; set; }
    public List<Speaker> Speakers { get; set; } = new();
}

public class GenerateOutlineRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public int LengthMinutes { get; set; }
    public List<Speaker> Speakers { get; set; } = new();
}
