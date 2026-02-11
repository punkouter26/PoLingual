using Microsoft.CognitiveServices.Speech;

namespace PoLingual.Web.Services.Speech;

/// <summary>
/// Azure Cognitive Services Speech SDK implementation for Text-to-Speech.
/// Used by the Debate feature for rap delivery.
/// </summary>
public class TextToSpeechService : ITextToSpeechService
{
    private readonly SpeechConfig? _speechConfig;
    private readonly ILogger<TextToSpeechService> _logger;
    public bool IsConfigured { get; }

    public TextToSpeechService(IConfiguration configuration, ILogger<TextToSpeechService> logger)
    {
        _logger = logger;
        var speechRegion = configuration["Azure:Speech:Region"];
        var speechSubscriptionKey = configuration["Azure:Speech:SubscriptionKey"];

        if (string.IsNullOrEmpty(speechRegion) || string.IsNullOrEmpty(speechSubscriptionKey))
        {
            _logger.LogWarning("Azure Speech not configured. TTS will be unavailable.");
            IsConfigured = false;
            return;
        }

        _speechConfig = SpeechConfig.FromSubscription(speechSubscriptionKey, speechRegion);
        _speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);
        IsConfigured = true;
        _logger.LogInformation("Azure Speech client initialized for region: {Region}", speechRegion);
    }

    public async Task<byte[]> GenerateSpeechAsync(string text, string voiceName, CancellationToken cancellationToken)
    {
        if (_speechConfig == null)
            throw new InvalidOperationException("Azure Speech service is not configured.");

        _logger.LogInformation("Generating speech for voice: {Voice}", voiceName);
        _speechConfig.SpeechSynthesisVoiceName = voiceName;

        using var synthesizer = new SpeechSynthesizer(_speechConfig, null);
        var synthesisTask = synthesizer.SpeakTextAsync(text);
        var cancellationTask = Task.Delay(Timeout.Infinite, cancellationToken);
        var completedTask = await Task.WhenAny(synthesisTask, cancellationTask);

        if (completedTask == cancellationTask)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        using var result = await synthesisTask;
        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            var audioBytes = result.AudioData;
            if (audioBytes != null && audioBytes.Length > 0)
                return audioBytes;
            throw new InvalidOperationException("No audio data was generated");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var details = SpeechSynthesisCancellationDetails.FromResult(result);
            throw new InvalidOperationException($"Speech synthesis canceled: {details.Reason} - {details.ErrorDetails}");
        }

        throw new InvalidOperationException($"Speech synthesis failed: {result.Reason}");
    }
}
