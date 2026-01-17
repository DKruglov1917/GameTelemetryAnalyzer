namespace Tests.TestData;

using GameTelemetryAnalyzer.Domain;

public static class TestSnapshotBuilder
{
    public static Snapshot Minimal() => new()
    {
        AvailableResources = new List<ResourceMetric>(),
        ElectricityMetrics = new List<ElectricityMetric>(),
        EnemySpawnerMetrics = new List<EnemySpawnerMetric>(),
        WeatherMetrics = new List<WeatherMetric>(),
        PoiMetrics = new List<PoiMetric>()
    };

    public static Snapshot WithPoi(string label, AccessType access, float distance) => new()
    {
        AvailableResources = new List<ResourceMetric>(),
        ElectricityMetrics = new List<ElectricityMetric>(),
        EnemySpawnerMetrics = new List<EnemySpawnerMetric>(),
        WeatherMetrics = new List<WeatherMetric>(),
        PoiMetrics = new List<PoiMetric>
        {
            new()
            {
                Id = "poi-1",
                Label = label,
                AccessType = access,
                DistanceToBase = distance
            }
        }
    };
    
    public static Snapshot WithResource(string name, int amount)
    {
        return new Snapshot
        {
            AvailableResources = new List<ResourceMetric>
            {
                new()
                {
                    ResourceId = name,
                    Amount = amount
                }
            },
            ElectricityMetrics = new List<ElectricityMetric>(),
            EnemySpawnerMetrics = new List<EnemySpawnerMetric>(),
            PoiMetrics = new List<PoiMetric>(),
            WeatherMetrics = new List<WeatherMetric>()
        };
    }
}