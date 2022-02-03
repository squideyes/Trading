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

public class KeltnerChannelIndictor : BasicIndicatorBase
{
    private readonly double offsetMultiplier;
    private readonly SmaIndicator smaDiff;
    private readonly SmaIndicator smaTypical;
    private readonly SlidingBuffer<double> diff;
    private readonly SlidingBuffer<double> typical;

    public KeltnerChannelIndictor(int period, Pair pair, RateToUse rateToUse, double offsetMultiplier = 1.5)
        : base(period, pair, rateToUse, 2)
    {
        this.offsetMultiplier = offsetMultiplier
            .Validated(nameof(offsetMultiplier), v => v.Between(0.5, 5.0));

        diff = new SlidingBuffer<double>(period, true);
        typical = new SlidingBuffer<double>(period, true);
        smaDiff = new SmaIndicator(period, pair, rateToUse);
        smaTypical = new SmaIndicator(period, pair, rateToUse);
    }

    public ChannelResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        diff.Add(candle.High.Value - candle.Low.Value);

        typical.Add(candle.AsFunc(c => (c.High.Value + c.Low.Value + c.Close.Value) / 3.0));

        var middle = smaTypical.AddAndCalc(
            candle.OpenOn, typical[0]).Value;

        var x = smaDiff.AddAndCalc(candle.OpenOn, diff[0]).Value;

        var offset = x * offsetMultiplier;

        return new ChannelResult()
        {
            OpenOn = candle.OpenOn,
            Upper = middle + offset,
            Middle = middle,
            Lower = middle - offset
        };
    }
}