using Xunit;
using FluentAssertions;
using GameTelemetryAnalyzer.Analysis;
using GameTelemetryAnalyzer.Domain;
using Tests.TestData;

namespace Tests.Analysis;

public class EconomyAnalyzerTests
{
    [Fact]
    [Trait("Category", "Economy")]
    public void Small_delta_from_recommended_should_trigger_warning()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithResource("Fuel", amount: 120) 
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

        var result = new EconomyAnalyzer()
            .Execute(run.Snapshot, config);

        result.Findings.Should().ContainSingle(f =>
            f.Severity == Severity.Warning &&
            f.Message.Contains("Fuel")
        );
    }

    [Fact]
    [Trait("Category", "Economy")]
    public void Large_delta_from_recommended_should_be_critical()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithResource("Fuel", amount: 170)
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

        var result = new EconomyAnalyzer()
            .Execute(run.Snapshot, config);

        result.Findings.Should().ContainSingle(f =>
            f.Severity == Severity.Critical &&
            f.Message.Contains("Fuel")
        );
    }

    [Fact]
    [Trait("Category", "Economy")]
    public void Very_small_delta_should_not_produce_findings()
    {
        var run = TestTelemetryRunBuilder.Create()
            .WithResource("Fuel", amount: 105) 
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

        var result = new EconomyAnalyzer()
            .Execute(run.Snapshot, config);

        result.Findings.Should().BeEmpty();
    }
}
