using System.Diagnostics.Metrics;
using PoLingual.Web.Services.Telemetry;

namespace PoLingual.UnitTests.Services;

public class DebateMetricsTests
{
    [Fact]
    public void RecordDebateStarted_DoesNotThrow()
    {
        var meterFactory = new TestMeterFactory();
        var metrics = new DebateMetrics(meterFactory);

        var act = () => metrics.RecordDebateStarted();
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordTranslationRequested_DoesNotThrow()
    {
        var meterFactory = new TestMeterFactory();
        var metrics = new DebateMetrics(meterFactory);

        var act = () => metrics.RecordTranslationRequested(150.0);
        act.Should().NotThrow();
    }

    private sealed class TestMeterFactory : IMeterFactory
    {
        public Meter Create(MeterOptions options) => new(options);
        public void Dispose() { }
    }
}
