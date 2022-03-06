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
    private readonly SlidingBuffer<double> buffer;

    private int barCount = 0;

    public CciIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse)
    {
        ArgumentNullException.ThrowIfNull(pair);

        sma = new SmaIndicator(period, pair, rateToUse);

        buffer = new SlidingBuffer<double>(period, true);
    }

    public bool IsPrimed => buffer.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        double result;

        buffer.Add(candle.AsFunc(
            c => (c.High.Value + c.Low.Value + c.Close.Value) / 3.0));

        var sma0 = sma.AddAndCalc(candle.OpenOn, buffer[0]).Value;

        if (barCount == 0)
        {
            result = 0;
        }
        else
        {
            var mean = 0.0;

            for (var idx = Math.Min(barCount, Period - 1); idx >= 0; idx--)
                mean += Math.Abs(buffer[idx] - sma0);

            result = (buffer[0] - sma0) / (mean.Approximates(0.0) ?
                1 : (0.015 * (mean / Math.Min(Period, barCount + 1))));
        }

        barCount++;

        return GetBasicResult(candle.OpenOn, result);
    }
}