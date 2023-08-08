using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.BTiService.Protos;

namespace Tgs.GameRecord.BTiService.Core.UnitTest;

public class GameRecordUtensilUnitTest
{
    [Fact]
    public async Task Settlement_Brand_Game_Provider_Setting_Does_Not_Exist()
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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Throws<Exception>();

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        _ = await Assert.ThrowsAsync<Exception>(
            () => gameRecordUtensil
            .SettlementConvertRecordAsync(
                source: source)
            .AsTask())
            .ConfigureAwait(false);


    }

    [Fact]
    public async Task Settlement_Game_Does_Not_Exist()
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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Throws<Exception>();

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        _ = await Assert.ThrowsAsync<Exception>(
            () => gameRecordUtensil
            .SettlementConvertRecordAsync(
                 source: source)
             .AsTask())
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task Settlement_Normal_Process()
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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
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
    public async Task Settlement_Normal_Process_Win()
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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
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
    public async Task Settlement_Player_Does_Not_Exist()
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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "BTI";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(source.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Throws<Exception>();

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetGameExternalId()
            .Returns(externalGameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        _ = await Assert.ThrowsAsync<Exception>(
            () => gameRecordUtensil
            .SettlementConvertRecordAsync(
                source: source)
            .AsTask())
            .ConfigureAwait(false);
    }
}
