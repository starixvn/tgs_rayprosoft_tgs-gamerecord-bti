using Microsoft.Extensions.Options;

namespace Tgs.GameRecord.BTi.Core;

internal class UtensilRepository : IUtensil
{
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public UtensilRepository(
        IOptions<GameRecordInfoOptions> options)
    {
        _gameRecordInfo = options.Value;
    }

    public string GetGameExternalId()
        => _gameRecordInfo.GameExternalId;

    public string GetGameProviderCode()
        => _gameRecordInfo.GameProviderCode;
}
