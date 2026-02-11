using Microsoft.AspNetCore.SignalR;
using PoLingual.Shared.Models;
using PoLingual.Web.Services.Orchestration;

namespace PoLingual.Web.Hubs;

/// <summary>
/// SignalR hub for real-time debate state streaming.
/// Provides methods for starting debates and signaling audio playback completion.
/// </summary>
public class DebateHub : Hub
{
    private readonly IDebateOrchestrator _orchestrator;
    private readonly ILogger<DebateHub> _logger;

    public DebateHub(IDebateOrchestrator orchestrator, ILogger<DebateHub> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        _orchestrator.OnStateChangeAsync += NotifyStateChangeAsync;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        _orchestrator.OnStateChangeAsync -= NotifyStateChangeAsync;
        await base.OnDisconnectedAsync(exception);
    }

    public async Task StartDebate(string rapper1Name, string rapper2Name, string topicTitle)
    {
        _logger.LogInformation("Debate requested: {R1} vs {R2} on {Topic}", rapper1Name, rapper2Name, topicTitle);
        var rapper1 = new Rapper { Name = rapper1Name };
        var rapper2 = new Rapper { Name = rapper2Name };
        var topic = new Topic { Title = topicTitle };
        await _orchestrator.StartNewDebateAsync(rapper1, rapper2, topic);
    }

    public Task AudioPlaybackComplete()
    {
        return _orchestrator.SignalAudioPlaybackCompleteAsync();
    }

    public void ResetDebate()
    {
        _orchestrator.ResetDebate();
    }

    private async Task NotifyStateChangeAsync(DebateState state)
    {
        try
        {
            await Clients.All.SendAsync("DebateStateUpdated", new
            {
                state.CurrentTurn,
                state.IsDebateInProgress,
                state.IsDebateFinished,
                state.IsGeneratingTurn,
                state.IsRapper1Turn,
                state.CurrentTurnText,
                state.WinnerName,
                state.JudgeReasoning,
                state.ErrorMessage,
                Rapper1Name = state.Rapper1.Name,
                Rapper2Name = state.Rapper2.Name,
                TopicTitle = state.Topic.Title,
                HasAudio = state.CurrentTurnAudio is { Length: > 0 },
                AudioBase64 = state.CurrentTurnAudio is { Length: > 0 } ? Convert.ToBase64String(state.CurrentTurnAudio) : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending state update via SignalR.");
        }
    }
}
