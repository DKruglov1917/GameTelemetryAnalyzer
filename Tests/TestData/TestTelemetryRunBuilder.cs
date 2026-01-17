using GameTelemetryAnalyzer.Domain;

namespace Tests.TestData;

public sealed class TestTelemetryRunBuilder
{
    private RunInfo _runInfo = new()
    {
        RunId = "test",
        Source = "tests",
        GeneratedAtUtc = DateTimeOffset.UtcNow.ToString("O")
    };

    private Snapshot _snapshot = TestSnapshotBuilder.Minimal();
    private RuntimeEvents _runtimeEvents = RuntimeEvents.Empty();

    public static TestTelemetryRunBuilder Create() => new();

    public TestTelemetryRunBuilder WithPoi(string label, AccessType access, float distance)
    {
        _snapshot = TestSnapshotBuilder.WithPoi(label, access, distance);
        return this;
    }
    
    public TestTelemetryRunBuilder WithResource(string name, int amount)
    {
        _snapshot = TestSnapshotBuilder.WithResource(name, amount);
        return this;
    }

    public TelemetryRun Build() => new()
    {
        RunInfo = _runInfo,
        Snapshot = _snapshot,
        RuntimeEvents = _runtimeEvents
    };
}
