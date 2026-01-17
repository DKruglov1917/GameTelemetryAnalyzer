namespace GameTelemetryAnalyzer.Domain;

public sealed record AnalyzerSelection(
    IReadOnlySet<string> Only,
    IReadOnlySet<string> Exclude)
{
    public static AnalyzerSelection All { get; } =
        new(new HashSet<string>(), new HashSet<string>());
}