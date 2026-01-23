namespace GameTelemetryAnalyzer.Infrastructure.Sheets;

public sealed class SheetRow
{
    public int RowIndex { get; }
    public IReadOnlyList<string> Raw { get; }

    public SheetRow(int rowIndex, IReadOnlyList<string> raw)
    {
        RowIndex = rowIndex;
        Raw = raw;
    }

    public bool IsEmpty => Raw.All(string.IsNullOrWhiteSpace);

    public string FirstCell =>
        Raw.Count > 0
            ? Raw[0].Trim().TrimStart('\uFEFF')
            : "";
}
