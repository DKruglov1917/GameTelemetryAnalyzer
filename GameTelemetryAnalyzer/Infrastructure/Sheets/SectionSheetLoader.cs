namespace GameTelemetryAnalyzer.Infrastructure.Sheets;

public sealed class SectionSheetLoader
{
    private readonly HttpClient _http;

    public SectionSheetLoader(HttpClient http)
    {
        _http = http;
    }

    public async Task<SectionSheet> Load(string sheetUrl)
    {
        var rows = await SheetLoader.Load(_http, sheetUrl);
        return new SectionSheet(rows);
    }
}

public sealed class SectionSheet
{
    private readonly IReadOnlyDictionary<string, Dictionary<string, string>> _sections;

    public SectionSheet(IEnumerable<SheetRow> rows)
    {
        _sections = SectionSheetParser.Parse(rows);
    }

    public IReadOnlyDictionary<string, string> Require(string name)
    {
        if (!_sections.TryGetValue(name, out var section))
            throw new InvalidOperationException($"Missing section: {name}");

        return section;
    }
}

