using System;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameInterface.BTiService.Protos;

namespace Tgs.GameRecord.BTi.Core.UnitTest;

public class GameRecordUtensilUnitTest
{
    [Fact]
    public void Settlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameHistorySettled
        {
            ExternalRoundId = "443988629220675584",
            PlayerCode = "4ucmqplxb029",
            IsClosed = true,
            Status = GameHistoryStatus.Won,
            ExternalCreatedOn = DateTimeOffset.Parse("2023-05-10T04:38:56.602").ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.Parse("2023-05-10T04:44:56.009").ToTimestamp(),
            Bet = 4m,
            ValidBet = 4m,
            Settle = 8.24m,
            PlayerWinLoss = 4.240000m,
            Turnover = 4,
            AdditionalData = "{\"BetSettledDate\":\"2023-05-10T04:38:56.602\",\"BetStatus\":\"Won\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T04:12:18.11\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"OddsStyleOfUser\":\"European\",\"PL\":4.24,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"443988629220675584\",\"RealMoneyAmount\":4.0,\"Return\":8.24,\"SearchDateTime\":\"2023-05-10T04:44:56.009\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Phoenix Suns\",\"BetID\":\"443988629220675585\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0HC443242889859137536AMM\",\"EventDate\":\"2023-05-10T02:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting Spread\",\"GameID\":443242887480950784,\"HomeTeam\":\"Denver Nuggets\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":2,\"LineTypeName\":\"Spread\",\"LiveScore1\":104,\"LiveScore2\":80,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"Points\":19.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"118 : 102\",\"Status\":\"Won\",\"UpdateDate\":null,\"YourBet\":\"Phoenix Suns \\u002B19.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Won\",\"SystemName\":\"0/1\",\"TotalStake\":4.0,\"UpdateDate\":\"2023-05-10T04:38:56.602\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":4.0}"
        };

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = gameRecordUtensil.SettlementConvertRecord(source: source);

        Assert.NotNull(actual);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.True(actual.Status != GameHistoryStatus.Opened);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.ValidBet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.ValidBet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.PlayerWinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public void Settlement_Normal_Process_Win()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameHistorySettled
        {
            ExternalRoundId = "443988629220675584",
            PlayerCode = "4ucmqplxb029",
            IsClosed = true,
            Status = GameHistoryStatus.Won,
            ExternalCreatedOn = DateTimeOffset.Parse("2023-05-10T04:38:56.602").ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.Parse("2023-05-10T04:44:56.009").ToTimestamp(),
            Bet = 4m,
            ValidBet = 4m,
            Settle = 8.24m,
            PlayerWinLoss = 4.240000m,
            Turnover = 4,
            AdditionalData = "{\"BetSettledDate\":\"2023-05-10T04:38:56.602\",\"BetStatus\":\"Won\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T04:12:18.11\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"OddsStyleOfUser\":\"European\",\"PL\":4.24,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"443988629220675584\",\"RealMoneyAmount\":4.0,\"Return\":8.24,\"SearchDateTime\":\"2023-05-10T04:44:56.009\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Phoenix Suns\",\"BetID\":\"443988629220675585\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0HC443242889859137536AMM\",\"EventDate\":\"2023-05-10T02:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting Spread\",\"GameID\":443242887480950784,\"HomeTeam\":\"Denver Nuggets\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":2,\"LineTypeName\":\"Spread\",\"LiveScore1\":104,\"LiveScore2\":80,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"Points\":19.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"118 : 102\",\"Status\":\"Won\",\"UpdateDate\":null,\"YourBet\":\"Phoenix Suns \\u002B19.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Won\",\"SystemName\":\"0/1\",\"TotalStake\":4.0,\"UpdateDate\":\"2023-05-10T04:38:56.602\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":4.0}"
        };

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = gameRecordUtensil.SettlementConvertRecord(source: source);

        Assert.NotNull(actual);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.True(actual.Status != GameHistoryStatus.Opened);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.ValidBet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.ValidBet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.PlayerWinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }
}
