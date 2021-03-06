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

public class StdDevIndicator : BasicIndicatorBase, IBasicIndicator
{
    private readonly SlidingBuffer<double> prices;
    private readonly SlidingBuffer<double> sumSeries;

    private int index = 0;

    public StdDevIndicator(int period, Pair pair, RateToUse rateToUse)
        : base(period, pair, rateToUse)
    {
        prices = new SlidingBuffer<double>(period + 1, true);
        sumSeries = new SlidingBuffer<double>(period + 1, true);
    }

    public bool IsPrimed => prices.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        var (openOn, value) = candle.ToBasicResult(Pair, RateToUse)
            .AsFunc(r => (r.OpenOn, r.Value));

        prices.Add(value);

        sumSeries.Add(0.0);

        if (index < 1)
        {
            sumSeries.Update(value);

            index++;

            return GetBasicResult(openOn, 0.0);
        }
        else
        {
            sumSeries.Update(value + sumSeries[1] - (index >= Period ?
                prices[Period] : 0.0));

            var average = sumSeries[0] / Math.Min(index + 1, Period);

            var sum = 0.0;

            for (var barsBack = Math.Min(index, Period - 1); barsBack >= 0; barsBack--)
                sum += (prices[barsBack] - average) * (prices[barsBack] - average);

            var result = GetBasicResult(openOn,
                Math.Sqrt(sum / Math.Min(index + 1, Period)));

            index++;

            return result;
        }
    }
}