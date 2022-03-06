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

public class BollingerBandsIndictor : BasicIndicatorBase
{
    private readonly SmaIndicator sma;
    private readonly StdDevIndicator stdDev;
    private readonly double stdDevFactor;

    public BollingerBandsIndictor(
        int period, Pair pair, RateToUse rateToUse, double stdDevFactor)
        : base(period, pair, rateToUse)
    {
        if (stdDevFactor <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(stdDevFactor));

        this.stdDevFactor = stdDevFactor;

        sma = new SmaIndicator(period, pair, rateToUse);

        stdDev = new StdDevIndicator(period, pair, rateToUse);
    }

    public bool IsPrimed => sma.IsPrimed;

    public ChannelResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var smaValue = sma.AddAndCalc(candle).Value;

        var stdDevValue = stdDev.AddAndCalc(candle).Value;

        var delta = stdDevFactor * stdDevValue;

        return new ChannelResult()
        {
            OpenOn = candle.OpenOn,
            Upper = smaValue + delta,
            Middle = smaValue,
            Lower = smaValue - delta
        };
    }
}