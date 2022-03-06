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

public class WmaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly SlidingBuffer<float> buffer;

    private int index = 0;

    private double prevSum;
    private double prevWsum;

    public WmaIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse)
    {
        buffer = new SlidingBuffer<float>(period + 1);
    }

    public bool IsPrimed => buffer.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var price = candle.GetRate(Pair, RateToUse);

        buffer.Add(price);

        var factor = Math.Min(index + 1, Period);

        var wsum = prevWsum -
            (index >= Period ? prevSum : 0.0) + factor * price;

        var sum = prevSum + price -
            (index >= Period ? buffer[0] : 0.0);

        var wma = wsum / (0.5 * factor * (factor + 1));

        index++;

        prevWsum = wsum;
        prevSum = sum;

        return GetBasicResult(candle.OpenOn, wma);
    }
}