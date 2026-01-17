using FluentAssertions;
using GameTelemetryAnalyzer.Domain;
using Xunit;

namespace Tests.Domain;

public class AnalyzerSelectionTests
{
    [Fact]
    public void Only_should_include_specified_analyzers()
    {
        var selection = new AnalyzerSelection(
            new HashSet<string> { "Reachability" },
            new HashSet<string>()
        );

        selection.Only.Should().Contain("Reachability");
        selection.Exclude.Should().BeEmpty();
    }

    [Fact]
    public void Exclude_should_exclude_specified_analyzers()
    {
        var selection = new AnalyzerSelection(
            new HashSet<string>(),
            new HashSet<string> { "Economy" }
        );

        selection.Exclude.Should().Contain("Economy");
        selection.Only.Should().BeEmpty();
    }

    [Fact]
    public void All_should_have_empty_sets()
    {
        AnalyzerSelection.All.Only.Should().BeEmpty();
        AnalyzerSelection.All.Exclude.Should().BeEmpty();
    }
}