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

public class SmmaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly SlidingBuffer<float> buffer;

    private int index = 0;
    private double lastSmma = 0;
    private double sum = 0;
    private double prevsum = 0;
    private double prevsmma = 0;

    public SmmaIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
        buffer = new SlidingBuffer<float>(period);
    }

    public bool IsPrimed => buffer.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var price = candle.GetRate(Pair, RateToUse);

        buffer.Add(price);

        double smma;

        if (index++ <= Period)
        {
            sum = buffer.Sum();

            lastSmma = sum / Period;

            smma = lastSmma;
        }
        else
        {
            prevsum = sum;

            prevsmma = lastSmma;

            smma = (prevsum - prevsmma + price) / Period;

            sum = prevsum - prevsmma + price;

            lastSmma = (sum - prevsmma + price) / Period;
        }

        return GetBasicResult(candle.OpenOn, smma);
    }
}