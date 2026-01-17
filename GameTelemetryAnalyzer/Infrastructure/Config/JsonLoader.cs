using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTelemetryAnalyzer.Infrastructure.Config;

public static class JsonLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static T Read<T>(string fileName, string description)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, fileName);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException(
                $"{description} file not found: {fullPath}");

        var json = File.ReadAllText(fullPath);

        return JsonSerializer.Deserialize<T>(json, Options)
               ?? throw new InvalidOperationException(
                   $"{description} JSON is invalid");
    }
}