using FluentAssertions;
using GameTelemetryAnalyzer.Analysis;
using GameTelemetryAnalyzer.Domain;
using Tests.TestData;
using Xunit;

namespace Tests.Analysis;

public class ReachabilityAnalyzerTests
{
    [Fact]
    public void Poi_too_far_should_produce_critical_finding()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithPoi(
                label: "Radar",
                access: AccessType.TwoWayOnFoot,
                distance: 50_000
            )
            .Build();

        var config = TestRunConfigBuilder.Create()
            .WithReachability(
                oneWayOnFoot: 5_000,
                twoWayOnFoot: 10_000,
                twoWayByCar: 20_000,
                warning: 20_000,
                critical: 30_000
            )
            .Build();

        var analyzer = new ReachabilityAnalyzer();

        var result = analyzer.Execute(run.Snapshot, config);

        result.Findings.Should().ContainSingle(f =>
            f.Severity == Severity.Critical &&
            f.Message.Contains("Radar")
        );
    }
    
    [Fact]
    public void Poi_within_baseline_should_not_produce_findings()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithPoi(
                label: "Radar",
                access: AccessType.TwoWayOnFoot,
                distance: 8_000
            )
            .Build();

        var config = TestRunConfigBuilder.Create()
            .WithReachability(
                oneWayOnFoot: 5_000,
                twoWayOnFoot: 10_000,
                twoWayByCar: 20_000,
                warning: 15_000,
                critical: 25_000
            )
            .Build();

        var analyzer = new ReachabilityAnalyzer();

        var result = analyzer.Execute(run.Snapshot, config);

        result.Findings.Should().BeEmpty();
    }
}