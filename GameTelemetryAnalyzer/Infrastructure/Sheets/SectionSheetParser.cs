namespace GameTelemetryAnalyzer.Infrastructure.Sheets;

public static class SectionSheetParser
{
    public static IReadOnlyDictionary<string, Dictionary<string, string>> Parse(
        IEnumerable<SheetRow> rows)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();
        string? currentSection = null;

        foreach (var row in rows)
        {
            if (row.IsEmpty)
                continue;

            var firstCell = row.FirstCell;

            if (firstCell.StartsWith("#"))
            {
                currentSection = firstCell
                    .TrimStart('#')
                    .Trim()
                    .TrimEnd(',');

                result[currentSection] = new Dictionary<string, string>();
                continue;
            }

            if (currentSection == null)
                continue;

            if (row.Raw.Count < 2)
                continue;

            var key = row.Raw[0].Trim();
            var value = row.Raw[1].Trim();

            if (key.Equals("Key", StringComparison.OrdinalIgnoreCase))
                continue;

            if (string.IsNullOrWhiteSpace(key))
                continue;

            result[currentSection][key] = value;
        }

        return result;
    }
}