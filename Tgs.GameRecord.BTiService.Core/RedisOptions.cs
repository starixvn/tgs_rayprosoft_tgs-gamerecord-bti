namespace Tgs.GameRecord.BTiService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
