using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PodcastMaker.Core.DTOs;
using PodcastMaker.Core.Interfaces;
using PodcastMaker.Core.Models;

namespace PodcastMaker.Infrastructure.Storage;

public class ProjectStorageService : IProjectStorage
{
    private readonly string _basePath;

    public ProjectStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "episodes");
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<List<Episode>> ListEpisodesAsync()
    {
        var episodes = new List<Episode>();
        if (!Directory.Exists(_basePath)) return episodes;

        foreach (var dir in Directory.GetDirectories(_basePath))
        {
            var metadataPath = Path.Combine(dir, "metadata.json");
            if (File.Exists(metadataPath))
            {
                var json = await File.ReadAllTextAsync(metadataPath);
                var episode = JsonSerializer.Deserialize<Episode>(json);
                if (episode != null)
                {
                    episodes.Add(episode);
                }
            }
        }

        // Sort descending by CreatedAt
        episodes.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
        return episodes;
    }

    public async Task<EpisodeDetailsResponse?> GetEpisodeAsync(Guid episodeId)
    {
        var dirPath = Path.Combine(_basePath, episodeId.ToString());
        if (!Directory.Exists(dirPath)) return null;

        var fullDataPath = Path.Combine(dirPath, "full_state.json");
        if (File.Exists(fullDataPath))
        {
            var json = await File.ReadAllTextAsync(fullDataPath);
            return JsonSerializer.Deserialize<EpisodeDetailsResponse>(json);
        }

        // Fallback for reading individual files if needed, but we'll try to stick to full_state.json for simplicity in Phase 1
        var metadataPath = Path.Combine(dirPath, "metadata.json");
        if (!File.Exists(metadataPath)) return null;

        var metaJson = await File.ReadAllTextAsync(metadataPath);
        var episode = JsonSerializer.Deserialize<Episode>(metaJson);

        if (episode == null) return null;

        return new EpisodeDetailsResponse
        {
            Episode = episode,
            Job = new GenerationJob { Status = episode.Status }
        };
    }

    public async Task SaveEpisodeAsync(EpisodeDetailsResponse episodeDetails)
    {
        var dirPath = Path.Combine(_basePath, episodeDetails.Episode.Id.ToString());
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var fullDataPath = Path.Combine(dirPath, "full_state.json");
        var metadataPath = Path.Combine(dirPath, "metadata.json");

        var options = new JsonSerializerOptions { WriteIndented = true };

        var fullJson = JsonSerializer.Serialize(episodeDetails, options);
        await File.WriteAllTextAsync(fullDataPath, fullJson);

        var metaJson = JsonSerializer.Serialize(episodeDetails.Episode, options);
        await File.WriteAllTextAsync(metadataPath, metaJson);
    }
}
