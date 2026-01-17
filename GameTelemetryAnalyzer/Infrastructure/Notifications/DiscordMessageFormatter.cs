using System.Text;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Delivery;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;
public interface IMessageFormatter
{
    string Format(
        TelemetryReport report,
        DiscordMessageTemplate template);
}

public class DiscordMessageFormatter : IMessageFormatter
{
    public string Format(TelemetryReport report, DiscordMessageTemplate template)
    {
        var sb = new StringBuilder();

        sb.AppendLine(template.Title);

        var unix = report.RunTimestamp.ToUnixTimeSeconds();
        sb.AppendLine($"{template.Header} <t:{unix}:F>");

        var sections = BuildSections(report);

        var body = new StringBuilder();
        RenderSections(body, sections, template);

        var content = body.ToString();

        if (template.WrapInCodeBlock)
            content = $"```txt\n{content}\n```";

        sb.Append(content);
        
        sb.AppendLine("\u200B"); // Zero-width space for Discord message separation

        return sb.ToString();
    }

    private static IReadOnlyList<AnalysisSection> BuildSections(
        TelemetryReport report)
    {
        var sections = new List<AnalysisSection>();

        foreach (var result in report.Results)
        {
            var blocks = result.Findings
                .GroupBy(f => f.Severity)
                .Select(g => new SeverityBlock
                {
                    Severity = g.Key,
                    Items = g.Select(f => f.Message).ToList()
                })
                .ToList();
            
            sections.Add(new AnalysisSection
            {
                Title = result.AnalyzerId,
                Blocks = blocks
            });
        }
        
        return sections;
    }

    private static void RenderSections(
        StringBuilder sb,
        IReadOnlyList<AnalysisSection> sections,
        DiscordMessageTemplate template)
    {
        foreach (var section in sections)
        {
            if (template.HideEmptySections &&
                section.Blocks.All(b =>
                    b.Items.Count == 0 ||
                    (template.HideOkBlocks && b.Severity == Severity.Ok)))
            {
                continue;
            }

            sb.AppendLine(
                template.SectionHeaderFormat
                    .Replace("{title}", section.Title));

            foreach (var block in section.Blocks)
            {
                if (template.HideOkBlocks &&
                    block.Severity == Severity.Ok)
                    continue;

                if (template.HideEmptySeverityBlocks &&
                    block.Items.Count == 0)
                    continue;

                sb.AppendLine(
                    $"{template.Indent}{GetSeverityPrefix(block.Severity, template)}");

                foreach (var item in block.Items)
                {
                    sb.AppendLine($"{template.Indent}{template.Indent}{item}");
                }

                sb.AppendLine();
            }
        }
    }


    private static string GetSeverityPrefix(
        Severity severity,
        DiscordMessageTemplate template)
    {
        return severity switch
        {
            Severity.Ok => template.ItemPrefixOk,
            Severity.Warning => template.ItemPrefixWarning,
            Severity.Critical => template.ItemPrefixCritical,
            _ => severity.ToString()
        };
    }
}