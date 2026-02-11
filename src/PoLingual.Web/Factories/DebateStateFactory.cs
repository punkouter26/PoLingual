using PoLingual.Shared.Models;
using System.Text;

namespace PoLingual.Web.Factories;

/// <summary>
/// Factory for creating initial DebateState instances.
/// </summary>
public static class DebateStateFactory
{
    public static DebateState CreateEmpty() => new()
    {
        Rapper1 = new Rapper(),
        Rapper2 = new Rapper(),
        Topic = new Topic(),
        DebateTranscript = new StringBuilder(),
        CurrentTurnText = string.Empty,
        ErrorMessage = string.Empty
    };

    public static DebateState CreateForNewDebate(Rapper rapper1, Rapper rapper2, Topic topic, int maxTurns)
    {
        int totalRounds = maxTurns / 2;
        string introText = $"Let the battle begin! {rapper1.Name} vs {rapper2.Name} on '{topic.Title}'. {totalRounds} rounds. Let's go!";
        return new DebateState
        {
            Rapper1 = rapper1,
            Rapper2 = rapper2,
            Topic = topic,
            IsDebateInProgress = true,
            CurrentTurn = 0,
            IsRapper1Turn = true,
            DebateTranscript = new StringBuilder(),
            CurrentTurnText = introText,
            ErrorMessage = string.Empty
        };
    }
}
