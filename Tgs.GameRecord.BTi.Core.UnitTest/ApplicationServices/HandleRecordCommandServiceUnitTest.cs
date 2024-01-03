using System;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameRecord.BTi.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.BTi.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Settlement_Canceled()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book",
            GameProviderConfiguration = "BTIConfiguration",
            HangfirePrefix = "gamerecord-bti-hangfire:",
            HangfireLocking = "gamerecord-bti-locking:",
            IntervalMinutes = 15,
            IntervalMinutesMax = 30,
            IntervalMinutesMinimum = 5
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Info: new GameWagerInfo
            {
                PlayerCode = "4uispltzj9d8",
                ExternalWagerId = "491416857069453312",
                IsClosed = true,
                Status = GameHistoryStatus.Canceled,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-09-18T01:15:08.8").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-09-19T12:36:22.327").ToTimestamp(),
                Bet = 50.0m,
                ValidBet = 0.0m,
                Settle = 50.0m,
                WinLoss = 0.0m,
                AdditionalData = "{\"BetSettledDate\":\"2023-09-19T12:36:22.327\",\"BetStatus\":\"Canceled\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPISO\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-09-18T01:15:08.8\",\"Currency\":\"PHP\",\"CustomerID\":124440379,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4ukukz9lsn0e\",\"NonCashOutAmount\":0.0,\"Odds\":-156,\"OddsDec\":1.64,\"OddsInUserStyle\":\"1.64\",\"OddsStyleOfUser\":\"European\",\"PL\":0.0,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"491416857069453312\",\"RealMoneyAmount\":50.0,\"Return\":50.0,\"SearchDateTime\":\"2023-09-19T12:44:25.214\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Lin Zhu\",\"BetID\":\"491416857069453313\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":6,\"BranchName\":\"Tennis\",\"CouponCode\":\"\",\"EncodedLineID\":\"0ML491219173364744204A\",\"EventDate\":\"2023-09-18T02:00:00\",\"EventTypeID\":0,\"EventTypeName\":\"FT Winner\",\"GameID\":491219172865658880,\"HomeTeam\":\"Anna Kalinskaya\",\"IsLive\":0,\"IsNewLine\":0,\"LeagueID\":490605751149318144,\"LeagueName\":\"WTA Guangzhou\",\"LineID\":0,\"LineTypeID\":1,\"LineTypeName\":\"Winner\",\"LiveScore1\":0,\"LiveScore2\":0,\"Odds\":-156,\"OddsDec\":1.64,\"OddsInUserStyle\":\"1.64\",\"Points\":0.0,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"None\",\"Status\":\"Canceled\",\"UpdateDate\":null,\"YourBet\":\"Lin Zhu\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Canceled\",\"SystemName\":\"0/1\",\"TotalStake\":50.0,\"UpdateDate\":\"2023-09-19T12:36:22.327\",\"UserName\":\"4ukukz9lsn0e\",\"ValidStake\":0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Info.PlayerCode),
            wagerState: Arg.Is(WagerStates.Refund),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Info.ExternalWagerId),
            externalWagerId: Arg.Is(command.Info.ExternalWagerId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.00m),
            turnover: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Info.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void Settlement_Cashout()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book",
            GameProviderConfiguration = "BTIConfiguration",
            HangfirePrefix = "gamerecord-bti-hangfire:",
            HangfireLocking = "gamerecord-bti-locking:",
            IntervalMinutes = 15,
            IntervalMinutesMax = 30,
            IntervalMinutesMinimum = 5
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Info: new GameWagerInfo
            {
                PlayerCode = "4uispltzj9d8",
                ExternalWagerId = "643583c37c18c573b57bbb2c",
                IsClosed = true,
                Status = GameHistoryStatus.Cashout,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-10T04:38:56.602").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-05-10T04:44:56.009").ToTimestamp(),
                Bet = 4m,
                ValidBet = 0m,
                Settle = 4m,
                WinLoss = 0m,
                AdditionalData = "{\"BetSettledDate\":\"2023-05-10T04:38:56.602\",\"BetStatus\":\"Won\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T04:12:18.11\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"OddsStyleOfUser\":\"European\",\"PL\":4.24,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"443988629220675584\",\"RealMoneyAmount\":4.0,\"Return\":8.24,\"SearchDateTime\":\"2023-05-10T04:44:56.009\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Phoenix Suns\",\"BetID\":\"443988629220675585\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0HC443242889859137536AMM\",\"EventDate\":\"2023-05-10T02:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting Spread\",\"GameID\":443242887480950784,\"HomeTeam\":\"Denver Nuggets\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":2,\"LineTypeName\":\"Spread\",\"LiveScore1\":104,\"LiveScore2\":80,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"Points\":19.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"118 : 102\",\"Status\":\"Won\",\"UpdateDate\":null,\"YourBet\":\"Phoenix Suns \\u002B19.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Won\",\"SystemName\":\"0/1\",\"TotalStake\":4.0,\"UpdateDate\":\"2023-05-10T04:38:56.602\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":4.0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Info.PlayerCode),
            wagerState: Arg.Is(WagerStates.Refund),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.Cashout),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Info.ExternalWagerId),
            externalWagerId: Arg.Is(command.Info.ExternalWagerId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.00m),
            turnover: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Info.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void Settlement_Loss()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book",
            GameProviderConfiguration = "BTIConfiguration",
            HangfirePrefix = "gamerecord-bti-hangfire:",
            HangfireLocking = "gamerecord-bti-locking:",
            IntervalMinutes = 15,
            IntervalMinutesMax = 30,
            IntervalMinutesMinimum = 5
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Info: new GameWagerInfo
            {
                PlayerCode = "4uispltzj9d8",
                ExternalWagerId = "444000837900083200",
                IsClosed = true,
                Status = GameHistoryStatus.Lost,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-10T04:38:56.602").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-05-10T04:44:56.009").ToTimestamp(),
                Bet = 8m,
                ValidBet = 8m,
                Settle = 0m,
                WinLoss = -8m,
                AdditionalData = "{\"BetSettledDate\":\"2023-05-10T06:13:16.871\",\"BetStatus\":\"Lost\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T05:00:48.888\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":-114,\"OddsDec\":1.88,\"OddsInUserStyle\":\"1.88\",\"OddsStyleOfUser\":\"European\",\"PL\":-8.0,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"444000837900083200\",\"RealMoneyAmount\":8.0,\"Return\":0.0,\"SearchDateTime\":\"2023-05-10T06:16:36.887\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Indonesia [W]\",\"BetID\":\"444000837900083201\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0OU443743933215096834OMM\",\"EventDate\":\"2023-05-10T04:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting O/U\",\"GameID\":443743932179120128,\"HomeTeam\":\"Thailand [W]\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":443001798798925824,\"LeagueName\":\"SEA Games Women\",\"LineID\":0,\"LineTypeID\":3,\"LineTypeName\":\"O/U\",\"LiveScore1\":30,\"LiveScore2\":44,\"Odds\":-114,\"OddsDec\":1.88,\"OddsInUserStyle\":\"1.88\",\"Points\":141.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"69 : 70\",\"Status\":\"Lost\",\"UpdateDate\":null,\"YourBet\":\"Over 141.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Lost\",\"SystemName\":\"0/1\",\"TotalStake\":8.0,\"UpdateDate\":\"2023-05-10T06:13:16.871\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":8.0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Info.PlayerCode),
            wagerState: Arg.Is(WagerStates.Lose),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Info.ExternalWagerId),
            externalWagerId: Arg.Is(command.Info.ExternalWagerId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.00m),
            turnover: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Info.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void Settlement_Win()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book",
            GameProviderConfiguration = "BTIConfiguration",
            HangfirePrefix = "gamerecord-bti-hangfire:",
            HangfireLocking = "gamerecord-bti-locking:",
            IntervalMinutes = 15,
            IntervalMinutesMax = 30,
            IntervalMinutesMinimum = 5
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Info: new GameWagerInfo
            {
                PlayerCode = "4uispltzj9d8",
                ExternalWagerId = "643583c37c18c573b57bbb2c",
                IsClosed = true,
                Status = GameHistoryStatus.Won,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-10T04:38:56.602").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-05-10T04:44:56.009").ToTimestamp(),
                Bet = 4m,
                ValidBet = 4m,
                Settle = 8.24m,
                WinLoss = 4.24m,
                AdditionalData = "{\"BetSettledDate\":\"2023-05-10T04:38:56.602\",\"BetStatus\":\"Won\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T04:12:18.11\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"OddsStyleOfUser\":\"European\",\"PL\":4.24,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"443988629220675584\",\"RealMoneyAmount\":4.0,\"Return\":8.24,\"SearchDateTime\":\"2023-05-10T04:44:56.009\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Phoenix Suns\",\"BetID\":\"443988629220675585\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0HC443242889859137536AMM\",\"EventDate\":\"2023-05-10T02:00:00\",\"EventTypeID\":39,\"EventTypeName\":\"Live Betting Spread\",\"GameID\":443242887480950784,\"HomeTeam\":\"Denver Nuggets\",\"IsLive\":1,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":2,\"LineTypeName\":\"Spread\",\"LiveScore1\":104,\"LiveScore2\":80,\"Odds\":106,\"OddsDec\":2.06,\"OddsInUserStyle\":\"2.06\",\"Points\":19.5,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"118 : 102\",\"Status\":\"Won\",\"UpdateDate\":null,\"YourBet\":\"Phoenix Suns \\u002B19.5\",\"Isfreebet\":0,\"Isresettled\":0}],\"Status\":\"Won\",\"SystemName\":\"0/1\",\"TotalStake\":4.0,\"UpdateDate\":\"2023-05-10T04:38:56.602\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":4.0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Info.PlayerCode),
            wagerState: Arg.Is(WagerStates.Win),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Info.ExternalWagerId),
            externalWagerId: Arg.Is(command.Info.ExternalWagerId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.00m),
            turnover: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Info.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void Unsettlement()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "BTI",
            GameExternalId = "sports_book",
            GameProviderConfiguration = "BTIConfiguration",
            HangfirePrefix = "gamerecord-bti-hangfire:",
            HangfireLocking = "gamerecord-bti-locking:",
            IntervalMinutes = 15,
            IntervalMinutesMax = 30,
            IntervalMinutesMinimum = 5
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Info: new GameWagerInfo
            {
                PlayerCode = "4uispltzj9d8",
                ExternalWagerId = "643583c37c18c573b57bbb2c",
                IsClosed = false,
                Status = GameHistoryStatus.Opened,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-10T05:03:04.111").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
                Bet = 7m,
                ValidBet = 7m,
                Settle = 0m,
                WinLoss = 0m,
                AdditionalData = "{\"BetSettledDate\":\"2023-05-10T05:03:04.111\",\"BetStatus\":\"Opened\",\"BetTypeId\":1,\"BetTypeName\":\"Single bets\",\"Brand\":\"TGSPB1\",\"ComboBonusAmount\":0.0,\"CreationDate\":\"2023-05-10T05:03:03.988\",\"Currency\":\"PHP\",\"CustomerID\":112910928,\"DomainID\":0,\"FreeBet\":{\"Amount\":0.0,\"IsRiskFreeBet\":0},\"Gain\":\"13.37\",\"MerchantCustomerID\":\"4uiq47lurf6z\",\"NonCashOutAmount\":0,\"Odds\":-110,\"OddsDec\":1.91,\"OddsInUserStyle\":\"1.91\",\"OddsStyleOfUser\":\"European\",\"PL\":0.0,\"Platform\":\"Mobile\",\"PlayerLevelID\":0,\"PlayerLevelName\":\"Newly Registered\",\"PurchaseID\":\"444001404563144704\",\"RealMoneyAmount\":7.0,\"Return\":0.0,\"SearchDateTime\":\"2023-05-10T05:08:28.633\",\"Selections\":[{\"ActionType\":\"Real\",\"AwayTeam\":\"Miami Heat\",\"BetID\":\"444001404563144705\",\"BetType\":\"Single bets\",\"BonusID\":0,\"BranchID\":2,\"BranchName\":\"Basketball\",\"CouponCode\":\"\",\"EncodedLineID\":\"0OU443600785004355584OMM\",\"EventDate\":\"2023-05-10T23:30:00\",\"EventTypeID\":0,\"EventTypeName\":\"FT O/U\",\"GameID\":443600782814953472,\"HomeTeam\":\"New York Knicks\",\"IsLive\":0,\"IsNewLine\":0,\"LeagueID\":216474657157816320,\"LeagueName\":\"NBA\",\"LineID\":0,\"LineTypeID\":3,\"LineTypeName\":\"O/U\",\"LiveScore1\":0,\"LiveScore2\":0,\"Odds\":-110,\"OddsDec\":1.91,\"OddsInUserStyle\":\"1.91\",\"Points\":209.0,\"ReferenceID\":0,\"RelatedBetID\":0,\"Score\":\"None\",\"Status\":\"Opened\",\"UpdateDate\":null,\"YourBet\":\"Over 209\",\"Isfreebet\":0}],\"SystemName\":\"0/1\",\"TotalStake\":7.0,\"UpdateDate\":\"2023-05-10T05:03:04.111\",\"UserName\":\"4uiq47lurf6z\",\"ValidStake\":0}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Info.PlayerCode),
            wagerState: Arg.Is(WagerStates.Running),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Info.ExternalWagerId),
            externalWagerId: Arg.Is(command.Info.ExternalWagerId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(DateTimeOffset.UnixEpoch),
            externalPayoffOn: Arg.Is(DateTimeOffset.UnixEpoch),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Info.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(0.00m),
            commission: Arg.Is(0.00m),
            turnover: Arg.Is(0.00m),
            winLoss: Arg.Is(0.00m),
            additionalData: Arg.Is(command.Info.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
