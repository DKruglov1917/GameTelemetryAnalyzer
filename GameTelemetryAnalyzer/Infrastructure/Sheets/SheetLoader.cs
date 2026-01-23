using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace GameTelemetryAnalyzer.Infrastructure.Sheets;

public static class SheetLoader
{
    public static async Task<IReadOnlyList<SheetRow>> Load(
        HttpClient http,
        string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException("Sheet URL is missing");

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new InvalidOperationException("Sheet URL is not a valid absolute URI");

        var csvText = await http.GetStringAsync(url);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            TrimOptions = TrimOptions.Trim,
            HasHeaderRecord = false,
            IgnoreBlankLines = false
        };

        using var reader = new StringReader(csvText);
        using var csv = new CsvReader(reader, config);

        var rows = new List<SheetRow>();
        var rowIndex = 1;

        while (await csv.ReadAsync())
        {
            var raw = new List<string>();

            for (int i = 0; csv.TryGetField(i, out string? value); i++)
            {
                raw.Add(value ?? string.Empty);
            }

            rows.Add(new SheetRow(
                rowIndex: rowIndex++,
                raw: raw
            ));
        }

        return rows;
    }
}