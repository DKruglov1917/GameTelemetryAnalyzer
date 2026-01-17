namespace GameTelemetryAnalyzer;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        using var http = new HttpClient();

        var app = new TelemetryApp(http);
        await app.Run(args);
    }
}