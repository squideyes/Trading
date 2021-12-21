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
        [InlineData(true)]
        public void TickEnumeratorShouldEnumerateCorrectly(bool withJpy)
        {
            var tradeDate = new DateOnly(2020, 1, 6);

            var session = new Session(Extent.Day, tradeDate);

            List<TickSet> GetTickSets(Symbol symbol)
            {
                var tickSets = new List<TickSet>
                {
                    GetTickSet(symbol, tradeDate, 1),
                    GetTickSet(symbol, tradeDate, 2),
                    GetTickSet(symbol, tradeDate, 3),
                    GetTickSet(symbol, tradeDate, 4)
                };

                return tickSets;
            };

            var metaTickSet = new MetaTickSet(session);

            metaTickSet.Add(GetTickSets(Symbol.EURUSD));

            if (withJpy)
            {
                metaTickSet.Add(GetTickSets(Symbol.EURJPY));
                metaTickSet.Add(GetTickSets(Symbol.USDJPY));
            }

            metaTickSet.Count().Should().Be(72);

            foreach (var metaTick in metaTickSet)
            {
                System.Diagnostics.Debug.WriteLine(metaTick);
            }
        }

        private static TickSet GetTickSet(
            Symbol symbol, DateOnly tradeDate, int dataId)
        {
            static Tick GetTick(int seconds, int bid)
            {
                TickOn tickOn = new DateTime(
                    2020, 1, 5, 17, 0, seconds, DateTimeKind.Unspecified);

                return new Tick(tickOn, new Rate(bid), new Rate(bid + 1));
            }

            var tickSet = new TickSet(
                Source.SquidEyes, Known.Pairs[symbol], tradeDate);

            switch (dataId)
            {
                case 1:
                    tickSet.Add(GetTick(0, 7));
                    tickSet.Add(GetTick(1, 8));
                    tickSet.Add(GetTick(2, 9));
                    tickSet.Add(GetTick(3, 6));
                    tickSet.Add(GetTick(4, 14));
                    tickSet.Add(GetTick(5, 15));
                    break;
                case 2:
                    tickSet.Add(GetTick(0, 7));
                    tickSet.Add(GetTick(1, 8));
                    tickSet.Add(GetTick(2, 9));
                    tickSet.Add(GetTick(3, 14));
                    tickSet.Add(GetTick(4, 6));
                    tickSet.Add(GetTick(5, 5));
                    break;
                case 3:
                    tickSet.Add(GetTick(0, 12));
                    tickSet.Add(GetTick(1, 11));
                    tickSet.Add(GetTick(2, 10));
                    tickSet.Add(GetTick(3, 13));
                    tickSet.Add(GetTick(4, 5));
                    tickSet.Add(GetTick(5, 4));
                    break;
                case 4:
                    tickSet.Add(GetTick(0, 12));
                    tickSet.Add(GetTick(1, 11));
                    tickSet.Add(GetTick(2, 10));
                    tickSet.Add(GetTick(3, 5));
                    tickSet.Add(GetTick(4, 13));
                    tickSet.Add(GetTick(5, 14));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataId));
            }

            return tickSet;
        }
    }
}
