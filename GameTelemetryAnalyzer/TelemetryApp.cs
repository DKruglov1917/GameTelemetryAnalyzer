using GameTelemetryAnalyzer.Analysis;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Application;
using GameTelemetryAnalyzer.Delivery;
using GameTelemetryAnalyzer.Infrastructure.Config;
using GameTelemetryAnalyzer.Infrastructure.Notifications;
using GameTelemetryAnalyzer.Infrastructure.Sheets;

namespace GameTelemetryAnalyzer;

//application orchestration
public sealed class TelemetryApp
{
    
    private const string SourcesPath = "Data/sources.json";
    private const string TelemetryPath = "Data/telemetryRun.json";
    private const string DeliveryPath = "Data/delivery.json";
    
    private readonly HttpClient _http;

    public TelemetryApp(HttpClient http)
    {
        _http = http;
    }

    public async Task Run(string[] args)
    {
        var options = CliOptions.Parse(args);

        var sources = JsonLoader.Read<SourcesConfig>(
            SourcesPath,
            "Sources");

        var runConfig = await BuildRunConfig(sources);

        var telemetry = JsonLoader.Read<TelemetryRun>(
            TelemetryPath,
            "TelemetryRun");

        var report = AnalyzerRunner.Execute(
            telemetry,
            runConfig,
            options.ToAnalyzerSelection());

        var sinks = BuildReportSinks(options);

        foreach (var sink in sinks)
        {
            await sink.Publish(report);
        }
    }
    
    private IReadOnlyList<IReportSink> BuildReportSinks(CliOptions options)
    {
        var sinks = new List<IReportSink>();

        var delivery = JsonLoader.Read<DeliveryConfig>(
            DeliveryPath,
            "DeliveryConfig");

        var discordConfig = delivery.Discord
                            ?? throw new InvalidOperationException("Discord config missing");

        var formatter = new TextReportFormatter(
            discordConfig.Message
            ?? throw new InvalidOperationException("Discord message template missing")
        );

        if (options.PrintToConsole)
        {
            sinks.Add(new ConsoleReportSink(formatter));
        }

        if (options.SendToDiscord)
        {
            sinks.Add(new DiscordReportSink(
                discordConfig,
                formatter,
                _http
            ));
        }

        return sinks;
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