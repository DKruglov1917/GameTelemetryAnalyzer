namespace GameTelemetryAnalyzer.Infrastructure.Sheets.Rows;

public sealed record EconomyRow(
    string ResourceId,
    int Recommended,
    int Warning,
    int Critical
);
