using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;

public sealed class ConsoleReportSink : IReportSink
{
    private readonly IReportFormatter _formatter;

    public ConsoleReportSink(IReportFormatter formatter)
    {
        _formatter = formatter;
    }

    public Task Publish(TelemetryReport report)
    {
        var output = _formatter.Format(report);
        Console.WriteLine(output);
        return Task.CompletedTask;
    }
}
