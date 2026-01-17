namespace GameTelemetryAnalyzer.Domain;

public class AnalysisSection
{
    public required string Title { get; init; }
    public required IReadOnlyList<SeverityBlock> Blocks { get; init; }
}