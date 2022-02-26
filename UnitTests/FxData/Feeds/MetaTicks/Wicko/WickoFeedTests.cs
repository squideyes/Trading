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

        var candles = new List<Candle>();

        foreach (var fields in new CsvEnumerator(WickoCandles.ToStream(), 6))
        {
            var openOn = DateTime.Parse(fields[0]);
            var closeOn = DateTime.Parse(fields[1]);
            var open = new Rate(float.Parse(fields[2]), pair.Digits);
            var high = new Rate(float.Parse(fields[3]), pair.Digits);
            var low = new Rate(float.Parse(fields[4]), pair.Digits);
            var close = new Rate(float.Parse(fields[5]), pair.Digits);

            candles.Add(new Candle(session, openOn, closeOn, open, high, low, close));
        }

        var feed = new WickoFeed(1, RateToUse.Mid);

        var wickos = new List<Wicko>();

        var index = 0;

        feed.OnWicko += (s, e) =>
        {
            var candle = candles[index];

            e.Wicko.Open.Should().Be(candle.Open);
            e.Wicko.High.Should().Be(candle.High);
            e.Wicko.Low.Should().Be(candle.Low);
            e.Wicko.Close.Should().Be(candle.Close);

            index++;

            wickos.Add(e.Wicko);
        };

        foreach (var fields in new CsvEnumerator(WickoTicks.ToStream(), 3))
        {
            var tickOn = DateTime.Parse(fields[0]);
            var bid = new Rate(float.Parse(fields[1]), pair.Digits);
            var ask = new Rate(float.Parse(fields[1]), pair.Digits);

            var tick = new Tick(tickOn, bid, ask);

            feed.HandleTick(tick);
        }
    }
}