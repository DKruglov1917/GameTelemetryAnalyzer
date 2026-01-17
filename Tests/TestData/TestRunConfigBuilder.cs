using GameTelemetryAnalyzer.Domain;

namespace Tests.TestData;

public sealed class TestRunConfigBuilder
{
    private Economy _economy = DefaultEconomy();
    private Reachability _reachability = DefaultReachability();

    public static TestRunConfigBuilder Create() => new();

    public TestRunConfigBuilder WithReachability(
        int oneWayOnFoot,
        int twoWayOnFoot,
        int twoWayByCar,
        int warning,
        int critical,
        int dangerNone = 0,
        int dangerLow = 10,
        int dangerMedium = 20,
        int dangerHard = 30)
    {
        _reachability = new Reachability
        {
            Thresholds = new Thresholds
            {
                Warning = warning,
                Critical = critical
            },
            AccessBaseline = new AccessBaseline
            {
                OneWayOnFoot = oneWayOnFoot,
                TwoWayOnFoot = twoWayOnFoot,
                TwoWayByCar = twoWayByCar
            },
            DangerLevel = new DangerLevel
            {
                None = dangerNone,
                Low = dangerLow,
                Medium = dangerMedium,
                Hard = dangerHard
            }
        };

        return this;
    }

    public TestRunConfigBuilder WithEconomy(Dictionary<string, ResourceRule> resources)
    {
        _economy = new Economy
        {
            Resources = resources
        };

        return this;
    }

    public RunConfig Build()
        => new()
        {
            Economy = _economy,
            Reachability = _reachability
        };

    // ---------- defaults ----------

    private static Economy DefaultEconomy()
        => new()
        {
            Resources = new Dictionary<string, ResourceRule>()
        };

    private static Reachability DefaultReachability()
        => new()
        {
            Thresholds = new Thresholds
            {
                Warning = 10_000,
                Critical = 30_000
            },
            AccessBaseline = new AccessBaseline
            {
                OneWayOnFoot = 5_000,
                TwoWayOnFoot = 10_000,
                TwoWayByCar = 20_000
            },
            DangerLevel = new DangerLevel
            {
                None = 0,
                Low = 10,
                Medium = 20,
                Hard = 30
            }
        };
}
