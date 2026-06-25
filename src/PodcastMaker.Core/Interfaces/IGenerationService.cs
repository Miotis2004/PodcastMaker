using System;
using System.Threading;
using System.Threading.Tasks;
using PodcastMaker.Core.DTOs;

namespace PodcastMaker.Core.Interfaces;

public interface IGenerationService
{
    Task StartOutlineGenerationAsync(Guid episodeId, CreateEpisodeRequest request, CancellationToken cancellationToken = default);
    Task StartTranscriptGenerationAsync(Guid episodeId, CancellationToken cancellationToken = default);
    Task StartSegmentRegenerationAsync(Guid episodeId, Guid segmentId, CancellationToken cancellationToken = default);
}
