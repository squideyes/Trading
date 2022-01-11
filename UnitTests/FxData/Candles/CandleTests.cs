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
        var open = goodOpen ? new Rate(3) : default;
        var high = goodHigh ? new Rate(3) : default;
        var low = goodLow ? new Rate(3) : default;
        var close = goodClose ? new Rate(3) : default;

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

        FluentActions.Invoking(() => _ = new Candle(session!, openOn, closeOn,
            new Rate(open), new Rate(high), new Rate(low), new Rate(close)))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////////

    //[Theory]
    //[InlineData(5, 1, 999999, "01/05/2020 17:00:00.000,0.00001,9.99999")]
    //[InlineData(3, 1, 999999, "01/05/2020 17:00:00.000,0.001,999.999")]
    //public void DigitsToCsvString(
    //    int digits, int bidValue, int askValue, string result)
    //{
    //    GetTick(bidValue, askValue)
    //        .AsFunc(x => x.ToCsvString(digits).Should().Be(result));
    //}

    //////////////////////////////

    //[Theory]
    //[InlineData(5, 1, 999999, "01/05/2020 17:00:00.000,0.00001,9.99999")]
    //[InlineData(3, 1, 999999, "01/05/2020 17:00:00.000,0.001,999.999")]
    //public void PairToCsvString(
    //    int digits, int bidValue, int askValue, string result)
    //{
    //    var pair = digits == 5 ?
    //        Known.Pairs[Symbol.EURUSD] : Known.Pairs[Symbol.USDJPY];

    //    GetTick(bidValue, askValue)
    //        .AsFunc(x => x.ToCsvString(pair).Should().Be(result));
    //}

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

    [Fact]
    public void CandleNotEqualToNullCandle() =>
        GetCandle(3, 4, 1, 2).Equals(null).Should().BeFalse();

    //////////////////////////////

    [Fact]
    public void CandleNotEqualToNullObject() =>
        GetCandle(3, 4, 1, 2).Equals((object?)null).Should().BeFalse();

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
    [InlineData(true)]
    [InlineData(false)]
    public void CandleEqualsCandle(bool result)
    {
        var tick1 = GetCandle(3, 4, 1, 2);
        var tick2 = result ? GetCandle(3, 4, 1, 2) : GetCandle(3, 4, 1, 3);

        tick1.Equals(tick2).Should().Be(result);
    }

    //////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CandleEqualsCandleOperator(bool result)
    {
        var candle1 = GetCandle(3, 4, 1, 2);
        var candle2 = result ? GetCandle(3, 4, 1, 2) : GetCandle(3, 4, 1, 3);

        (candle1 == candle2).Should().Be(result);
    }

    //////////////////////////////

    //[Theory]
    //[InlineData(true)]
    //[InlineData(false)]
    //public void TickNotEqualsTickOperator(bool result)
    //{
    //    var tick1 = GetTick(1, 2);
    //    var tick2 = result ? GetTick(2, 3) : GetTick(1, 2);

    //    (tick1 != tick2).Should().Be(result);
    //}

    //////////////////////////////

    private static Session GetSession() =>
        new(Extent.Day, new DateOnly(2020, 1, 6));

    private static Candle GetCandle(int open, int high, int low, int close)
    {
        var session = GetSession();

        return new Candle(GetSession(), session.MinTickOn, session.MaxTickOn,
            new Rate(open), new Rate(high), new Rate(low), new Rate(close));
    }
}