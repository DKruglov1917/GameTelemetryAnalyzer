using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Analysis;

public sealed class EconomyAnalyzer
    : BaselineAnalyzer<ResourceMetric>
{
    public override string Name => "Economy";

    protected override IEnumerable<ResourceMetric> SelectMetrics(
        Snapshot snapshot)
        => snapshot.AvailableResources;

    protected override AnalyzerFinding? AnalyzeMetric(
        ResourceMetric metric,
        RunConfig config)
    {
        if (!config.Economy.Resources.TryGetValue(
                metric.ResourceId,
                out var rule))
        {
            return new AnalyzerFinding(
                Severity.Warning,
                $"Resource '{metric.ResourceId}' has no economy rule"
            );
        }

        float expected = rule.Recommended;
        float actual = metric.Amount;
        float delta = Math.Abs(actual - expected);

        Severity severity =
            rule.Thresholds.Evaluate(delta);

        if (severity == Severity.Ok)
            return null;

        return new AnalyzerFinding(
            severity,
            $"{metric.ResourceId}: amount {actual} deviates from recommended {expected} by {delta}"
        );
    }
}
