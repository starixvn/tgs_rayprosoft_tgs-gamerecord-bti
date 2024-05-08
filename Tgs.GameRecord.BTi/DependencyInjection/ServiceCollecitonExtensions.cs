using Tgs.GameCommand.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameRecord.BTi;
using Tgs.GameRecord.BTi.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameRecord(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> redisDBconfigure,
		Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceBTiClientOptions, IServiceProvider> gameInterfaceBTiClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisDBconfigure)
		.Services
		.AddOptions<GameRecordInfoOptions>()
		.Configure(gameRecordInfoOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameRecordCore(
			gameCommandClientOptions: gameCommandClientOptions,
			gameInterfaceBTiClientOptions: gameInterfaceBTiClientOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
