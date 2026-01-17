namespace GameTelemetryAnalyzer.Infrastructure.Sheets.Rows;

public sealed record ReachabilityRow(
    string Key,
    int Value
);