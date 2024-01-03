using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.BTi.Common;

public record BTiGameRecordMessage
{
    /// <summary>
    /// The data API URL
    /// </summary>
    [Required]
    [DataType(DataType.Url)]
    public Uri DataApiUrl { get; init; } = default!;
    /// <summary>
    /// The agent user name
    /// </summary>
    [Required]
    public string AgentUserName { get; init; } = default!;
    /// <summary>
    /// The agent password
    /// </summary>
    [Required]
    public string AgentPassword { get; init; } = default!;
    /// <summary>
    /// The start time
    /// </summary>
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; } = default!;
    /// <summary>
    /// The end time
    /// </summary>
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; } = default!;
}
