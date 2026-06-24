using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PodcastMaker.Core.DTOs;
using PodcastMaker.Core.Interfaces;
using PodcastMaker.Core.Models;
using PodcastMaker.Core.Services;

namespace PodcastMaker.Tests.Core;

[TestFixture]
public class GenerationServiceTests
{
    private Mock<IOllamaClient> _ollamaClientMock;
    private Mock<IProjectStorage> _projectStorageMock;
    private GenerationService _service;

    [SetUp]
    public void Setup()
    {
        _ollamaClientMock = new Mock<IOllamaClient>();
        _projectStorageMock = new Mock<IProjectStorage>();
        _service = new GenerationService(_ollamaClientMock.Object, _projectStorageMock.Object, NullLogger<GenerationService>.Instance);
    }

    [Test]
    public async Task StartOutlineGenerationAsync_ShouldSaveParsedSegments()
    {
        var episodeId = Guid.NewGuid();
        var request = new CreateEpisodeRequest { Topic = "Test" };
        var fakeJsonResponse = @"
        {
            ""episode title"": ""Test Episode"",
            ""short description"": ""A test"",
            ""segments"": [
                {
                    ""title"": ""Intro"",
                    ""purpose of each segment"": ""Introduce topic"",
                    ""estimated duration for each segment"": 60,
                    ""key talking points"": [""Hello""]
                }
            ]
        }";

        _ollamaClientMock.Setup(c => c.GenerateJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeJsonResponse);

        EpisodeDetailsResponse savedState = null;
        _projectStorageMock.Setup(s => s.SaveEpisodeAsync(It.IsAny<EpisodeDetailsResponse>()))
            .Callback<EpisodeDetailsResponse>(state => savedState = state);

        await _service.StartOutlineGenerationAsync(episodeId, request);

        Assert.That(savedState, Is.Not.Null);
        Assert.That(savedState.Episode.Title, Is.EqualTo("Test Episode"));
        Assert.That(savedState.Segments.Count, Is.EqualTo(1));
        Assert.That(savedState.Segments[0].Segment.Title, Is.EqualTo("Intro"));
        Assert.That(savedState.Episode.Status, Is.EqualTo("OutlineGenerated"));
    }

    [Test]
    public async Task StartTranscriptGenerationAsync_ShouldParseDialogueLines()
    {
        var episodeId = Guid.NewGuid();
        var episodeDetails = new EpisodeDetailsResponse
        {
            Episode = new Episode { Id = episodeId, Title = "Test" },
            Speakers = new System.Collections.Generic.List<Speaker> { new Speaker { Role = "Host", Name = "Host" } },
            Segments = new System.Collections.Generic.List<SegmentResponse>
            {
                new SegmentResponse { Segment = new Segment { Id = Guid.NewGuid(), Title = "Seg1" } }
            }
        };

        _projectStorageMock.Setup(s => s.GetEpisodeAsync(episodeId)).ReturnsAsync(episodeDetails);

        var fakeTranscript = @"
Host: Hello there!
Guest: Hi! How are you?
This is a continuation line.
Host: Good.
";
        _ollamaClientMock.Setup(c => c.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeTranscript);

        EpisodeDetailsResponse savedState = null;
        _projectStorageMock.Setup(s => s.SaveEpisodeAsync(It.IsAny<EpisodeDetailsResponse>()))
            .Callback<EpisodeDetailsResponse>(state => savedState = state);

        await _service.StartTranscriptGenerationAsync(episodeId);

        Assert.That(savedState, Is.Not.Null);
        var lines = savedState.Segments[0].DialogueLines;

        Assert.That(lines.Count, Is.EqualTo(3));
        Assert.That(lines[0].SpeakerName, Is.EqualTo("Host"));
        Assert.That(lines[0].Text, Is.EqualTo("Hello there!"));
        Assert.That(lines[1].SpeakerName, Is.EqualTo("Guest"));
        Assert.That(lines[1].Text, Is.EqualTo("Hi! How are you? This is a continuation line."));
        Assert.That(lines[2].SpeakerName, Is.EqualTo("Host"));
        Assert.That(lines[2].Text, Is.EqualTo("Good."));

        Assert.That(savedState.Episode.Status, Is.EqualTo("TranscriptGenerated"));
    }
}
