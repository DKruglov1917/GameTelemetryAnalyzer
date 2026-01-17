using System.Text.Json;
using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Analysis;

public sealed class ReachabilityAnalyzer
    : BaselineAnalyzer<PoiMetric>
{
    public override string Name => "Reachability";

    protected override IEnumerable<PoiMetric> SelectMetrics(
        Snapshot snapshot)
        => snapshot.PoiMetrics;

    protected override AnalyzerFinding? AnalyzeMetric(
        PoiMetric metric,
        RunConfig config)
    {
        int expected = config.Reachability
            .AccessBaseline
            .Get(metric.AccessType);

        float actual = metric.DistanceToBase;
        float delta = Math.Abs(actual - expected);

        Severity severity =
            config.Reachability.Thresholds.Evaluate(delta);

        if (severity == Severity.Ok)
            return null;

        return new AnalyzerFinding(
            severity,
            $"{metric.Label}: distance {actual} exceeds baseline {expected} by {delta}"
        );
    }
}
