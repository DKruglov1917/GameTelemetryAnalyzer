using GameTelemetryAnalyzer.Analysis;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Application;
using GameTelemetryAnalyzer.Infrastructure.Config;
using GameTelemetryAnalyzer.Infrastructure.Notifications;
using GameTelemetryAnalyzer.Infrastructure.Sheets;

namespace GameTelemetryAnalyzer;

//application orchestration
public sealed class TelemetryApp
{
    
    private const string SourcesPath = "Data/sources.json";
    private const string TelemetryPath = "Data/telemetryRun.json";
    
    private readonly HttpClient _http;

    public TelemetryApp(HttpClient http)
    {
        _http = http;
    }

    public async Task Run(string[] args)
    {
        var sources = JsonLoader.Read<SourcesConfig>(SourcesPath, "SourcesConfig");

        var runConfig = await BuildRunConfig(sources);
        var run = JsonLoader.Read<TelemetryRun>(TelemetryPath, "TelemetryRun");
        var options = CliOptions.Parse(args);

        var report = AnalyzerRunner.Execute(
            run,
            runConfig,
            options.ToAnalyzerSelection()
        );

        if (options.SendToDiscord)
            await DiscordNotifier.Notify(report);
    }

    private async Task<RunConfig> BuildRunConfig(SourcesConfig sources)
    {
        var economyRows = await new EconomySheetLoader(_http)
            .Load(sources.Sheets.Economy);

        var economy = RunConfigBuilder.BuildEconomy(economyRows);

        var reachRows = await new ReachabilitySheetLoader(_http)
            .Load(sources.Sheets.Reachability);

        var map = reachRows.ToDictionary(r => r.Key, r => r.Value);
        var reachability = RunConfigBuilder.BuildReachability(map);

        return new RunConfig
        {
            Economy = economy,
            Reachability = reachability
        };
    }
}