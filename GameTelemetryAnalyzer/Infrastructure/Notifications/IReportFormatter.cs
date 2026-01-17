using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;

public interface IReportFormatter
{
    string Format(TelemetryReport report);
}