using Microsoft.Extensions.Options;

namespace Tgs.GameRecord.BTi.Core.UnitTest;

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
