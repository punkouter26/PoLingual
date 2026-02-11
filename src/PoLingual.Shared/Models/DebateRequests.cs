using System.Text;
using System.Text.Json.Serialization;

namespace PoLingual.Shared.Models;

public class StartDebateRequest
{
    public required Rapper Rapper1 { get; set; }
    public required Rapper Rapper2 { get; set; }
    public required Topic Topic { get; set; }
}

public class DebateState
{
    public Rapper Rapper1 { get; set; } = new();
    public Rapper Rapper2 { get; set; } = new();
    public Topic Topic { get; set; } = new();
    public bool IsDebateInProgress { get; set; }
    public bool IsDebateFinished { get; set; }
    public int CurrentTurn { get; set; }
    public int CurrentTurnNumber => CurrentTurn;
    public int TotalTurns { get; set; }

    [JsonIgnore]
    public StringBuilder DebateTranscript { get; set; } = new StringBuilder();

    public string DebateTranscriptText
    {
        get => DebateTranscript.ToString();
        set => DebateTranscript = new StringBuilder(value ?? string.Empty);
    }

    public string CurrentTurnText { get; set; } = string.Empty;
    public bool IsRapper1Turn { get; set; }
    public byte[] CurrentTurnAudio { get; set; } = [];
    public string WinnerName { get; set; } = string.Empty;
    public string JudgeReasoning { get; set; } = string.Empty;
    public DebateStats Stats { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsGeneratingTurn { get; set; }
    public RhymeAnalytics? RhymeAnalytics { get; set; }
    public TurnAnalytics? CurrentTurnAnalytics { get; set; }
}
