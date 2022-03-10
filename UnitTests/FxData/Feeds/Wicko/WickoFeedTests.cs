// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using System.Collections.Generic;
using Xunit;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.FxData;

public class WickoFeedTests
{
    [Fact]
    public void TicksGenerateExpectedCandles()
    {
        var pair = Known.Pairs[Symbol.EURUSD];
        var tradeDate = new DateOnly(2020, 1, 6);
        var session = new Session(Extent.Day, tradeDate);
        var expected = new List<Candle>();
        var countByCandleSetId = new Dictionary<int, int>();

        foreach (var fields in new CsvEnumerator(WickoCandles.ToStream(), 6))
        {
            var openOn = DateTime.Parse(fields[0]);
            var closeOn = DateTime.Parse(fields[1]);
            var open = new Rate(float.Parse(fields[2]), pair.Digits);
            var high = new Rate(float.Parse(fields[3]), pair.Digits);
            var low = new Rate(float.Parse(fields[4]), pair.Digits);
            var close = new Rate(float.Parse(fields[5]), pair.Digits);

            expected.Add(new Candle(openOn, closeOn, open, high, low, close));
        }

        var feed = new WickoFeed(session, 1, MidOrAsk.Mid);

        var index = 0;

        feed.OnCandle += (s, e) =>
        {
            var candle = expected[index];

            e.Candle.OpenOn.Should().Be(candle.OpenOn);
            e.Candle.CloseOn.Should().Be(candle.CloseOn);
            e.Candle.Open.Should().Be(candle.Open);
            e.Candle.High.Should().Be(candle.High);
            e.Candle.Low.Should().Be(candle.Low);
            e.Candle.Close.Should().Be(candle.Close);

            index++;

            if (!countByCandleSetId.ContainsKey(e.CandleSetId))
                countByCandleSetId.Add(e.CandleSetId, 1);
            else
                countByCandleSetId[e.CandleSetId]++;
        };

        foreach (var fields in new CsvEnumerator(WickoTicks.ToStream(), 3))
        {
            var tickOn = DateTime.Parse(fields[0]);
            var bid = new Rate(float.Parse(fields[1]), pair.Digits);
            var ask = new Rate(float.Parse(fields[1]), pair.Digits);

            var tick = new Tick(tickOn, bid, ask);

            feed.HandleTick(tick);
        }

        index.Should().Be(expected.Count);

        feed.OpenCandle.OpenOn.Should().Be(
            new TickOn(new DateTime(2020, 1, 5, 21, 0, 48, 680)));
        feed.OpenCandle.CloseOn.Should().Be(
            new TickOn(new DateTime(2020, 1, 5, 21, 0, 48, 930)));
        feed.OpenCandle.Open.Should().Be(new Rate(105946));
        feed.OpenCandle.High.Should().Be(new Rate(105947));
        feed.OpenCandle.Low.Should().Be(new Rate(105946));
        feed.OpenCandle.Close.Should().Be(new Rate(105946));
    }
}