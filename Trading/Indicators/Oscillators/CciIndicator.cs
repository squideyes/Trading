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

namespace SquidEyes.Trading.Indicators;

public class CciIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly SmaIndicator sma;
    private readonly SlidingBuffer<double> typical;

    private int barCount = 0;

    public CciIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
        ArgumentNullException.ThrowIfNull(pair);

        sma = new SmaIndicator(period, pair, rateToUse);

        typical = new SlidingBuffer<double>(period, true);
    }

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        double result;

        typical.Add(candle.AsFunc(c =>
            (c.High.Value + c.Low.Value + c.Close.Value) / 3.0));

        var sma0 = sma.AddAndCalc(candle.OpenOn, typical[0]).Value;

        if (barCount == 0)
        {
            result = 0;
        }
        else
        {
            var mean = 0.0;

            for (var idx = Math.Min(barCount, Period - 1); idx >= 0; idx--)
                mean += Math.Abs(typical[idx] - sma0);

            result = (typical[0] - sma0) / (mean.Approximates(0.0) ?
                1 : (0.015 * (mean / Math.Min(Period, barCount + 1))));
        }

        barCount++;

        return GetBasicResult(candle.OpenOn, result);
    }
}