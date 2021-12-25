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
using Xunit;
using static SquidEyes.Trading.Context.Symbol;

namespace SquidEyes.UnitTests.Context;

public class MetaTickTests
{
    [Fact]
    public void ConstructorWithGoodArgs()
    {
        var metaTick = GetMetaTick(EURUSD, 1, 2, 3);

        metaTick.Pair.Should().Be(Known.Pairs[EURUSD]);
        metaTick.Tick.TickOn.Should().Be(GetTickOn(1));
        metaTick.Tick.Bid.Should().Be(new Rate(2));
        metaTick.Tick.Ask.Should().Be(new Rate(3));
    }

    [Theory]
    [InlineData((Symbol)0, 1, 2, 3)]
    [InlineData(EURUSD, -1, 2, 3)]
    [InlineData(EURUSD, 1, 0, 3)]
    [InlineData(EURUSD, 1, 2, 0)]
    public void ConstructorWithBadArgs(Symbol symbol, int seconds, int bid, int ask)
    {
        FluentActions.Invoking(() => _ = GetMetaTick(symbol, seconds, bid, ask))
            .Should().Throw<Exception>();
    }

    [Fact]
    public void ToStringWithoutArgs() => GetMetaTick(EURUSD, 1, 2, 3)
        .ToString().Should().Be("EURUSD,01/06/2020 17:00:01.000,2,3");

    [Fact]
    public void EqualsMethodWithNullType() =>
          GetMetaTick(EURUSD, 0, 1, 2).Equals(null).Should().BeFalse();

    [Fact]
    public void EqualsMethodWithNullObject() =>
        GetMetaTick(EURUSD, 0, 1, 2).Equals((object?)null).Should().BeFalse();

    [Fact]
    public void GetHashCodeDerivesFromPairAndTick()
    {
        var metaTick = GetMetaTick(EURUSD, 1, 2, 3);

        metaTick.GetHashCode().Should().Be(HashCode.Combine(metaTick.Pair, metaTick.Tick));
    }

    [Theory]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 2, 3, true)]
    [InlineData(AUDUSD, 1, 2, 3, EURUSD, 1, 2, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 2, 2, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 1, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 2, 4, false)]
    public void EqualsMethodWithMixedArgs(Symbol symbol1, int seconds1,
        int bid1, int ask1, Symbol symbol2, int seconds2, int bid2, int ask2, bool result)
    {
        var left = GetMetaTick(symbol1, seconds1, bid1, ask1);
        var right = GetMetaTick(symbol2, seconds2, bid2, ask2);

        left.Equals(right).Should().Be(result);
    }

    [Theory]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 2, 3, true)]
    [InlineData(AUDUSD, 1, 2, 3, EURUSD, 1, 2, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 2, 2, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 1, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 2, 4, false)]
    public void EqualsOperatorWithMixedArgs(Symbol symbol1, int seconds1, int bid1,
        int ask1, Symbol symbol2, int seconds2, int bid2, int ask2, bool result)
    {
        var left = GetMetaTick(symbol1, seconds1, bid1, ask1);
        var right = GetMetaTick(symbol2, seconds2, bid2, ask2);

        (left == right).Should().Be(result);
    }

    [Theory]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 2, 3, false)]
    [InlineData(AUDUSD, 1, 2, 3, EURUSD, 1, 2, 3, true)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 2, 2, 3, true)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 1, 3, true)]
    [InlineData(AUDUSD, 1, 2, 3, AUDUSD, 1, 2, 4, true)]
    public void NotEqualsOperatorWithMixedArgs(Symbol symbol1, int seconds1, int bid1,
        int ask1, Symbol symbol2, int seconds2, int bid2, int ask2, bool result)
    {
        var left = GetMetaTick(symbol1, seconds1, bid1, ask1);
        var right = GetMetaTick(symbol2, seconds2, bid2, ask2);

        (left != right).Should().Be(result);
    }

    private static TickOn GetTickOn(int seconds) =>
        new DateTime(2020, 1, 6, 17, 0, seconds, DateTimeKind.Unspecified);

    private static MetaTick GetMetaTick(Symbol symbol, int seconds, int bid, int ask) =>
        new(Known.Pairs[symbol], new Tick(GetTickOn(seconds), new Rate(bid), new Rate(ask)));
}
