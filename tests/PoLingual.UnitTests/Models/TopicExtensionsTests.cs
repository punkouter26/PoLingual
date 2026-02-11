using PoLingual.Shared.Models;
using PoLingual.Web.Extensions;

namespace PoLingual.UnitTests.Models;

public class TopicExtensionsTests
{
    [Fact]
    public void GetEmoji_Technology_ReturnsComputerEmoji()
    {
        var topic = new Topic { Category = "Technology" };
        topic.GetEmoji().Should().Be("💻");
    }

    [Fact]
    public void GetEmoji_UnknownCategory_ReturnsMicrophone()
    {
        var topic = new Topic { Category = "Unknown" };
        topic.GetEmoji().Should().Be("🎤");
    }

    [Fact]
    public void GetColor_Food_ReturnsOrange()
    {
        var topic = new Topic { Category = "Food" };
        topic.GetColor().Should().Be("#FFA726");
    }

    [Fact]
    public void GetDefaultTopics_ReturnsNonEmptyList()
    {
        var topics = TopicMapperExtensions.GetDefaultTopics();
        topics.Should().NotBeEmpty();
        topics.Should().AllSatisfy(t =>
        {
            t.Title.Should().NotBeNullOrEmpty();
            t.Category.Should().NotBeNullOrEmpty();
        });
    }
}
