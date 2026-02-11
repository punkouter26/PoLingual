using PoLingual.Web.Services.AudioSynthesis;
using PoLingual.Web.Services.Telemetry;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for speech synthesis (Victorian English TTS).
/// </summary>
public static class SpeechEndpoints
{
    public static IEndpointRouteBuilder MapSpeechEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/speech").WithTags("Speech").WithOpenApi();

        group.MapPost("/synthesize", async (SpeechRequest request, IAudioSynthesisService audioService, DebateMetrics metrics) =>
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return Results.BadRequest(new { error = "Text is required." });

            metrics.RecordTTSRequested();
            var audioBytes = await audioService.SynthesizeSpeechAsync(request.Text);
            return Results.File(audioBytes, "audio/mpeg", "speech.mp3");
        })
        .WithName("SynthesizeSpeech")
        .WithSummary("Converts text to speech using Azure Speech (Victorian English voice).");

        return endpoints;
    }
}

public record SpeechRequest(string Text);
