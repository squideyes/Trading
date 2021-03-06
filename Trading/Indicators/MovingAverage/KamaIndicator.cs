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

public class KamaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly double fastCF;
    private readonly double slowCF;
    private readonly SlidingBuffer<double> diffs;
    private readonly SlidingBuffer<double> values;

    private double lastResult = 0;

    private int index = 0;

    public KamaIndicator(int period, int fast, int slow, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse)
    {
        fastCF = 2.0 / (fast + 1);
        slowCF = 2.0 / (slow + 1);

        diffs = new SlidingBuffer<double>(period + 1, true);
        values = new SlidingBuffer<double>(period + 1, true);
    }

    public bool IsPrimed => values.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var dataPoint = candle.ToBasicResult(Pair, RateToUse);

        BasicResult UpdateIndexAndLastResultThenGetResult(double value)
        {
            index++;

            return GetBasicResult(candle.OpenOn, lastResult = value);
        }

        values.Add(dataPoint.Value);

        diffs.Add(index > 0 ? Math.Abs(dataPoint.Value - values[1]) : dataPoint.Value);

        if (index < Period)
            return UpdateIndexAndLastResultThenGetResult(dataPoint.Value);

        var signal = Math.Abs(dataPoint.Value - values[Period]);

        var noise = diffs.Take(Period).Sum();

        if (noise == 0.0)
            return UpdateIndexAndLastResultThenGetResult(dataPoint.Value);

        var result = lastResult + Math.Pow(signal / noise *
            (fastCF - slowCF) + slowCF, 2) * (dataPoint.Value - lastResult);

        return UpdateIndexAndLastResultThenGetResult(result);
    }
}