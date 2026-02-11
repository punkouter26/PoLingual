using PoLingual.Shared.Models;
using PoLingual.Web.Factories;

namespace PoLingual.UnitTests.Factories;

public class DebateStateFactoryTests
{
    [Fact]
    public void CreateEmpty_ReturnsDefaultState()
    {
        var state = DebateStateFactory.CreateEmpty();
        state.IsDebateInProgress.Should().BeFalse();
        state.Rapper1.Should().NotBeNull();
        state.Rapper2.Should().NotBeNull();
        state.Topic.Should().NotBeNull();
        state.DebateTranscript.Should().NotBeNull();
    }

    [Fact]
    public void CreateForNewDebate_SetsCorrectState()
    {
        var r1 = new Rapper { Name = "Rapper A" };
        var r2 = new Rapper { Name = "Rapper B" };
        var topic = new Topic { Title = "Test Topic" };

        var state = DebateStateFactory.CreateForNewDebate(r1, r2, topic, 6);

        state.IsDebateInProgress.Should().BeTrue();
        state.Rapper1.Name.Should().Be("Rapper A");
        state.Rapper2.Name.Should().Be("Rapper B");
        state.Topic.Title.Should().Be("Test Topic");
        state.IsRapper1Turn.Should().BeTrue();
        state.CurrentTurn.Should().Be(0);
        state.CurrentTurnText.Should().Contain("Rapper A");
        state.CurrentTurnText.Should().Contain("Rapper B");
    }
}
