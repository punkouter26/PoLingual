namespace PoLingual.Shared.Models;

/// <summary>
/// Holds numerical statistics for a completed debate, as determined by the AI judge.
/// </summary>
public class DebateStats
{
    public int Rapper1LogicScore { get; set; }
    public int Rapper1SentimentScore { get; set; }
    public int Rapper1AdherenceScore { get; set; }
    public int Rapper1RebuttalScore { get; set; }
    public int Rapper2LogicScore { get; set; }
    public int Rapper2SentimentScore { get; set; }
    public int Rapper2AdherenceScore { get; set; }
    public int Rapper2RebuttalScore { get; set; }
    public int Rapper1TotalScore { get; set; }
    public int Rapper2TotalScore { get; set; }

    public static DebateStats Empty => new DebateStats();
}
