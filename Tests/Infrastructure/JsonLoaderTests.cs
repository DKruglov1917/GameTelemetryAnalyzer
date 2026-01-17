using FluentAssertions;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Infrastructure.Config;
using Xunit;

namespace Tests.Infrastructure;

public class JsonLoaderTests
{
    [Fact]
    public void Should_load_telemetry_run_from_valid_json()
    {
        var run = JsonLoader.Read<TelemetryRun>(
            "TestData/telemetryRun.valid.json",
            "TelemetryRun"
        );

        run.RunInfo.RunId.Should().NotBeNullOrEmpty();
        run.RunInfo.GeneratedAtUtc.Should().NotBeNullOrEmpty();

        run.Snapshot.PoiMetrics.Should().NotBeEmpty();
        run.Snapshot.AvailableResources.Should().NotBeEmpty();

        run.RuntimeEvents.Should().NotBeNull();
        
        run.Snapshot.PoiMetrics
            .Select(p => p.AccessType)
            .Should().Contain(AccessType.OneWayOnFoot);
    }
}