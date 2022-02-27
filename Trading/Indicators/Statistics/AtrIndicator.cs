// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;

namespace SquidEyes.Trading.Indicators;

public class AtrIndicator : BasicIndicatorBase, IBasicIndicator
{
    private int index = 0;
    private double lastValue = 0.0;
    private ICandle lastCandle = null!;

    public AtrIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
    }

    public bool IsPrimed => index >= Period;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        double high0 = candle.High.Value;
        double low0 = candle.Low.Value;

        if (index++ == 0)
        {
            lastCandle = candle;
            lastValue = high0 - low0;

            return GetBasicResult(candle.OpenOn, lastValue);
        }
        else
        {
            double close1 = lastCandle.Close.Value;

            var trueRange = Math.Max(Math.Abs(low0 - close1),
                Math.Max(high0 - low0, Math.Abs(high0 - close1)));

            var result = ((Math.Min(index, Period) - 1)
                * lastValue + trueRange) / Math.Min(index, Period);

            lastCandle = candle;
            lastValue = result;

            return GetBasicResult(candle.OpenOn, result);
        }
    }
}