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

public static class DataExtenders
{
    public static float GetRate(
        this ICandle candle, Pair pair, RateToUse rateToUse)
    {
        ArgumentNullException.ThrowIfNull(candle);

        return rateToUse switch
        {
            RateToUse.Open => candle.Open.GetFloat(pair.Digits),
            RateToUse.High => candle.High.GetFloat(pair.Digits),
            RateToUse.Low => candle.Low.GetFloat(pair.Digits),
            RateToUse.Close => candle.Close.GetFloat(pair.Digits),
            _ => throw new ArgumentOutOfRangeException(nameof(rateToUse))
        };
    }

    public static BasicResult ToBasicResult(
        this ICandle candle, Pair pair, RateToUse rateToUse)
    {
        ArgumentNullException.ThrowIfNull(candle);
        ArgumentNullException.ThrowIfNull(pair);

        return new()
        {
            OpenOn = candle.OpenOn,
            Value = candle.GetRate(pair, rateToUse)
        };
    }
}