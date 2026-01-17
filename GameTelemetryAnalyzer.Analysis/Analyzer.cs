using System.Text.Json;
using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Analysis;

public abstract class Analyzer
{
    /// <summary>
    /// Readable identifier used in CLI (--only / --exclude)
    /// </summary>
    public abstract string Name { get; }
    
    public abstract AnalyzerResult Execute (Snapshot snapshot, RunConfig config);
}