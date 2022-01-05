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
    private readonly SmaIndicator smaIndicator;
    private readonly StdDevIndicator stdDevIndicator;
    private readonly double stdDevFactor;

    public BollingerBandsIndictor(
        int period, Pair pair, RateToUse rateToUse, double stdDevFactor)
        : base(period, pair, rateToUse, 2)
    {
        if (stdDevFactor <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(stdDevFactor));

        this.stdDevFactor = stdDevFactor;

        smaIndicator = new SmaIndicator(period, pair, rateToUse);

        stdDevIndicator = new StdDevIndicator(period, pair, rateToUse);
    }

    public ChannelResult AddAndCalc(ICandle candle)
    {
        var smaValue = smaIndicator.AddAndCalc(candle).Value;

        var stdDevValue = stdDevIndicator.AddAndCalc(candle).Value;

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