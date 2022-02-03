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

public class TemaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly EmaIndicator ema1;
    private readonly EmaIndicator ema2;
    private readonly EmaIndicator ema3;

    public TemaIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
        ema1 = new EmaIndicator(period, pair, rateToUse);
        ema2 = new EmaIndicator(period, pair, rateToUse);
        ema3 = new EmaIndicator(period, pair, rateToUse);
    }

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var value1 = ema1.AddAndCalc(candle).Value;
        var dataPoint1 = GetBasicResult(candle.OpenOn, value1);

        var value2 = ema2.AddAndCalc(dataPoint1).Value;
        var dataPoint2 = GetBasicResult(candle.OpenOn, value2);

        var value3 = ema3.AddAndCalc(dataPoint2).Value;
        var tema = (3 * value1) - (3 * value2) + value3;

        return GetBasicResult(candle.OpenOn, tema);
    }
}