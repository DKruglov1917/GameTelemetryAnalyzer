using System.Text;
using System.Text.Json;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Delivery;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;

public static class DiscordNotifier
{
    private const string DeliveryPath = "Data/delivery.json";

    public static async Task Notify(TelemetryReport report)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, DeliveryPath);
        var json = await File.ReadAllTextAsync(fullPath);

        var delivery = JsonSerializer.Deserialize<DeliveryConfig>(json)
                       ?? throw new InvalidOperationException("Delivery config is invalid");

        var discord = delivery.Discord
                      ?? throw new InvalidOperationException("Discord config missing");

        var template = discord.Message
                       ?? throw new InvalidOperationException("Discord message template missing");

        var formatter = new DiscordMessageFormatter();
        var message = formatter.Format(report, template);

        var payload = JsonSerializer.Serialize(new
        {
            content = message
        });

        using var http = new HttpClient();
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var response = await http.PostAsync(discord.WebhookUrl!, content);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Discord error: {response.StatusCode}: {err}");
        }
    }
}