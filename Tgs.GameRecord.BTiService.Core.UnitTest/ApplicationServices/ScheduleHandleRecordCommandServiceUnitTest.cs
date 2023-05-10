using System;
using System.Text.Json;
using System.Threading;
using NSubstitute;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTiService.Core.ApplicationServices;
using Tgs.GameRecord.BTiService.Core.Models;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.BTiService.Core.UnitTest.ApplicationServices;

public class ScheduleHandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process_Settlement_Loss()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new ScheduleHandleRecordCommand(
            GameRecord: new RecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "BTI",
                ExternalRoundId: "444000837900083200",
                GameId: 1,
                ExternalGameId: "sports_book",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: GameHistoryStatus.Lost,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-05-10T04:38:56.602"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-05-10T04:44:56.009"),
                Bet: 8m,
                ValidBet: 8m,
                Settle: 0m,
                Commission: 0,
                Turnover: 8m,
                WinLoss: -8m,
                AdditionalData: "{\"BetSettledDate\":\"2023-05-10T06:13:16.871\",\"BetStatus\":\"Lost\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T05:00:48.888\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":-114,\"OddsDec\":1.88,\"OddsInUserStyle\":\"1.88\",\"OddsStyleOfUser\":\"European\",\"PL\":-8.0,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"444000837900083200\",\"RealMoneyAmount\":8.0,\"Return\":0.0,\"SearchDateTime\":\"2023-05-10T06:16:36.887\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Indonesia [W]\",\"BetID\":\"444000837900083201\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0OU443743933215096834OMM\",\"EventDate\":\"2023-05-10T04:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting O/U\",\"GameID\":443743932179120128,\"HomeTeam\":\"Thailand [W]\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":443001798798925824,\"LeagueName\":\"SEA Games Women\",\"LineID\":0,\"LineTypeID\":3,\"LineTypeName\":\"O/U\",\"LiveScore1\":30,\"LiveScore2\":44,\"Odds\":-114,\"OddsDec\":1.88,\"OddsInUserStyle\":\"1.88\",\"Points\":141.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"69 : 70\",\"Status\":\"Lost\",\"UpdateDate\":null,\"YourBet\":\"Over 141.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Lost\",\"SystemName\":\"0/1\",\"TotalStake\":8.0,\"UpdateDate\":\"2023-05-10T06:13:16.871\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":8.0}"
                ));

        var service = new ScheduleHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Loss),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Void()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new ScheduleHandleRecordCommand(
            GameRecord: new RecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "BTI",
                ExternalRoundId: "438278232526213120",
                GameId: 1,
                ExternalGameId: "sports_book",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: GameHistoryStatus.Canceled,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-24T10:51:48.852"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-04-24T10:56:27.861"),
                Bet: 10m,
                ValidBet: 0m,
                Settle: 0m,
                Commission: 0,
                Turnover: 0m,
                WinLoss: 0m,
                AdditionalData: "{\"BetSettledDate\":\"2023-04-24T10:51:48.852\",\"BetStatus\":\"Canceled\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPISO\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-04-24T10:01:13.432\",\"Currency\":\"PHP\",\"CustomerID\":109328836,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uhbg9votnm8\",\"NonCashOutAmount\":0,\"Odds\":125,\"OddsDec\":2.25,\"OddsInUserStyle\":\"2.25\",\"OddsStyleOfUser\":\"European\",\"PL\":0.0,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"438278232526213120\",\"RealMoneyAmount\":10.0,\"Return\":10.0,\"SearchDateTime\":\"2023-04-24T10:56:27.861\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Eva Lys\",\"BetID\":\"438278232526213121\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":6,\"BranchName\":\"Tennis\",\"CouponCode\":\"\",\"EncodedLineID\":\"0OU438048282158383113UM100\",\"EventDate\":\"2023-04-24T10:00:00\",\"EventTypeID\":721,\"EventTypeName\":\"1st Set Total Games O/U\",\"GameID\":438048281759924224,\"HomeTeam\":\"Dayana Yastremska\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":438042669189705728,\"LeagueName\":\"WTA Madrid\",\"LineID\":0,\"LineTypeID\":3,\"LineTypeName\":\"O/U\",\"LiveScore1\":0,\"LiveScore2\":0,\"Odds\":125,\"OddsDec\":2.25,\"OddsInUserStyle\":\"2.25\",\"Points\":8.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"None\",\"Status\":\"Canceled\",\"UpdateDate\":null,\"YourBet\":\"Under 8.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Canceled\",\"SystemName\":\"0/1\",\"TotalStake\":10.0,\"UpdateDate\":\"2023-04-24T10:51:48.852\",\"UserName\":\"4uhbg9votnm8\",\"ValidStake\":0}"
                ));

        var service = new ScheduleHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Win),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Win()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new ScheduleHandleRecordCommand(
            GameRecord: new RecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "BTI",
                ExternalRoundId: "643583c37c18c573b57bbb2c",
                GameId: 1,
                ExternalGameId: "sports_book",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: GameHistoryStatus.Won,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-05-10T04:38:56.602"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-05-10T04:44:56.009"),
                Bet: 4m,
                ValidBet: 4m,
                Settle: 8.24m,
                Commission: 0,
                Turnover: 4m,
                WinLoss: 4.24m,
                AdditionalData: "{\"BetSettledDate\":\"2023-05-10T04:38:56.602\",\"BetStatus\":\"Won\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T04:12:18.11\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"OddsStyleOfUser\":\"European\",\"PL\":4.24,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"443988629220675584\",\"RealMoneyAmount\":4.0,\"Return\":8.24,\"SearchDateTime\":\"2023-05-10T04:44:56.009\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Phoenix Suns\",\"BetID\":\"443988629220675585\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0HC443242889859137536AMM\",\"EventDate\":\"2023-05-10T02:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting Spread\",\"GameID\":443242887480950784,\"HomeTeam\":\"Denver Nuggets\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":2,\"LineTypeName\":\"Spread\",\"LiveScore1\":104,\"LiveScore2\":80,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"Points\":19.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"118 : 102\",\"Status\":\"Won\",\"UpdateDate\":null,\"YourBet\":\"Phoenix Suns \\u002B19.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Won\",\"SystemName\":\"0/1\",\"TotalStake\":4.0,\"UpdateDate\":\"2023-05-10T04:38:56.602\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":4.0}"
                ));

        var service = new ScheduleHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Win),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Success_Is_False()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new ScheduleHandleRecordCommand(
            GameRecord: new RecordInfo(
                Success: false,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "BTI",
                ExternalRoundId: "643583c37c18c573b57bbb2c",
                GameId: 1,
                ExternalGameId: "sports_book",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: GameHistoryStatus.Won,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-05-10T04:38:56.602"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-05-10T04:44:56.009"),
                Bet: 4m,
                ValidBet: 4m,
                Settle: 8.24m,
                Commission: 0,
                Turnover: 4m,
                WinLoss: 4.24m,
                AdditionalData: "{\"BetSettledDate\":\"2023-05-10T04:38:56.602\",\"BetStatus\":\"Won\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T04:12:18.11\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"OddsStyleOfUser\":\"European\",\"PL\":4.24,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"443988629220675584\",\"RealMoneyAmount\":4.0,\"Return\":8.24,\"SearchDateTime\":\"2023-05-10T04:44:56.009\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Phoenix Suns\",\"BetID\":\"443988629220675585\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0HC443242889859137536AMM\",\"EventDate\":\"2023-05-10T02:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting Spread\",\"GameID\":443242887480950784,\"HomeTeam\":\"Denver Nuggets\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":2,\"LineTypeName\":\"Spread\",\"LiveScore1\":104,\"LiveScore2\":80,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"Points\":19.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"118 : 102\",\"Status\":\"Won\",\"UpdateDate\":null,\"YourBet\":\"Phoenix Suns \\u002B19.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Won\",\"SystemName\":\"0/1\",\"TotalStake\":4.0,\"UpdateDate\":\"2023-05-10T04:38:56.602\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":4.0}"
                ));

        var service = new ScheduleHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundMissing(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                data: Arg.Is(JsonSerializer.Serialize(command.GameRecord)),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Unsettlement()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new ScheduleHandleRecordCommand(
            GameRecord: new RecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "BTI",
                ExternalRoundId: "643583c37c18c573b57bbb2c",
                GameId: 1,
                ExternalGameId: "sports_book",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Status: GameHistoryStatus.Opened,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-05-10T05:03:04.111"),
                ExternalClosedOn: null,
                Bet: 7m,
                ValidBet: 7m,
                Settle: 0m,
                Commission: 0,
                Turnover: 7m,
                WinLoss: 0m,
                AdditionalData: "{\"BetSettledDate\":\"2023-05-10T05:03:04.111\",\"BetStatus\":\"Opened\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T05:03:03.988\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"Gain\":\"13.37\",\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":-110,\"OddsDec\":1.91,\"OddsInUserStyle\":\"1.91\",\"OddsStyleOfUser\":\"European\",\"PL\":0.0,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"444001404563144704\",\"RealMoneyAmount\":7.0,\"Return\":0.0,\"SearchDateTime\":\"2023-05-10T05:08:28.633\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Miami Heat\",\"BetID\":\"444001404563144705\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0OU443600785004355584OMM\",\"EventDate\":\"2023-05-10T23:30:00\",\"EventTypeID\":0,\"EventTypeName\":\"FT O/U\",\"GameID\":443600782814953472,\"HomeTeam\":\"New York Knicks\",\"IsLive\":0,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":3,\"LineTypeName\":\"O/U\",\"LiveScore1\":0,\"LiveScore2\":0,\"Odds\":-110,\"OddsDec\":1.91,\"OddsInUserStyle\":\"1.91\",\"Points\":209.0,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"None\",\"Status\":\"Opened\",\"UpdateDate\":null,\"YourBet\":\"Over 209\",\"Isfreebet\":0}],\"SystemName\":\"0/1\",\"TotalStake\":7.0,\"UpdateDate\":\"2023-05-10T05:03:04.111\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":0}"
                ));

        var service = new ScheduleHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Open),
                clientStatus: Arg.Is(RoundClientStatus.Open),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
