using Microsoft.AspNetCore.Mvc;
using PodcastMaker.Core.DTOs;
using PodcastMaker.Core.Interfaces;
using PodcastMaker.Core.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PodcastMaker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EpisodesController : ControllerBase
{
    private readonly IProjectStorage _projectStorage;
    private readonly IGenerationService _generationService;

    public EpisodesController(IProjectStorage projectStorage, IGenerationService generationService)
    {
        _projectStorage = projectStorage;
        _generationService = generationService;
    }

    [HttpGet]
    public async Task<IActionResult> ListEpisodes()
    {
        var episodes = await _projectStorage.ListEpisodesAsync();
        return Ok(episodes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEpisode(Guid id)
    {
        var episodeDetails = await _projectStorage.GetEpisodeAsync(id);
        if (episodeDetails == null) return NotFound();
        return Ok(episodeDetails);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEpisode([FromBody] CreateEpisodeRequest request)
    {
        var episodeId = Guid.NewGuid();

        // Save initial state so the frontend doesn't get a 404 on immediate redirect
        var initialEpisode = new EpisodeDetailsResponse
        {
            Episode = new Episode
            {
                Id = episodeId,
                Topic = request.Topic,
                Style = request.Style,
                LengthMinutes = request.LengthMinutes,
                Title = "Generating Title...",
                Status = "GeneratingOutline"
            },
            Job = new GenerationJob
            {
                Status = "Running",
                StartedAt = DateTime.UtcNow,
                CurrentMessage = "Initializing..."
            }
        };
        await _projectStorage.SaveEpisodeAsync(initialEpisode);

        // Return 202 Accepted and kick off outline generation in background
        _ = Task.Run(() => _generationService.StartOutlineGenerationAsync(episodeId, request));

        return Accepted(new { Id = episodeId, Message = "Outline generation started." });
    }

    [HttpPost("{id:guid}/generate-transcript")]
    public async Task<IActionResult> GenerateTranscript(Guid id)
    {
        var episodeDetails = await _projectStorage.GetEpisodeAsync(id);
        if (episodeDetails == null) return NotFound();

        // Return 202 Accepted and kick off transcript generation in background
        _ = Task.Run(() => _generationService.StartTranscriptGenerationAsync(id));

        return Accepted(new { Id = id, Message = "Transcript generation started." });
    }

    [HttpGet("{id:guid}/export/transcript")]
    public async Task<IActionResult> ExportTranscript(Guid id, [FromQuery] string format = "txt")
    {
        var episodeDetails = await _projectStorage.GetEpisodeAsync(id);
        if (episodeDetails == null) return NotFound();

        var content = new StringBuilder();

        if (format.ToLower() == "md")
        {
            content.AppendLine($"# {episodeDetails.Episode.Title}");
            content.AppendLine($"**Topic:** {episodeDetails.Episode.Topic}");
            content.AppendLine();

            foreach (var segment in episodeDetails.Segments)
            {
                content.AppendLine($"## {segment.Segment.Title}");
                content.AppendLine();

                foreach (var line in segment.DialogueLines)
                {
                    content.AppendLine($"**{line.SpeakerName}:** {line.Text}");
                    content.AppendLine();
                }
            }
        }
        else
        {
            // Plain text
            content.AppendLine(episodeDetails.Episode.Title);
            content.AppendLine($"Topic: {episodeDetails.Episode.Topic}");
            content.AppendLine();

            foreach (var segment in episodeDetails.Segments)
            {
                content.AppendLine($"--- {segment.Segment.Title} ---");
                content.AppendLine();

                foreach (var line in segment.DialogueLines)
                {
                    content.AppendLine($"{line.SpeakerName}: {line.Text}");
                    content.AppendLine();
                }
            }
        }

        var response = new ExportTranscriptResponse
        {
            Format = format,
            Content = content.ToString(),
            Filename = $"{episodeDetails.Episode.Title.Replace(" ", "_")}.{format}"
        };

        return Ok(response);
    }
}
