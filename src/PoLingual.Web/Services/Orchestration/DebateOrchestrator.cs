using PoLingual.Shared.Models;
using PoLingual.Web.Factories;
using PoLingual.Web.Hubs;
using PoLingual.Web.Services.AI;
using PoLingual.Web.Services.Speech;
using PoLingual.Web.Services.Data;
using PoLingual.Web.Services.Factories;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace PoLingual.Web.Services.Orchestration;

/// <summary>
/// Orchestrates the full lifecycle of a rap debate.
/// Uses IHubContext for SignalR state broadcasting and Factory pattern for scoped service creation.
/// </summary>
public class DebateOrchestrator : IDebateOrchestrator
{
    private readonly ILogger<DebateOrchestrator> _logger;
    private readonly IDebateServiceFactory _serviceFactory;
    private readonly IHubContext<DebateHub> _hubContext;
    private DebateState _currentState;
    public DebateState CurrentState => _currentState;

    private CancellationTokenSource? _debateCancellationTokenSource;
    private TaskCompletionSource<bool> _audioPlaybackCompletionSource;

    private const int MaxDebateTurns = 6;
    private const int MaxTokensPerTurn = 150;
    private const string Rapper1Voice = "en-US-GuyNeural";
    private const string Rapper2Voice = "en-US-JennyNeural";
    private static readonly TimeSpan AudioPlaybackTimeout = TimeSpan.FromSeconds(60);

    public DebateOrchestrator(ILogger<DebateOrchestrator> logger, IDebateServiceFactory serviceFactory, IHubContext<DebateHub> hubContext)
    {
        _logger = logger;
        _serviceFactory = serviceFactory;
        _hubContext = hubContext;
        _currentState = DebateStateFactory.CreateEmpty();
        _audioPlaybackCompletionSource = new TaskCompletionSource<bool>();
    }

    public void ResetDebate()
    {
        _debateCancellationTokenSource?.Cancel();
        _debateCancellationTokenSource?.Dispose();
        _debateCancellationTokenSource = null;
        _audioPlaybackCompletionSource?.TrySetResult(true);
        _audioPlaybackCompletionSource = new TaskCompletionSource<bool>();
        _currentState = DebateStateFactory.CreateEmpty();
        _ = NotifyStateChangeAsync();
    }

    public async Task StartNewDebateAsync(Rapper rapper1, Rapper rapper2, Topic topic)
    {
        ResetDebate();
        _debateCancellationTokenSource = new CancellationTokenSource();
        _currentState = DebateStateFactory.CreateForNewDebate(rapper1, rapper2, topic, MaxDebateTurns);
        await NotifyStateChangeAsync();
        await GenerateIntroductionAsync();
        _logger.LogInformation("Debate started: {R1} vs {R2} on {Topic}", rapper1.Name, rapper2.Name, topic.Title);
        _ = Task.Run(() => RunDebateTurnsAsync(_debateCancellationTokenSource.Token));
    }

    private async Task GenerateIntroductionAsync()
    {
        try
        {
            using var scope = _serviceFactory.CreateScope();
            _currentState.CurrentTurnAudio = await scope.TTSService.GenerateSpeechAsync(_currentState.CurrentTurnText, Rapper1Voice, CancellationToken.None);
            await NotifyStateChangeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating intro speech.");
            _currentState.CurrentTurnAudio = [];
            await NotifyStateChangeAsync();
        }
        _audioPlaybackCompletionSource = new TaskCompletionSource<bool>();
    }

    private async Task RunDebateTurnsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (_currentState.CurrentTurn < MaxDebateTurns && !cancellationToken.IsCancellationRequested)
            {
                using var serviceScope = _serviceFactory.CreateScope();
                _currentState.CurrentTurn++;
                _currentState.IsGeneratingTurn = true;
                _currentState.ErrorMessage = string.Empty;
                await NotifyStateChangeAsync();

                string currentRapper = _currentState.IsRapper1Turn ? _currentState.Rapper1.Name : _currentState.Rapper2.Name;
                string opponent = _currentState.IsRapper1Turn ? _currentState.Rapper2.Name : _currentState.Rapper1.Name;
                string role = _currentState.IsRapper1Turn ? "Pro" : "Con";

                int round = (_currentState.CurrentTurn + 1) / 2;
                int totalRounds = MaxDebateTurns / 2;
                string prompt = $"You are {currentRapper} in a rap battle against {opponent} on '{_currentState.Topic.Title}'. " +
                                $"Role: {role}. Round {round}/{totalRounds}. " +
                                $"IMPORTANT: Your verse MUST be exactly 8 lines or fewer. Do not exceed 8 lines. " +
                                $"Transcript so far:\n{_currentState.DebateTranscript}\nYour rap verse (max 8 lines):";

                try
                {
                    _currentState.CurrentTurnText = await serviceScope.AIService.GenerateDebateTurnAsync(prompt, MaxTokensPerTurn, cancellationToken);
                    _currentState.DebateTranscript.AppendLine($"{currentRapper} (Turn {_currentState.CurrentTurn}):\n{_currentState.CurrentTurnText}\n");
                }
                catch (Exception ex)
                {
                    _currentState.CurrentTurnText = "[Turn skipped due to error]";
                    _currentState.ErrorMessage = $"Error generating rap for {currentRapper}: {ex.Message}";
                }

                try
                {
                    string voice = _currentState.IsRapper1Turn ? Rapper1Voice : Rapper2Voice;
                    _currentState.CurrentTurnAudio = await serviceScope.TTSService.GenerateSpeechAsync(_currentState.CurrentTurnText, voice, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating speech for turn {Turn}.", _currentState.CurrentTurn);
                    _currentState.CurrentTurnAudio = [];
                }

                await NotifyStateChangeAsync();

                // Wait for audio playback with timeout — don't hang if client doesn't respond
                bool hasAudio = _currentState.CurrentTurnAudio is { Length: > 0 };
                if (hasAudio)
                {
                    var completed = await Task.WhenAny(_audioPlaybackCompletionSource.Task, Task.Delay(AudioPlaybackTimeout, cancellationToken));
                    if (completed != _audioPlaybackCompletionSource.Task)
                        _logger.LogWarning("Audio playback timed out for turn {Turn}, continuing.", _currentState.CurrentTurn);
                }

                _audioPlaybackCompletionSource = new TaskCompletionSource<bool>();
                _currentState.IsRapper1Turn = !_currentState.IsRapper1Turn;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                _currentState.IsDebateInProgress = false;
                _currentState.IsGeneratingTurn = true;
                await NotifyStateChangeAsync();

                using var judgeScope = _serviceFactory.CreateScope();
                try
                {
                    var judgeResponse = await judgeScope.AIService.JudgeDebateAsync(
                        _currentState.DebateTranscript.ToString(), _currentState.Rapper1.Name, _currentState.Rapper2.Name, _currentState.Topic.Title, cancellationToken);
                    _currentState.WinnerName = judgeResponse.WinnerName;
                    _currentState.JudgeReasoning = judgeResponse.Reasoning;
                    _currentState.Stats = judgeResponse.Stats;

                    if (!string.IsNullOrEmpty(_currentState.WinnerName) && _currentState.WinnerName != "Error Judging")
                    {
                        string loserName = _currentState.WinnerName == _currentState.Rapper1.Name ? _currentState.Rapper2.Name : _currentState.Rapper1.Name;
                        await judgeScope.RapperRepository.UpdateWinLossRecordAsync(_currentState.WinnerName, loserName);
                    }
                }
                catch (Exception ex)
                {
                    _currentState.WinnerName = "Error Judging";
                    _currentState.JudgeReasoning = $"Error during judging: {ex.Message}";
                }

                _currentState.IsDebateFinished = true;
                _currentState.IsGeneratingTurn = false;
                await NotifyStateChangeAsync();
            }
        }
        catch (OperationCanceledException)
        {
            _currentState.ErrorMessage = "Debate cancelled by user.";
            _currentState.IsDebateInProgress = false;
            _currentState.IsGeneratingTurn = false;
            await NotifyStateChangeAsync();
        }
    }

    public Task SignalAudioPlaybackCompleteAsync()
    {
        _audioPlaybackCompletionSource.TrySetResult(true);
        return Task.CompletedTask;
    }

    private async Task NotifyStateChangeAsync()
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("DebateStateUpdated", new
            {
                _currentState.CurrentTurn,
                _currentState.IsDebateInProgress,
                _currentState.IsDebateFinished,
                _currentState.IsGeneratingTurn,
                _currentState.IsRapper1Turn,
                _currentState.CurrentTurnText,
                CurrentTurnNumber = _currentState.CurrentTurn,
                _currentState.WinnerName,
                _currentState.JudgeReasoning,
                _currentState.ErrorMessage,
                Rapper1Name = _currentState.Rapper1.Name,
                Rapper2Name = _currentState.Rapper2.Name,
                TopicTitle = _currentState.Topic.Title,
                HasAudio = _currentState.CurrentTurnAudio is { Length: > 0 },
                AudioBase64 = _currentState.CurrentTurnAudio is { Length: > 0 } ? Convert.ToBase64String(_currentState.CurrentTurnAudio) : null,
                _currentState.Stats
            });
        }
        catch (Exception ex) { _logger.LogError(ex, "Error sending state update via SignalR."); }
    }
}
