// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using System.Collections.Generic;
using System.Linq;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.Testing
{
    internal static class IndicatorData
    {
        public static List<ICandle> GetCandles()
        {
            var tickSet = new TickSet(Source.Dukascopy,
                Known.Pairs[Symbol.EURUSD], new DateOnly(2020, 1, 6));

            tickSet.LoadFromStream(
                DC_EURUSD_20200106_EST_STS.ToStream(), DataKind.STS);

            var candles = new List<ICandle>();

            var feed = new IntervalFeed(
                tickSet.Pair, tickSet.Session, 300);

            feed.OnCandle += (s, e) => candles.Add(e.Candle);

            var ticks = tickSet.ToList();

            foreach (var tick in tickSet.Take(tickSet.Count - 1))
                feed.HandleTick(tick);

            feed.RaiseOnCandleAndReset(ticks.Last());

            return candles;
        }
    }
}