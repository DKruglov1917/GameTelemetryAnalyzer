namespace GameTelemetryAnalyzer.Domain;

public enum Severity 
{
    Ok,
    Warning,
    Critical
}

public enum AccessType
{
    OneWayOnFoot,
    TwoWayOnFoot,
    TwoWayByCar
}

public sealed class RunConfig
{
    public required Economy Economy { get; init; }
    public required Reachability Reachability { get; init; }
}

public sealed class Economy
{
    public required Dictionary<string, ResourceRule> Resources { get; init; }
}

public sealed class ResourceRule
{
    public required int Recommended { get; init; }
    public required Thresholds Thresholds { get; init; }
}

public sealed class Reachability
{
    public required Thresholds Thresholds { get; init; }
    public required AccessBaseline AccessBaseline { get; init; }
    public required DangerLevel DangerLevel { get; init; }
}

public sealed class Thresholds
{
    public required int Warning { get; init; }
    public required int Critical { get; init; }
    
    public Severity Evaluate(float delta)
    {
        if (delta >= Critical)
            return Severity.Critical;
        
        if (delta >= Warning)
            return Severity.Warning;

        return Severity.Ok;
    }
}

public sealed class AccessBaseline
{
    public required int OneWayOnFoot { get; init; }
    public required int TwoWayOnFoot { get; init; }
    public required int TwoWayByCar { get; init; }

    public int Get(AccessType type) => type switch
    {
        AccessType.OneWayOnFoot => OneWayOnFoot,
        AccessType.TwoWayOnFoot => TwoWayOnFoot,
        AccessType.TwoWayByCar  => TwoWayByCar,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}

public sealed class DangerLevel
{
    public required int None { get; init; }
    public required int Low { get; init; }
    public required int Medium { get; init; }
    public required int Hard { get; init; }
}