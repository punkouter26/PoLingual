namespace PoLingual.Web.Services.Speech;

public interface ITextToSpeechService
{
    bool IsConfigured { get; }
    Task<byte[]> GenerateSpeechAsync(string text, string voiceName, CancellationToken cancellationToken);
}
