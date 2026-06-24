using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PodcastMaker.Core.Models;
using PodcastMaker.Core.DTOs;

namespace PodcastMaker.Core.Interfaces;

public interface IProjectStorage
{
    Task<List<Episode>> ListEpisodesAsync();
    Task<EpisodeDetailsResponse?> GetEpisodeAsync(Guid episodeId);
    Task SaveEpisodeAsync(EpisodeDetailsResponse episodeDetails);
}
