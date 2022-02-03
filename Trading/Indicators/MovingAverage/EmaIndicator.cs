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

public class EmaIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly double constant1;
    private readonly double constant2;
    private double? lastEma;

    public EmaIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse, 2)
    {
        constant1 = 2.0 / (1 + period);
        constant2 = 1.0 - (2.0 / (1 + period));
    }

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        return AddAndCalc(candle.ToBasicResult(Pair, RateToUse.Close));
    }

    public BasicResult AddAndCalc(BasicResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var ema = !lastEma.HasValue ? result.Value :
            result.Value * constant1 + constant2 * lastEma;

        lastEma = ema;

        return GetBasicResult(result.OpenOn, ema.Value);
    }
}