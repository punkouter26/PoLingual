namespace PoLingual.Web.Services.AudioSynthesis;

public interface IAudioSynthesisService
{
    Task<byte[]> SynthesizeSpeechAsync(string text, CancellationToken cancellationToken = default);
}
