using Xunit;
using FluentAssertions;
using GameTelemetryAnalyzer.Analysis;
using GameTelemetryAnalyzer.Domain;
using Tests.TestData;

namespace Tests.Analysis;

public class AnalyzerRunnerTests
{
    [Fact]
    public void Only_selection_should_execute_only_specified_analyzer()
    {
        var run = TestTelemetryRunBuilder.Create().Build();
        var config = TestRunConfigBuilder.Create().Build();

        var selection = new AnalyzerSelection(
            Only: new HashSet<string> { "Reachability" },
            Exclude: new HashSet<string>()
        );

        var report = AnalyzerRunner.Execute(run, config, selection);

        report.ExecutedAnalyses.Should().ContainSingle(a =>
            a.Name.Contains("Reachability")
        );
    }
    
    [Fact]
    public void Exclude_should_skip_specified_analyzer()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithPoi("Radar", AccessType.TwoWayOnFoot, 50_000)
            .Build();

        var config = TestRunConfigBuilder.Create().Build();

        var selection = new AnalyzerSelection(
            Only: new HashSet<string>(),
            Exclude: new HashSet<string> { "Reachability" }
        );

        var report = AnalyzerRunner.Execute(run, config, selection);

        report.ExecutedAnalyses.Should().NotContain(a =>
            a.Name.Contains("Reachability")
        );
    }
    
    [Fact]
    public void Exclude_should_override_only()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithPoi("Radar", AccessType.TwoWayOnFoot, 50_000)
            .Build();

        var config = TestRunConfigBuilder.Create().Build();

        var selection = new AnalyzerSelection(
            Only: new HashSet<string> { "Reachability", "Economy" },
            Exclude: new HashSet<string> { "Reachability" }
        );

        var report = AnalyzerRunner.Execute(run, config, selection);

        report.ExecutedAnalyses.Should().ContainSingle(t =>
            t == typeof(EconomyAnalyzer)
        );
        
        report.ExecutedAnalyses.Should().NotContain(
            typeof(ReachabilityAnalyzer)
        );
    }
    
    [Fact]
    public void Report_should_contain_results_from_executed_analyzers()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithResource("Fuel", 40)
            .Build();

        var config = TestRunConfigBuilder.Create()
            .WithEconomy(new Dictionary<string, ResourceRule>
            {
                ["Fuel"] = new()
                {
                    Recommended = 100,
                    Thresholds = new Thresholds
                    {
                        Warning = 20,
                        Critical = 50
                    }
                }
            })
            .Build();

        var report = AnalyzerRunner.Execute(
            run,
            config,
            AnalyzerSelection.All
        );

        report.Results.Should().NotBeEmpty();
    }
    
    [Fact]
    public void Empty_snapshot_should_not_produce_findings()
    {
        var run = TestTelemetryRunBuilder.Create().Build();
        var config = TestRunConfigBuilder.Create().Build();

        var report = AnalyzerRunner.Execute(
            run,
            config,
            AnalyzerSelection.All
        );

        report.Results.SelectMany(r => r.Findings).Should().BeEmpty();
    }
    
}