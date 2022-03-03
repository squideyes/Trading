// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SquidEyes.UnitTests.FxData;

public class MetaTickSetTests
{
    [Theory]
    [InlineData(Extent.Day, true)]
    [InlineData(Extent.Day, false)]
    [InlineData(Extent.Week, true)]
    [InlineData(Extent.Week, false)]
    public void ConstructsWithGoodArgs(Extent extent, bool withJpy)
    {
        var session = new Session(Extent.Week, new DateOnly(2020, 1, 6));

        var pairs = new HashSet<Pair>() { Known.Pairs[Symbol.EURUSD] };

        if (withJpy)
            pairs.Add(Known.Pairs[Symbol.EURJPY]);

        var metaTicks = new MetaTickSet(Source.SquidEyes, session, pairs);

        List<TickSet> GetTickSets(int days, int dataId)
        {
            var tickSets = new List<TickSet>();

            foreach (var pair in metaTicks.Pairs.Keys)
                tickSets.Add(GetTickSet(pair, session, days, dataId));

            return tickSets;
        };

        metaTicks.Add(GetTickSets(0, 1));

        if (extent == Extent.Week)
        {
            metaTicks.Add(GetTickSets(1, 2));
            metaTicks.Add(GetTickSets(2, 3));
            metaTicks.Add(GetTickSets(4, 4));
        }

        if (withJpy)
        {
            metaTicks.Pairs.Should().HaveCount(3);
            metaTicks.Pairs.Should().ContainInOrder(
                new Dictionary<Pair, bool>() {
                    { Known.Pairs[Symbol.EURUSD], true },
                    { Known.Pairs[Symbol.EURJPY], true },
                    { Known.Pairs[Symbol.USDJPY], false } });
            metaTicks.Count().Should().Be(extent == Extent.Day ? 18 : 72);
        }
        else
        {
            metaTicks.Pairs.Should().HaveCount(1);
            metaTicks.Pairs.Should().ContainInOrder(
                new Dictionary<Pair, bool>() {
                    { Known.Pairs[Symbol.EURUSD], true } });
            metaTicks.Count().Should().Be(extent == Extent.Day ? 6 : 24);
        }
    }

    [Fact]
    public void EnumeratesMultipleTickSetPairs()
    {
        static string GetString(int index)
        {
            return index switch
            {
                0 => "EURJPY,01/05/2020 17:00:00.000,1,2",
                1 => "USDJPY,01/05/2020 17:00:01.000,2,3",
                2 => "EURUSD,01/05/2020 17:00:02.000,3,4",
                3 => "EURUSD,01/05/2020 17:00:03.000,5,6",
                4 => "EURJPY,01/05/2020 17:00:03.000,4,5",
                5 => "USDJPY,01/05/2020 17:00:04.000,6,7",
                6 => "EURJPY,01/05/2020 17:00:04.000,7,8",
                7 => "EURJPY,01/05/2020 17:00:05.000,8,9",
                8 => "USDJPY,01/05/2020 17:00:05.000,9,10",
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }

        var tradeDate = new DateOnly(2020, 1, 6);

        var session = new Session(Extent.Day, tradeDate);

        var pairs = new HashSet<Pair>()
        {
            Known.Pairs[Symbol.USDJPY],
            Known.Pairs[Symbol.EURJPY],
            Known.Pairs[Symbol.EURUSD]
        };

        var metaTickSet = new MetaTickSet(Source.SquidEyes, session, pairs);

        var eurJpy = new TickSet(
            Source.SquidEyes, Known.Pairs[Symbol.EURJPY], tradeDate);

        var usdJpy = new TickSet(
            Source.SquidEyes, Known.Pairs[Symbol.USDJPY], tradeDate);

        var eurUsd = new TickSet(
            Source.SquidEyes, Known.Pairs[Symbol.EURUSD], tradeDate);

        eurJpy.Add(GetTick(0, 0, 1));
        usdJpy.Add(GetTick(0, 1, 2));
        eurUsd.Add(GetTick(0, 2, 3));
        eurJpy.Add(GetTick(0, 3, 4));
        eurUsd.Add(GetTick(0, 3, 5));
        usdJpy.Add(GetTick(0, 4, 6));
        eurJpy.Add(GetTick(0, 4, 7));
        eurJpy.Add(GetTick(0, 5, 8));
        usdJpy.Add(GetTick(0, 5, 9));

        metaTickSet.Add(new List<TickSet> { eurJpy, usdJpy, eurUsd });

        var metaTicks = metaTickSet.ToList();

        for (var i = 0; i < metaTicks.Count; i++)
            metaTicks[i].ToString().Should().Be(GetString(i));
    }

    private static Tick GetTick(int days, int seconds, int bid)
    {
        TickOn tickOn = new DateTime(
            2020, 1, 5 + days, 17, 0, seconds, DateTimeKind.Unspecified);

        return new Tick(tickOn, bid, bid + 1);
    }

    private static TickSet GetTickSet(
        Pair pair, Session session, int days, int dataId)
    {
        var tickSet = new TickSet(
            Source.SquidEyes, pair, session.TradeDate.AddDays(days));

        switch (dataId)
        {
            case 1:
                tickSet.Add(GetTick(days, 0, 7));
                tickSet.Add(GetTick(days, 1, 8));
                tickSet.Add(GetTick(days, 2, 9));
                tickSet.Add(GetTick(days, 3, 6));
                tickSet.Add(GetTick(days, 4, 14));
                tickSet.Add(GetTick(days, 5, 15));
                break;

            case 2:
                tickSet.Add(GetTick(days, 0, 7));
                tickSet.Add(GetTick(days, 1, 8));
                tickSet.Add(GetTick(days, 2, 9));
                tickSet.Add(GetTick(days, 3, 14));
                tickSet.Add(GetTick(days, 4, 6));
                tickSet.Add(GetTick(days, 5, 5));
                break;

            case 3:
                tickSet.Add(GetTick(days, 0, 12));
                tickSet.Add(GetTick(days, 1, 11));
                tickSet.Add(GetTick(days, 2, 10));
                tickSet.Add(GetTick(days, 3, 13));
                tickSet.Add(GetTick(days, 4, 5));
                tickSet.Add(GetTick(days, 5, 4));
                break;

            case 4:
                tickSet.Add(GetTick(days, 0, 12));
                tickSet.Add(GetTick(days, 1, 11));
                tickSet.Add(GetTick(days, 2, 10));
                tickSet.Add(GetTick(days, 3, 5));
                tickSet.Add(GetTick(days, 4, 13));
                tickSet.Add(GetTick(days, 5, 14));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(dataId));
        }

        return tickSet;
    }
}