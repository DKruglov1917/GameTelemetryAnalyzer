using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;

public interface IReportSink
{
    Task Publish(TelemetryReport report);
}