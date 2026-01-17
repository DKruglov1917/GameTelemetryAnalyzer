using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Analysis;

public static class AnalyzerRunner
{
    public static TelemetryReport Execute(
        TelemetryRun run,
        RunConfig config,
        AnalyzerSelection selection)
    {
        var executedAnalyses = new HashSet<Type>();
        var results = new List<AnalyzerResult>();

        var analyzers = DiscoverAnalyzers();

        if (selection.Only.Count > 0)
            analyzers = analyzers
                .Where(a => selection.Only.Contains(a.Name))
                .ToList();

        if (selection.Exclude.Count > 0)
            analyzers = analyzers
                .Where(a => !selection.Exclude.Contains(a.Name))
                .ToList();


        foreach (var analyzer in analyzers)
        {
            var result = analyzer.Execute(run.Snapshot, config);

            results.Add(result);
            executedAnalyses.Add(analyzer.GetType());
        }

        return new TelemetryReport(
            executedAnalyses,
            results,
            DateTimeOffset.Parse(run.RunInfo.GeneratedAtUtc),
            ReportType.BeginPlay
        );
    }

    private static List<Analyzer> DiscoverAnalyzers()
    {
        var baseType = typeof(Analyzer);

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
            .Select(t => (Analyzer)Activator.CreateInstance(t)!)
            .ToList();
    }
}