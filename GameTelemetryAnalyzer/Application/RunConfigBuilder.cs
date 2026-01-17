using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Infrastructure.Sheets.Rows;

namespace GameTelemetryAnalyzer.Application;

public static class RunConfigBuilder
{
    public static Economy BuildEconomy(IEnumerable<EconomyRow> rows)
    {
        return new Economy
        {
            Resources = rows.ToDictionary(
                r => r.ResourceId,
                r => new ResourceRule
                {
                    Recommended = r.Recommended,
                    Thresholds = new Thresholds
                    {
                        Warning = r.Warning,
                        Critical = r.Critical
                    }
                }
            )
        };
    }
    
    public static Reachability BuildReachability(
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