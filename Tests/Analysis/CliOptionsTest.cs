using Xunit;
using FluentAssertions;
using GameTelemetryAnalyzer.Application;
using GameTelemetryAnalyzer.Domain;

namespace Tests.Analysis;

public class CliOptionsTest
{
    [Fact]
    public void Cli_options_should_map_to_analyzer_selection()
    {
        var options = CliOptions.Parse([
            "--only", "Economy",
            "--exclude", "Reachability"
        ]);

        var selection = options.ToAnalyzerSelection();

        selection.Only.Should().Contain("Economy");
        selection.Exclude.Should().Contain("Reachability");
    }
    
    [Fact]
    public void Analyzer_names_should_be_case_insensitive()
    {
        var options = CliOptions.Parse([
            "--only", "economy",
            "--exclude", "REACHABILITY"
        ]);

        var selection = options.ToAnalyzerSelection();

        selection.Only.Should().Contain("economy");
        selection.Exclude.Should().Contain("reachability");
    }
    
    [Fact]
    public void Multiple_values_should_be_parsed_from_comma_separated_list()
    {
        var options = CliOptions.Parse([
            "--only", "Economy,Reachability"
        ]);

        var selection = options.ToAnalyzerSelection();

        selection.Only.Should().BeEquivalentTo("Economy", "Reachability");
    }
    
    [Fact]
    public void Empty_args_should_produce_all_selection()
    {
        var options = CliOptions.Parse([]);

        var selection = options.ToAnalyzerSelection();

        selection.Should().BeEquivalentTo(AnalyzerSelection.All);
    }

    
    [Fact]
    public void Unknown_argument_should_throw()
    {
        Action act = () => CliOptions.Parse(["--LoremIpsum"]);

        act.Should().Throw<ArgumentException>();
    }
}