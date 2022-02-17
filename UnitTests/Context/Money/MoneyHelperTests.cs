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
using SquidEyes.Trading.Orders;
using System.Collections.Generic;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class MoneyHelperTests
{
    private class MinMargin : IMinMargin
    {
        private readonly Dictionary<Symbol, double> percents = new()
        {
            { Symbol.AUDUSD, 0.03 },
            { Symbol.EURGBP, 0.03 },
            { Symbol.EURJPY, 0.02 },
            { Symbol.EURUSD, 0.02 },
            { Symbol.GBPUSD, 0.03 },
            { Symbol.NZDUSD, 0.03 },
            { Symbol.USDCAD, 0.02 },
            { Symbol.USDCHF, 0.04 },
            { Symbol.USDJPY, 0.02 }
        };

        public double this[Symbol symbol] => percents[symbol];
        public double this[Pair pair] => percents[pair.Symbol];
    }

    [Theory]
    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 113460, 2269.21)]
    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 111460, 0.0)]
    [InlineData(Symbol.EURUSD, Side.Buy, 114960, 113460, -1701.9)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113480, 567.4)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113980, 0.0)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113380, 113480, -113.47)]
    [InlineData(Symbol.USDCAD, Side.Buy, 126180, 127200, 801.88)]
    [InlineData(Symbol.USDCAD, Side.Buy, 126180, 126180, 0.0)]
    [InlineData(Symbol.USDCAD, Side.Buy, 132180, 127180, -3931.43)]
    [InlineData(Symbol.USDCAD, Side.Sell, 129220, 127220, 1572.08)]
    [InlineData(Symbol.USDCAD, Side.Sell, 129220, 129220, 0.0)]
    [InlineData(Symbol.USDCAD, Side.Sell, 124220, 127220, -2358.12)]
    [InlineData(Symbol.EURJPY, Side.Buy, 105690, 115690, 9808.11)]
    [InlineData(Symbol.EURJPY, Side.Buy, 105690, 105690, 0.0)]
    [InlineData(Symbol.EURJPY, Side.Buy, 130690, 115690, -14712.16)]
    [InlineData(Symbol.EURJPY, Side.Sell, 127690, 115700, 11758.91)]
    [InlineData(Symbol.EURJPY, Side.Sell, 127690, 127690, 0.0)]
    [InlineData(Symbol.EURJPY, Side.Sell, 95690, 115700, -19624.32)]
    public void GetGrossProfitReturnsExpectedValue(
        Symbol symbol, Side side, int entry, int exit, double expected)
    {
        var helper = new MoneyHelper(
            MoneyData.GetUsdValueOf(MidOrAsk.Mid), new MinMargin());

        var actual = helper.GetPNL(
            Known.Pairs[symbol], side, 100000, entry, exit);

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(Symbol.USDJPY, Leverage.FiftyToOne, 0.0, 2000.0)]
    [InlineData(Symbol.USDJPY, Leverage.FiftyToOne, 0.1, 2200.0)]
    [InlineData(Symbol.EURUSD, Leverage.FiftyToOne, 0.0, 2269.4)]
    [InlineData(Symbol.EURUSD, Leverage.FiftyToOne, 0.1, 2496.34)]
    [InlineData(Symbol.EURGBP, Leverage.FiftyToOne, 0.0, 3404.1)]
    [InlineData(Symbol.EURGBP, Leverage.FiftyToOne, 0.1, 3744.51)]
    [InlineData(Symbol.EURJPY, Leverage.FiftyToOne, 0.0, 2269.4)]
    [InlineData(Symbol.EURJPY, Leverage.FiftyToOne, 0.1, 2496.34)]
    public void GetMarginInUsdReturnsExpectedValue(
        Symbol symbol, Leverage leverage, double cushion, double expected)
    {
        var helper = new MoneyHelper(
            MoneyData.GetUsdValueOf(MidOrAsk.Mid), new MinMargin());

        var pair = Known.Pairs[symbol]; 

        var actual = helper.GetMarginInUsd(pair, 100000, leverage, cushion);

        actual.Should().Be(expected);
    }
}