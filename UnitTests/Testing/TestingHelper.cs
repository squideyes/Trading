// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Trading.FxData;
using System;
using System.Linq;

namespace SquidEyes.UnitTests.Testing;

internal class TestingHelper<I>
{
    public static void IsPrimedReturnsExpectedValue(I indicator, int period,
        Action<I, ICandle> addAndCalc, Func<I, bool> getIsPrimed)
    {
        var candles = IndicatorData.GetCandles();

        foreach (var candle in candles.Take(period - 1))
        {
            addAndCalc(indicator, candle);

            getIsPrimed(indicator).Should().BeFalse();
        }

        addAndCalc(indicator, candles[period - 1]);

        getIsPrimed(indicator).Should().BeTrue();
    }
}