using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PodcastMaker.Core.DTOs;
using PodcastMaker.Core.Interfaces;
using PodcastMaker.Core.Models;

namespace PodcastMaker.Core.Services;

public class GenerationService : IGenerationService
{
    private readonly IOllamaClient _ollamaClient;
    private readonly IProjectStorage _projectStorage;
    private readonly ILogger<GenerationService> _logger;

    public GenerationService(IOllamaClient ollamaClient, IProjectStorage projectStorage, ILogger<GenerationService> logger)
    {
        _ollamaClient = ollamaClient;
        _projectStorage = projectStorage;
        _logger = logger;
    }

    public async Task StartOutlineGenerationAsync(Guid episodeId, CreateEpisodeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting outline generation for episode {EpisodeId}", episodeId);

        var episodeDetails = new EpisodeDetailsResponse
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
                CurrentMessage = "Generating outline..."
            },
            Speakers = new List<Speaker>
            {
                new Speaker { Name = request.HostName, Role = "Host", Personality = "Friendly", SpeakingStyle = "Casual" },
                new Speaker { Name = request.GuestName, Role = "Guest", Personality = "Knowledgeable", SpeakingStyle = "Informative" }
            }
        };

        await _projectStorage.SaveEpisodeAsync(episodeDetails);

        try
        {
            var prompt = PromptTemplates.OutlinePrompt(request.Topic, request.Style, request.LengthMinutes, request.HostName, request.GuestName);
            var jsonResponse = await _ollamaClient.GenerateJsonAsync(prompt, cancellationToken: cancellationToken);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var outline = JsonSerializer.Deserialize<OutlineJsonResponse>(jsonResponse, options);

            if (outline != null)
            {
                episodeDetails.Episode.Title = outline.EpisodeTitle;

                int order = 1;
                foreach (var seg in outline.Segments)
                {
                    episodeDetails.Segments.Add(new SegmentResponse
                    {
                        Segment = new Segment
                        {
                            EpisodeId = episodeId,
                            Title = seg.Title,
                            Purpose = seg.Purpose,
                            EstimatedDurationSeconds = seg.EstimatedDuration,
                            SortOrder = order++
                        }
                    });
                }
            }

            episodeDetails.Episode.Status = "OutlineGenerated";
            episodeDetails.Job.Status = "Completed";
            episodeDetails.Job.CompletedAt = DateTime.UtcNow;
            episodeDetails.Job.CurrentMessage = "Outline generated successfully";

            await _projectStorage.SaveEpisodeAsync(episodeDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate outline for episode {EpisodeId}", episodeId);
            episodeDetails.Episode.Status = "Failed";
            episodeDetails.Job.Status = "Failed";
            episodeDetails.Job.ErrorDetails = ex.Message;
            await _projectStorage.SaveEpisodeAsync(episodeDetails);
        }
    }

    public async Task StartTranscriptGenerationAsync(Guid episodeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting transcript generation for episode {EpisodeId}", episodeId);

        var episodeDetails = await _projectStorage.GetEpisodeAsync(episodeId);
        if (episodeDetails == null)
        {
            _logger.LogWarning("Episode {EpisodeId} not found", episodeId);
            return;
        }

        episodeDetails.Episode.Status = "GeneratingTranscript";
        episodeDetails.Job.Status = "Running";
        episodeDetails.Job.StartedAt = DateTime.UtcNow;
        episodeDetails.Job.CurrentMessage = "Generating transcript...";
        await _projectStorage.SaveEpisodeAsync(episodeDetails);

        try
        {
            var host = episodeDetails.Speakers.FirstOrDefault(s => s.Role == "Host")?.Name ?? "Host";
            var guest = episodeDetails.Speakers.FirstOrDefault(s => s.Role == "Guest")?.Name ?? "Guest";

            string previousSummary = "This is the start of the episode.";

            for (int i = 0; i < episodeDetails.Segments.Count; i++)
            {
                var segmentResponse = episodeDetails.Segments[i];
                var segment = segmentResponse.Segment;

                episodeDetails.Job.CurrentMessage = $"Generating segment {i + 1} of {episodeDetails.Segments.Count}: {segment.Title}";
                episodeDetails.Job.ProgressPercent = (int)((double)i / episodeDetails.Segments.Count * 100);
                await _projectStorage.SaveEpisodeAsync(episodeDetails);

                var prompt = PromptTemplates.DialogueSegmentPrompt(
                    episodeDetails.Episode.Title,
                    episodeDetails.Episode.Topic,
                    segment.Title,
                    segment.Purpose,
                    previousSummary,
                    host,
                    guest);

                var transcript = await _ollamaClient.GenerateCompletionAsync(prompt, cancellationToken: cancellationToken);

                segment.Transcript = transcript;
                segmentResponse.DialogueLines = ParseTranscript(segment.Id, transcript, host, guest);

                previousSummary = $"In the previous segment '{segment.Title}', the speakers discussed: {segment.Purpose}";

                // Save partial progress
                await _projectStorage.SaveEpisodeAsync(episodeDetails);
            }

            episodeDetails.Episode.Status = "TranscriptGenerated";
            episodeDetails.Job.Status = "Completed";
            episodeDetails.Job.ProgressPercent = 100;
            episodeDetails.Job.CompletedAt = DateTime.UtcNow;
            episodeDetails.Job.CurrentMessage = "Transcript generated successfully";
            await _projectStorage.SaveEpisodeAsync(episodeDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate transcript for episode {EpisodeId}", episodeId);
            episodeDetails.Episode.Status = "Failed";
            episodeDetails.Job.Status = "Failed";
            episodeDetails.Job.ErrorDetails = ex.Message;
            await _projectStorage.SaveEpisodeAsync(episodeDetails);
        }
    }

    private List<DialogueLine> ParseTranscript(Guid segmentId, string transcript, string hostName, string guestName)
    {
        var lines = new List<DialogueLine>();
        var stringLines = transcript.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int order = 1;
        foreach (var line in stringLines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            var colonIndex = trimmedLine.IndexOf(':');
            if (colonIndex > 0)
            {
                var speaker = trimmedLine.Substring(0, colonIndex).Trim();
                // Strip asterisks if AI generated something like **Host**:
                speaker = speaker.Replace("*", "");

                var text = trimmedLine.Substring(colonIndex + 1).Trim();

                lines.Add(new DialogueLine
                {
                    SegmentId = segmentId,
                    SpeakerName = speaker,
                    Text = text,
                    SortOrder = order++
                });
            }
            else if (lines.Count > 0)
            {
                // Append to previous line if no colon
                lines.Last().Text += " " + trimmedLine;
            }
        }

        return lines;
    }
}
