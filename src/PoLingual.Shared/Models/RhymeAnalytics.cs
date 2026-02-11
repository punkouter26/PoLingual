namespace PoLingual.Shared.Models;

public class RapperRhymeMetrics
{
    public double RhymeDensity { get; set; }
    public double VocabularyRichness { get; set; }
    public double SyllableComplexity { get; set; }
    public double WordComplexity { get; set; }
    public double AlliterationScore { get; set; }
    public int TotalWords { get; set; }
    public int UniqueWords { get; set; }
    public int TotalLines { get; set; }
    public int RhymeCount { get; set; }
    public double AverageWordLength { get; set; }
    public double AverageSyllables { get; set; }
}

public class RhymeAnalytics
{
    public required RapperRhymeMetrics Rapper1Metrics { get; set; }
    public required RapperRhymeMetrics Rapper2Metrics { get; set; }
    public List<TurnAnalytics> TurnByTurnAnalytics { get; set; } = new();

    public static RhymeAnalytics Empty => new()
    {
        Rapper1Metrics = new RapperRhymeMetrics(),
        Rapper2Metrics = new RapperRhymeMetrics()
    };
}

public class TurnAnalytics
{
    public int TurnNumber { get; set; }
    public bool IsRapper1Turn { get; set; }
    public double OverallScore { get; set; }
    public double RhymeDensity { get; set; }
    public double VocabularyRichness { get; set; }
    public int WordCount { get; set; }

    public CrowdReactionType GetReactionType()
    {
        return OverallScore switch
        {
            >= 80 => CrowdReactionType.Fireworks,
            >= 60 => CrowdReactionType.Confetti,
            >= 40 => CrowdReactionType.Cheers,
            _ => CrowdReactionType.ThumbsDown
        };
    }
}

public enum CrowdReactionType
{
    None,
    Cheers,
    Confetti,
    Fireworks,
    ThumbsDown
}
