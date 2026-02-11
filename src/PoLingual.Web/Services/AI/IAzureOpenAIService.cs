using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.AI;

public interface IAzureOpenAIService
{
    bool IsConfigured { get; }
    Task<string> GenerateDebateTurnAsync(string prompt, int maxTokens, CancellationToken cancellationToken);
    Task<JudgeDebateResponse> JudgeDebateAsync(string debateTranscript, string rapper1Name, string rapper2Name, string topic, CancellationToken cancellationToken);
}
