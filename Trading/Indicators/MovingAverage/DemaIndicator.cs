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

public class DemaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly EmaIndicator ema1;
    private readonly EmaIndicator ema2;

    public DemaIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
        ema1 = new EmaIndicator(period, pair, rateToUse);
        ema2 = new EmaIndicator(period, pair, rateToUse);
    }

    public bool IsPrimed => ema1.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var value1 = ema1.AddAndCalc(candle).Value;

        var result = GetBasicResult(candle.OpenOn, value1);

        var value2 = ema2.AddAndCalc(result).Value;

        var dema = (2.0 * value1) - value2;

        return GetBasicResult(candle.OpenOn, dema);
    }
}