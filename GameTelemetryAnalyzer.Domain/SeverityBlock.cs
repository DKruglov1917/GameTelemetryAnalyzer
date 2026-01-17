namespace GameTelemetryAnalyzer.Domain;

public class SeverityBlock
{
    public required Severity Severity { get; init; }
    public required IReadOnlyList<string> Items { get; init; }
}