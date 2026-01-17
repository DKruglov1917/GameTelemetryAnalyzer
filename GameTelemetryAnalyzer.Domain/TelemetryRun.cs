namespace GameTelemetryAnalyzer.Domain;

/// <summary>
/// Input model for telemetry analysis.
///
/// Telemetry conventions:
/// - All fields are always present
/// - ""  = N/A (not applicable / unknown) for string fields
/// - -1  = N/A for numeric fields
/// </summary>
/// 
public sealed class TelemetryRun
    {
        public required RunInfo RunInfo { get; init; }
        public required Snapshot Snapshot { get; init; }
        public required RuntimeEvents RuntimeEvents { get; init; }
    }

    public sealed class RunInfo
    {
        public required string RunId { get; init; }
        public required string GeneratedAtUtc { get; init; }
        public required string Source { get; init; }
    }

    public sealed class Snapshot
    {
        public required List<PoiMetric> PoiMetrics { get; init; }
        public required List<ResourceMetric> AvailableResources { get; init; }
        public required List<ElectricityMetric> ElectricityMetrics { get; init; }
        public required List<EnemySpawnerMetric> EnemySpawnerMetrics { get; init; }
        public required List<WeatherMetric> WeatherMetrics { get; init; }
    }

    public sealed class RuntimeEvents
    {
        public required List<RuntimeEventMetric> MainlandEvents { get; init; }
        public required List<RuntimeEventMetric> WeatherEvents { get; init; }
        public required List<RuntimeEventMetric> EconomyEvents { get; init; }
        public required List<RuntimeEventMetric> EnemyEvents { get; init; }
        
        public static RuntimeEvents Empty() => new()
        {
            MainlandEvents = [],
            WeatherEvents  = [],
            EconomyEvents  = [],
            EnemyEvents    = []
        };
    }

    #region ItemModels

    public sealed class RuntimeEventMetric
    {
        public required double Time { get; init; }
        public required string Type { get; init; }
        public required string EntityId { get; init; }
        public required string PoiId { get; init; }
    }

    public sealed class PoiMetric
    {
        public required string Label { get; init; }
        public required string Id { get; init; }
        public required AccessType AccessType { get; init; }
        public required float DistanceToBase { get; init; }
    }

    public sealed class ResourceMetric
    {
        public required string ResourceId { get; init; }
        public required int Amount { get; init; }
    }

    public sealed class ElectricityMetric
    {
        public required string NodeId { get; init; }
        public required double Produced { get; init; }
        public required double Consumed { get; init; }
    }

    public sealed class EnemySpawnerMetric
    {
        public required string SpawnerId { get; init; }
        public required string EnemyType { get; init; }
        public required int MaxAlive { get; init; }
    }

    public sealed class WeatherMetric
    {
        public required string ZoneId { get; init; }
        public required string WeatherType { get; init; }
        public required double TemperatureC { get; init; }
    }

    #endregion
