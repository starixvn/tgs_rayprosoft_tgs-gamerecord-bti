using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.BTiService.Core;

internal class UtensilRepository : IUtensil
{
    private readonly IGameCommandClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public UtensilRepository(
         IOptions<GameRecordInfoOptions> options,
        IGameCommandClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        => _client.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId);

    public async ValueTask<int> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
    {
        var gameModel = await _client.GameGetAsync(gameProviderId, gameExternalId).ConfigureAwait(false);
        return gameModel?.GameId ?? throw new Exception($"The identifier for this game does not exist.({gameExternalId})");
    }

    public async ValueTask<Guid> GetGameProviderIdAsync()
    => await _client.GameProviderGetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false)
        ?? throw new Exception($"The identifier for this game provider does not exist.({_gameRecordInfo.GameProviderCode})");

    public ValueTask<PlayerInfo?> GetPlayerAsync(string playerCode)
        => _client.PlayerGetAsync(playerCode);
}
