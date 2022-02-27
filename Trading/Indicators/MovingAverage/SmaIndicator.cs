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

public class SmaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly SlidingBuffer<double> buffer;

    private int index = 0;
    private double sum = 0;
    private double priorSum = 0;

    public SmaIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
        buffer = new SlidingBuffer<double>(period + 1);
    }

    public bool IsPrimed => buffer.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        return AddAndCalc(candle.OpenOn, candle.GetRate(Pair, RateToUse));
    }

    public BasicResult AddAndCalc(TickOn openOn, double price)
    {
        buffer.Add(price);

        sum = priorSum + price - (index >= Period ? buffer[0] : 0);

        var sma = sum / (index < Period ? index + 1 : Period);

        priorSum = sum;

        index++;

        return GetBasicResult(openOn, sma);
    }
}