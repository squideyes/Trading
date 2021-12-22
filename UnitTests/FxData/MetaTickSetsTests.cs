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

namespace SquidEyes.UnitTests.FxData
{
    public class MetaTickSetTests
    {
        [Theory]
        [InlineData(Extent.Day, true)]
        [InlineData(Extent.Day, false)]
        [InlineData(Extent.Week, true)]
        [InlineData(Extent.Week, false)]
        public void ConstructsWithGoodArgs(Extent extent, bool withJpy)
        {
            var tradeDate = new DateOnly(2020, 1, 6);

            var session = new Session(Extent.Week, tradeDate);

            List<TickSet> GetTickSets(Symbol symbol)
            {
                var tickSet = new List<TickSet>()
                {
                    GetTickSet(symbol, tradeDate, 0, 1),
                };

                if (extent == Extent.Week)
                {
                    tickSet.Add(GetTickSet(symbol, tradeDate, 1, 2));
                    tickSet.Add(GetTickSet(symbol, tradeDate, 2, 3));
                    tickSet.Add(GetTickSet(symbol, tradeDate, 4, 4));
                }

                return tickSet;
            };

            var metaTickSet = new MetaTickSet(Source.SquidEyes, session)
            {
                GetTickSets(Symbol.EURUSD)
            };

            if (withJpy)
            {
                metaTickSet.Add(GetTickSets(Symbol.EURJPY));
                metaTickSet.Add(GetTickSets(Symbol.USDJPY));
            }

            var count = extent == Extent.Week ? 24 : 6;

            metaTickSet.Count().Should().Be(withJpy ? count * 3 : count);
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
                    5 => "EURJPY,01/05/2020 17:00:04.000,7,8",
                    6 => "USDJPY,01/05/2020 17:00:04.000,6,7",
                    7 => "USDJPY,01/05/2020 17:00:05.000,9,10",
                    8 => "EURJPY,01/05/2020 17:00:05.000,8,9",
                    _ => throw new ArgumentOutOfRangeException(nameof(index))
                };
            }

            var tradeDate = new DateOnly(2020, 1, 6);

            var session = new Session(Extent.Day, tradeDate);

            var metaTickSet = new MetaTickSet(Source.SquidEyes, session);

            var tickSet1 = new TickSet(Source.SquidEyes, Known.Pairs[Symbol.EURJPY], tradeDate);
            var tickSet2 = new TickSet(Source.SquidEyes, Known.Pairs[Symbol.USDJPY], tradeDate);
            var tickSet3 = new TickSet(Source.SquidEyes, Known.Pairs[Symbol.EURUSD], tradeDate);

            tickSet1.Add(GetTick(0, 0, 1));
            tickSet2.Add(GetTick(0, 1, 2));
            tickSet3.Add(GetTick(0, 2, 3));
            tickSet1.Add(GetTick(0, 3, 4));
            tickSet3.Add(GetTick(0, 3, 5));
            tickSet2.Add(GetTick(0, 4, 6));
            tickSet1.Add(GetTick(0, 4, 7));
            tickSet1.Add(GetTick(0, 5, 8));
            tickSet2.Add(GetTick(0, 5, 9));

            metaTickSet.Add(new List<TickSet> { tickSet1 });
            metaTickSet.Add(new List<TickSet> { tickSet2 });
            metaTickSet.Add(new List<TickSet> { tickSet3 });

            var metaTicks = metaTickSet.ToList();

            for (var i = 0; i < metaTicks.Count; i++)
                metaTicks[i].ToString().Should().Be(GetString(i));
        }

        private static Tick GetTick(int days, int seconds, int bid)
        {
            TickOn tickOn = new DateTime(
                2020, 1, 5 + days, 17, 0, seconds, DateTimeKind.Unspecified);

            return new Tick(tickOn, new Rate(bid), new Rate(bid + 1));
        }

        private static TickSet GetTickSet(
            Symbol symbol, DateOnly tradeDate, int days, int dataId)
        {
            var tickSet = new TickSet(
                Source.SquidEyes, Known.Pairs[symbol], tradeDate.AddDays(days));

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
}
