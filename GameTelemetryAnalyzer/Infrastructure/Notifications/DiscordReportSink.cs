using System.Text;
using System.Text.Json;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Delivery;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;

public sealed class DiscordReportSink : IReportSink
{
    private readonly DiscordDelivery _config;
    private readonly IReportFormatter _formatter;
    private readonly HttpClient _http;

    public DiscordReportSink(
        DiscordDelivery config,
        IReportFormatter formatter,
        HttpClient http)
    {
        _config = config;
        _formatter = formatter;
        _http = http;
    }

    public async Task Publish(TelemetryReport report)
    {
        if (!_config.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(_config.WebhookUrl))
            throw new InvalidOperationException(
                "Discord delivery is enabled, but WebhookUrl is missing");

        var message = _formatter.Format(report);
        
        // Discord message separation
        message += "\n\u200B";

        var payload = JsonSerializer.Serialize(new { content = message });
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(_config.WebhookUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Discord error: {response.StatusCode}: {err}");
        }
    }
}