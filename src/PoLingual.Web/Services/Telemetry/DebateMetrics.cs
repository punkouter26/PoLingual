using System.Diagnostics.Metrics;

namespace PoLingual.Web.Services.Telemetry;

/// <summary>
/// Custom metrics for debate and translation feature usage tracking.
/// </summary>
public sealed class DebateMetrics
{
    public const string MeterName = "PoLingual";
    private readonly Counter<long> _debatesStarted;
    private readonly Counter<long> _debatesCompleted;
    private readonly Counter<long> _translationsRequested;
    private readonly Counter<long> _ttsRequested;
    private readonly Histogram<double> _debateDuration;
    private readonly Histogram<double> _translationDuration;

    public DebateMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _debatesStarted = meter.CreateCounter<long>("polingual.debates.started", "count", "Number of debates started");
        _debatesCompleted = meter.CreateCounter<long>("polingual.debates.completed", "count", "Number of debates completed");
        _translationsRequested = meter.CreateCounter<long>("polingual.translations.requested", "count", "Number of translations requested");
        _ttsRequested = meter.CreateCounter<long>("polingual.tts.requested", "count", "Number of TTS requests");
        _debateDuration = meter.CreateHistogram<double>("polingual.debates.duration", "ms", "Debate duration in milliseconds");
        _translationDuration = meter.CreateHistogram<double>("polingual.translations.duration", "ms", "Translation duration in milliseconds");
    }

    public void RecordDebateStarted() => _debatesStarted.Add(1);
    public void RecordDebateCompleted(double durationMs)
    {
        _debatesCompleted.Add(1);
        _debateDuration.Record(durationMs);
    }
    public void RecordTranslationRequested(double durationMs)
    {
        _translationsRequested.Add(1);
        _translationDuration.Record(durationMs);
    }
    public void RecordTTSRequested() => _ttsRequested.Add(1);
}
