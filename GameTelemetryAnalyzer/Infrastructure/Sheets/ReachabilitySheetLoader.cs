using CsvHelper;
using System.Globalization;
using GameTelemetryAnalyzer.Infrastructure.Sheets.Rows;

namespace GameTelemetryAnalyzer.Infrastructure.Sheets;

public sealed class ReachabilitySheetLoader
{
    private readonly HttpClient _http;

    public ReachabilitySheetLoader(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ReachabilityRow>> Load(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException(
                "Reachability sheet URL is missing"
            );

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new InvalidOperationException(
                "Reachability sheet URL is not a valid absolute URI"
            );
        
        var csvText = await _http.GetStringAsync(url);

        using var reader = new StringReader(csvText);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<ReachabilityRow>().ToList();
    }
}