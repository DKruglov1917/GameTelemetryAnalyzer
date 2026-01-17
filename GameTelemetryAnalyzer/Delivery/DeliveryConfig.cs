namespace GameTelemetryAnalyzer.Delivery;

public sealed class DeliveryConfig
{
    public required DiscordDelivery Discord { get; init; }
}

public sealed class DiscordDelivery
{
    public required bool Enabled { get; init; }
    public required string WebhookUrl { get; init; }
    public required DiscordMessageTemplate Message { get; init; }
}

public sealed class DiscordMessageTemplate
{
    public required string Title { get; init; }
    public required string Header { get; init; }
    public required string SnapshotHeader { get; init; }
    public required string ExecutedHeader { get; init; }

    public required string SectionHeaderFormat { get; init; }
    public required string ItemPrefixOk { get; init; }
    public required string ItemPrefixWarning { get; init; }
    public required string ItemPrefixCritical { get; init; }
    public required string Indent { get; init; }

    // Display logic
    public required bool HideOkBlocks { get; init; }   
    public required bool HideEmptySeverityBlocks { get; init; } 
    public required bool HideEmptySections { get; init; }      
    public required bool WrapInCodeBlock { get; init; }       
}

