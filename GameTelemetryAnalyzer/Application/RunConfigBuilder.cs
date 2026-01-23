using System.Globalization;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Infrastructure.Sheets;

namespace GameTelemetryAnalyzer.Application;

public static class RunConfigBuilder
{
    public static RunConfig Build(SectionSheet sheet)
    {
        var economy = BuildEconomy(
            sheet.Require("ECONOMY_RESOURCES_BASE"),
            sheet.Require("ECONOMY_RESOURCES_THRESHOLDS")
        );

        var reachMap = new Dictionary<string, int>();

        foreach (var kv in sheet.Require("REACHABILITY"))
            reachMap[kv.Key] = int.Parse(kv.Value, CultureInfo.InvariantCulture);

        foreach (var kv in sheet.Require("DANGER_LEVEL"))
            reachMap[kv.Key] = int.Parse(kv.Value, CultureInfo.InvariantCulture);

        var reachability = BuildReachability(reachMap);

        return new RunConfig
        {
            Economy = economy,
            Reachability = reachability
        };
    }
    
    private static Economy BuildEconomy(
        IReadOnlyDictionary<string, string> baseResources,
        IReadOnlyDictionary<string, string> thresholds)
    {
        if (!thresholds.TryGetValue("Threshold.Warning", out var warnRaw))
            throw new InvalidOperationException("Missing Threshold.Warning");

        if (!thresholds.TryGetValue("Threshold.Critical", out var critRaw))
            throw new InvalidOperationException("Missing Threshold.Critical");

        var warning  = float.Parse(warnRaw, CultureInfo.InvariantCulture);
        var critical = float.Parse(critRaw, CultureInfo.InvariantCulture);

        return new Economy
        {
            Resources = baseResources.ToDictionary(
                kv => kv.Key,
                kv => new ResourceRule
                {
                    Recommended = float.Parse(kv.Value, CultureInfo.InvariantCulture),
                    Thresholds = new Thresholds
                    {
                        Warning  = warning,
                        Critical = critical
                    }
                })
        };
    }

    
    private static Reachability BuildReachability(
        IReadOnlyDictionary<string, int> map)
    {
        return new Reachability
        {
            Thresholds = new Thresholds
            {
                Warning  = Get(map, "Threshold.Warning"),
                Critical = Get(map, "Threshold.Critical")
            },

            AccessBaseline = new AccessBaseline
            {
                OneWayOnFoot = Get(map, "Access.OneWayOnFoot"),
                TwoWayOnFoot = Get(map, "Access.TwoWayOnFoot"),
                TwoWayByCar  = Get(map, "Access.TwoWayByCar")
            },

            DangerLevel = new DangerLevel
            {
                None   = Get(map, "Danger.None"),
                Low    = Get(map, "Danger.Low"),
                Medium = Get(map, "Danger.Medium"),
                Hard   = Get(map, "Danger.Hard")
            }
        };
    }

    private static int Get(
        IReadOnlyDictionary<string, int> map,
        string key)
    {
        if (!map.TryGetValue(key, out var value))
            throw new InvalidOperationException(
                $"Missing reachability config key: {key}");

        return value;
    }
}