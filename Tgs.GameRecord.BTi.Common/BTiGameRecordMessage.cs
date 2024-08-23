using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.BTi.Common;

public record BTiGameRecordMessage
{
	/// <summary>
	/// The data API URL
	/// </summary>
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://data-api.442hattrick.com/")]
	public Uri DataApiUrl { get; init; } = default!;

	/// <summary>
	/// The agent user name
	/// </summary>
	[Required]
	[DefaultValue(typeof(string), "PegaGaming_GPIRMB")]
	public string AgentUserName { get; init; } = default!;

	/// <summary>
	/// The agent password
	/// </summary>
	[Required]
	[DefaultValue(typeof(string), "IM60Ho7Y4B9C")]
	public string AgentPassword { get; init; } = default!;

	/// <summary>
	/// The start time
	/// </summary>
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-05-29T00:00:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;

	/// <summary>
	/// The end time
	/// </summary>
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-05-30T00:00:00+08:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
