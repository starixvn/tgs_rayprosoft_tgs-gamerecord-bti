using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.BTiService.Core.UnitTest;

public class UtensilRepositoryUnitTest
{



    [Fact]
    public void Get_Game_External_Id()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book"
        });

        var repository = new UtensilRepository(
            gameRecordInfo);

        var actual = repository.GetGameExternalId();

        Assert.NotNull(actual);
        Assert.Equal(gameRecordInfo.Value.GameExternalId, actual);
    }

    [Fact]
    public void Get_Game_Provider_Code()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book"
        });

        var repository = new UtensilRepository(
            gameRecordInfo);

        var actual = repository.GetGameProviderCode();

        Assert.NotNull(actual);
        Assert.Equal(gameRecordInfo.Value.GameProviderCode, actual);
    }
}
