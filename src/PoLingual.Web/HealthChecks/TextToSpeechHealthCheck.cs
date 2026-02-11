using Microsoft.Extensions.Diagnostics.HealthChecks;
using PoLingual.Web.Services.Speech;

namespace PoLingual.Web.HealthChecks;

public class TextToSpeechHealthCheck : IHealthCheck
{
    private readonly ITextToSpeechService _service;
    public TextToSpeechHealthCheck(ITextToSpeechService service) => _service = service;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_service.IsConfigured
            ? HealthCheckResult.Healthy("TTS (Speech SDK) is configured.")
            : HealthCheckResult.Degraded("TTS (Speech SDK) is not configured."));
    }
}
