using GameTelemetryAnalyzer.Domain;

namespace GameTelemetryAnalyzer.Application;

public sealed record CliOptions(
    bool SendToDiscord,
    IReadOnlySet<string> Only,
    IReadOnlySet<string> Exclude)
{
    public static CliOptions Parse(string[] args)
    {
        bool sendToDiscord = true;

        var only = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            switch (arg)
            {
                case "--no-discord":
                    sendToDiscord = false;
                    break;

                case "--only":
                    AddValues(args, ref i, only, "--only");
                    break;

                case "--exclude":
                    AddValues(args, ref i, exclude, "--exclude");
                    break;

                default:
                    throw new ArgumentException($"Unknown argument: {arg}");
            }
        }

        return new CliOptions(sendToDiscord, only, exclude);
    }

    /// <summary>
    /// Converts CLI options to domain-level analyzer selection
    /// </summary>
    public AnalyzerSelection ToAnalyzerSelection()
    {
        return new AnalyzerSelection(Only, Exclude);
    }

    private static void AddValues(
        string[] args,
        ref int index,
        ISet<string> target,
        string key)
    {
        if (index + 1 >= args.Length)
            throw new ArgumentException($"Missing value for {key}");

        index++;

        foreach (var value in args[index]
                     .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            target.Add(value);
        }
    }
}