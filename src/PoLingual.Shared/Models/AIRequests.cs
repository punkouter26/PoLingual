namespace PoLingual.Shared.Models;

public class GenerateDebateTurnRequest
{
    public required string Prompt { get; set; }
    public int MaxTokens { get; set; }
}

public class JudgeDebateRequest
{
    public required string DebateTranscript { get; set; }
    public required string Rapper1Name { get; set; }
    public required string Rapper2Name { get; set; }
    public required string Topic { get; set; }
}

public class JudgeDebateResponse
{
    public required string WinnerName { get; set; }
    public required string Reasoning { get; set; }
    public required DebateStats Stats { get; set; }
}

public class GenerateSpeechRequest
{
    public required string Text { get; set; }
    public required string VoiceName { get; set; }
}
