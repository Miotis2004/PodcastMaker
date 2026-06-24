using PodcastMaker.Core.Configuration;

namespace PodcastMaker.Tests;

public sealed class ConfigurationTests
{
    [Fact]
    public void DefaultOptionsContainSafeDevelopmentValues()
    {
        var options = new PodcastMakerOptions();

        Assert.Equal("http://localhost:11434/", options.OllamaUrl.ToString());
        Assert.Equal("llama3.1", options.DefaultModel);
        Assert.Contains("txt", options.AllowedExportTypes);
        Assert.Contains("md", options.AllowedExportTypes);
    }
}
