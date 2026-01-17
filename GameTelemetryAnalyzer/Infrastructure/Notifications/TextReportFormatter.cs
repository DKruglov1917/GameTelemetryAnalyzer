using System.Text;
using GameTelemetryAnalyzer.Domain;
using GameTelemetryAnalyzer.Delivery;

namespace GameTelemetryAnalyzer.Infrastructure.Notifications;

public sealed class TextReportFormatter : IReportFormatter
{
    private readonly DiscordMessageTemplate _template;

    public TextReportFormatter(DiscordMessageTemplate template)
    {
        _template = template;
    }
    public string Format(TelemetryReport report)
    {
        var sb = new StringBuilder();

        sb.AppendLine(_template.Title);

        var unix = report.RunTimestamp.ToUnixTimeSeconds();
        sb.AppendLine($"{_template.Header} <t:{unix}:F>");

        var sections = BuildSections(report);

        var body = new StringBuilder();
        RenderSections(body, sections, _template);

        var content = body.ToString();

        if (_template.WrapInCodeBlock)
            content = $"```txt\n{content}\n```";

        sb.Append(content);

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