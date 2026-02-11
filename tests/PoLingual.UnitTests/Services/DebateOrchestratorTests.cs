using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using PoLingual.Shared.Models;
using PoLingual.Web.Hubs;
using PoLingual.Web.Services.AI;
using PoLingual.Web.Services.Data;
using PoLingual.Web.Services.Factories;
using PoLingual.Web.Services.Orchestration;
using PoLingual.Web.Services.Speech;

namespace PoLingual.UnitTests.Services;

public class DebateOrchestratorTests
{
    private readonly Mock<IDebateServiceFactory> _factoryMock = new();
    private readonly Mock<IHubContext<DebateHub>> _hubContextMock = new();
    private readonly DebateOrchestrator _sut;

    public DebateOrchestratorTests()
    {
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        _hubContextMock.Setup(h => h.Clients).Returns(mockClients.Object);

        _sut = new DebateOrchestrator(NullLogger<DebateOrchestrator>.Instance, _factoryMock.Object, _hubContextMock.Object);
    }

    [Fact]
    public void CurrentState_Initially_IsEmpty()
    {
        var state = _sut.CurrentState;
        state.IsDebateInProgress.Should().BeFalse();
        state.IsDebateFinished.Should().BeFalse();
        state.CurrentTurn.Should().Be(0);
    }

    [Fact]
    public void ResetDebate_ResetsState()
    {
        _sut.ResetDebate();
        var state = _sut.CurrentState;
        state.IsDebateInProgress.Should().BeFalse();
        state.CurrentTurn.Should().Be(0);
    }

    [Fact]
    public void SignalAudioPlaybackComplete_CompletesWithoutError()
    {
        var task = _sut.SignalAudioPlaybackCompleteAsync();
        task.IsCompleted.Should().BeTrue();
    }
}
