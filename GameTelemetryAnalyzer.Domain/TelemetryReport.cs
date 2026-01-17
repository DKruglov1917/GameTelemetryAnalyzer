namespace GameTelemetryAnalyzer.Domain;

public enum ReportType
{
    BeginPlay
}

public sealed class TelemetryReport
{
    public IReadOnlySet<Type> ExecutedAnalyses { get; }
    public IReadOnlyList<AnalyzerResult> Results { get; }
    public DateTimeOffset RunTimestamp { get; }
    public ReportType ReportType { get; }

    public TelemetryReport(
        IReadOnlySet<Type> executedAnalyses,
        IReadOnlyList<AnalyzerResult> results,
        DateTimeOffset runTimestamp,
        ReportType reportType)
    {
        ExecutedAnalyses = executedAnalyses 
                           ?? throw new ArgumentNullException(nameof(executedAnalyses));

        Results = results 
                  ?? throw new ArgumentNullException(nameof(results));

        RunTimestamp = runTimestamp;
        ReportType = reportType;
    }
}

public sealed class AnalyzerResult
{
    public string AnalyzerId { get; }
    public IReadOnlyList<AnalyzerFinding> Findings { get; }

    public AnalyzerResult(
        string analyzerId,
        IReadOnlyList<AnalyzerFinding> findings)
    {
        if (string.IsNullOrWhiteSpace(analyzerId))
            throw new ArgumentException(
                "AnalyzerId must not be empty",
                nameof(analyzerId));

        if (findings is null)
            throw new ArgumentNullException(nameof(findings));

        AnalyzerId = analyzerId;
        Findings = findings;
    }
}

public sealed class AnalyzerFinding
{
    public Severity Severity { get; }
    public string Message { get; }

    public AnalyzerFinding(Severity severity, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Finding message must not be empty",
                nameof(message));

        Severity = severity;
        Message = message;
    }
}


