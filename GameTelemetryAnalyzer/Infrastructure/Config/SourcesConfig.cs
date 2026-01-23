namespace GameTelemetryAnalyzer.Infrastructure.Config;

public sealed class SourcesConfig
{
    public required SheetsSources Sheets { get; init; }
}

public sealed class SheetsSources
{
    public required string Url { get; init; }
}