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
using Xunit;

namespace SquidEyes.UnitTests.FxData;

public class CandleTests
{
    [Fact]
    public void ContructWithGoodArgs() => _ = GetCandle(
        Rate.MaxValue, Rate.MaxValue, Rate.MinValue, Rate.MinValue);

    ////////////////////////////

    [Theory]
    [InlineData(false, true, true, true, true, true, true)]
    [InlineData(true, false, true, true, true, true, true)]
    [InlineData(true, true, false, true, true, true, true)]
    [InlineData(true, true, true, false, true, true, true)]
    [InlineData(true, true, true, true, false, true, true)]
    [InlineData(true, true, true, true, true, false, true)]
    [InlineData(true, true, true, true, true, true, false)]
    public void ConstructWithBadArgs1(bool goodSession, bool goodOpenOn,
        bool goodCloseOn, bool goodOpen, bool goodHigh, bool goodLow, bool goodClose)
    {
        var session = goodSession ? GetSession() : default;
        var openOn = goodSession && goodOpenOn ? session!.MinTickOn : default;
        var closeOn = goodSession && goodCloseOn ? session!.MaxTickOn : default;
        var open = goodOpen ? 3 : default;
        var high = goodHigh ? 3 : default;
        var low = goodLow ? 3 : default;
        var close = goodClose ? 3 : default;

        FluentActions.Invoking(() => _ = new Candle(session!,
            openOn, closeOn, open, high, low, close)).Should().Throw<Exception>();
    }

    //////////////////////////////

    [Theory]
    [InlineData(-72, 0, 3, 3, 2, 2)]
    [InlineData(0, 24, 3, 3, 2, 2)]
    [InlineData(2, -23, 3, 3, 2, 2)]
    [InlineData(0, 0, 3, 2, 3, 2)]
    [InlineData(0, 0, 1, 3, 2, 2)]
    [InlineData(0, 0, 4, 3, 2, 2)]
    [InlineData(0, 0, 3, 3, 2, 1)]
    [InlineData(0, 0, 3, 3, 2, 4)]
    public void ConstructWithBadArgs2(int openOnHours,
        int closeOnHours, int open, int high, int low, int close)
    {
        var session = GetSession();
        var openOn = session!.MinTickOn.Value.AddHours(openOnHours);
        var closeOn = session!.MaxTickOn.Value.AddHours(closeOnHours);

        FluentActions.Invoking(() => _ = new Candle(
            session!, openOn, closeOn, open, high, low, close))
                .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////////

    [Fact]
    public void CloneReturnsMemberwiseClone()
    {
        var source = GetCandle(3, 4, 1, 2);
        var target = source.Clone();

        source.Equals(target).Should().BeTrue();
    }

    //////////////////////////////

    [Fact]
    public void ToCsvStringWithoutPair() =>
        GetCandle(3, 4, 1, 2).AsFunc(c => c.ToCsvString().Should().Be(c.ToString()));

    //////////////////////////////

    [Fact]
    public void ToCsvStringWithPair()
    {
        var pair = Known.Pairs[Symbol.EURUSD];

        GetCandle(3, 4, 1, 2).AsAction(
            c => c.ToCsvString(pair).Should().Be(c.ToString(pair)));
    }

    //////////////////////////////

    [Fact]
    public void ToStringWithoutPair()
    {
        GetCandle(3, 4, 1, 2).AsFunc(x => x.ToString().Should().Be(
            "01/05/2020 17:00:00.000,01/06/2020 16:59:59.999,3,4,1,2"));
    }

    //////////////////////////////

    [Fact]
    public void ToStringWithPair()
    {
        var pair = Known.Pairs[Symbol.EURUSD];

        GetCandle(3, 4, 1, 2).AsFunc(x => x.ToString(pair).Should().Be(
            "01/05/2020 17:00:00.000,01/06/2020 16:59:59.999,0.00003,0.00004,0.00001,0.00002"));
    }

    //////////////////////////////

    [Theory]
    [InlineData(true, true, 4, 5, 2, 3, true)]
    [InlineData(true, false, 4, 5, 2, 3, false)]
    [InlineData(false, true, 4, 5, 2, 3, false)]
    [InlineData(false, false, 4, 5, 2, 3, true)]
    [InlineData(false, false, 3, 5, 2, 3, false)]
    [InlineData(false, false, 4, 6, 2, 3, false)]
    [InlineData(false, false, 4, 5, 1, 3, false)]
    [InlineData(false, false, 4, 5, 2, 4, false)]
    public void ClassEqualsOperator(bool leftNull, bool rightNull,
        int open, int high, int low, int close, bool result)
    {
        var left = leftNull ? null : GetCandle(4, 5, 2, 3);
        var right = rightNull ? null : GetCandle(open, high, low, close);

        (left! == right!).Should().Be(result);
    }

    //////////////////////////////

    [Theory]
    [InlineData(true, true, 4, 5, 2, 3, false)]
    [InlineData(true, false, 4, 5, 2, 3, true)]
    [InlineData(false, true, 4, 5, 2, 3, true)]
    [InlineData(false, false, 4, 5, 2, 3, false)]
    [InlineData(false, false, 3, 5, 2, 3, true)]
    [InlineData(false, false, 4, 6, 2, 3, true)]
    [InlineData(false, false, 4, 5, 1, 3, true)]
    [InlineData(false, false, 4, 5, 2, 4, true)]
    public void ClassNotEqualsOperator(bool leftNull, bool rightNull,
        int open, int high, int low, int close, bool result)
    {
        var left = leftNull ? null : GetCandle(open, high, low, close);
        var right = rightNull ? null : GetCandle(4, 5, 2, 3);

        (left! != right!).Should().Be(result);
    }

    //////////////////////////////

    [Theory]
    [InlineData(true, 4, 5, 2, 3, false)]
    [InlineData(false, 4, 5, 2, 3, true)]
    [InlineData(false, 3, 5, 2, 3, false)]
    [InlineData(false, 4, 6, 2, 3, false)]
    [InlineData(false, 4, 5, 1, 3, false)]
    [InlineData(false, 4, 5, 2, 4, false)]
    public void ClassEqualsMethod(bool rightNull,
        int open, int high, int low, int close, bool result)
    {
        var left = GetCandle(4, 5, 2, 3);
        var right = rightNull ? null : GetCandle(open, high, low, close);

        left!.Equals(right!).Should().Be(result);
    }

    //////////////////////////////

    [Theory]
    [InlineData(true, 4, 5, 2, 3, false)]
    [InlineData(false, 4, 5, 2, 3, true)]
    [InlineData(false, 3, 5, 2, 3, false)]
    [InlineData(false, 4, 6, 2, 3, false)]
    [InlineData(false, 4, 5, 1, 3, false)]
    [InlineData(false, 4, 5, 2, 4, false)]
    public void ObjectEqualsMethod(bool rightNull,
        int open, int high, int low, int close, bool result)
    {
        var left = GetCandle(4, 5, 2, 3);
        var right = rightNull ? null : GetCandle(open, high, low, close);

        left!.Equals((object)right!).Should().Be(result);
    }

    //////////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        var candle = GetCandle(3, 4, 1, 2);

        var hashCode = HashCode.Combine(candle.OpenOn, candle.CloseOn,
            candle.Open, candle.High, candle.Low, candle.Close);

        candle.GetHashCode().Should().Be(hashCode);
    }

    //////////////////////////////

    [Theory]
    [InlineData(1, 1, 1, 1, Trend.NoTrend)]
    [InlineData(1, 2, 1, 2, Trend.Rising)]
    [InlineData(2, 2, 1, 1, Trend.Falling)]
    public void TrendReturnsExpectedResult(
        int open, int high, int low, int close, Trend expected)
    {
        GetCandle(open, high, low, close).Trend.Should().Be(expected);
    }

    //////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IntervalAdjustDoesIndeedAdjust(bool hasInterval)
    {
        TimeSpan? interval = hasInterval ? TimeSpan.FromSeconds(10) : null;

        var candle = GetCandle(5, 5, 5, 5);

        void AdjustAndValidate(int rate, int open, int high, int low, int close)
        {
            candle.Adjust(new Tick(candle.OpenOn.Value, rate, rate), interval);

            candle.Open.Should().Be(open);
            candle.High.Should().Be(high);
            candle.Low.Should().Be(low);
            candle.Close.Should().Be(close);
        }

        AdjustAndValidate(5, 5, 5, 5, 5);
        AdjustAndValidate(7, 5, 7, 5, 7);
        AdjustAndValidate(3, 5, 7, 3, 3);
        AdjustAndValidate(4, 5, 7, 3, 4);
        AdjustAndValidate(5, 5, 7, 3, 5);
        AdjustAndValidate(6, 5, 7, 3, 6);
    }

    //////////////////////////////

    private static Session GetSession() =>
        new(Extent.Day, new DateOnly(2020, 1, 6));

    private static Candle GetCandle(int open, int high, int low, int close)
    {
        var session = GetSession();

        return new Candle(GetSession(),
            session.MinTickOn, session.MaxTickOn, open, high, low, close);
    }
}