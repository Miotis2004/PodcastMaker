using System;
using System.Collections.Generic;
using PodcastMaker.Core.Models;

namespace PodcastMaker.Core.DTOs;

public class ExportTranscriptResponse
{
    public string Format { get; set; } = string.Empty; // txt, md
    public string Content { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class EpisodeDetailsResponse
{
    public Episode Episode { get; set; } = new();
    public GenerationJob Job { get; set; } = new();
    public List<Speaker> Speakers { get; set; } = new();
    public List<SegmentResponse> Segments { get; set; } = new();
}

public class SegmentResponse
{
    public Segment Segment { get; set; } = new();
    public List<DialogueLine> DialogueLines { get; set; } = new();
}
