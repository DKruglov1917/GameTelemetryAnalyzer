using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Analysis;

public abstract class BaselineAnalyzer<TMetric> : Analyzer
{
    public sealed override AnalyzerResult Execute(
        Snapshot snapshot,
        RunConfig config)
    {
        var findings = new List<AnalyzerFinding>();

        foreach (var metric in SelectMetrics(snapshot))
        {
            var finding = AnalyzeMetric(metric, config);
            if (finding != null)
                findings.Add(finding);
        }

        return new AnalyzerResult(Name, findings);
    }

    protected abstract IEnumerable<TMetric> SelectMetrics(
        Snapshot snapshot);

    protected abstract AnalyzerFinding? AnalyzeMetric(
        TMetric metric,
        RunConfig config);
}
