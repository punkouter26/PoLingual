using PoLingual.Web.Services.AI;
using PoLingual.Web.Services.Speech;
using PoLingual.Web.Services.Data;

namespace PoLingual.Web.Services.Factories;

/// <summary>
/// Factory interface for creating debate-related services with proper DI scoping.
/// </summary>
public interface IDebateServiceFactory
{
    IDebateServiceScope CreateScope();
}

/// <summary>
/// Scoped set of services needed for a debate turn.
/// </summary>
public interface IDebateServiceScope : IDisposable
{
    IAzureOpenAIService AIService { get; }
    ITextToSpeechService TTSService { get; }
    IRapperRepository RapperRepository { get; }
}
