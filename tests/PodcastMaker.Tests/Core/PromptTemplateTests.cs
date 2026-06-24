using NUnit.Framework;
using PodcastMaker.Core.Models;

namespace PodcastMaker.Tests.Core;

[TestFixture]
public class PromptTemplateTests
{
    [Test]
    public void OutlinePrompt_ShouldInjectValues()
    {
        var result = PromptTemplates.OutlinePrompt("Guitars", "Casual", 15, "Alice", "Bob");

        Assert.That(result, Does.Contain("Guitars"));
        Assert.That(result, Does.Contain("Casual"));
        Assert.That(result, Does.Contain("15"));
        Assert.That(result, Does.Contain("Alice"));
        Assert.That(result, Does.Contain("Bob"));
    }
}
