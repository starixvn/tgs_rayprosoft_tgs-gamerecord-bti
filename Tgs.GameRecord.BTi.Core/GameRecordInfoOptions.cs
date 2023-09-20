namespace Tgs.GameRecord.BTi.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public string GameExternalId { get; set; } = default!;
    public string GameProviderConfiguration { get; init; } = default!;

    public string GameProviderHangfire { get; init; } = default!;

    public double IntervalMinutes { get; set; } = 5d;

    public double IntervalMinutesMax { get; set; } = 30d;

    public double IntervalMinutesMinimum { get; set; } = 1d;
}
