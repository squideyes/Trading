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

public class StochasticsIndicator
{
    private readonly SlidingBuffer<ICandle> candles;
    private readonly SlidingBuffer<(TickOn OpenOn, double Value)> fastK;
    private readonly int periodK;
    private readonly int periodD;
    private readonly int smooth;
    private readonly Pair pair;

    public StochasticsIndicator(int periodK, int periodD, int smooth, Pair pair)
    {
        this.periodK = periodK.Validated(nameof(periodK), v => v >= 2);
        this.periodD = periodD.Validated(nameof(periodD), v => v >= 2);
        this.smooth = smooth.Validated(nameof(smooth), v => v >= 1);
        this.pair = pair;

        candles = new SlidingBuffer<ICandle>(this.periodK, true);

        fastK = new SlidingBuffer<(TickOn, double)>(this.periodK, true);
    }

    public bool IsPrimed => candles.IsPrimed;

    public StochasticsResult AddAndCalc(ICandle candle)
    {
        if (candle == null)
            throw new ArgumentNullException(nameof(candle));

        candles.Add(candle);

        fastK.Add(GetFastK());

        return GetStochasticsResult(candle);
    }

    public StochasticsResult UpdateAndCalc(ICandle candle)
    {
        if (candle == null)
            throw new ArgumentNullException(nameof(candle));

        candles.Update(candle);

        fastK.Update(GetFastK());

        return GetStochasticsResult(candle);
    }

    private StochasticsResult GetStochasticsResult(ICandle candle)
    {
        var smaFastK = new SmaIndicator(smooth, pair, RateToUse.Close);
        var smaK = new SmaIndicator(periodD, pair, RateToUse.Close);

        double k = 0;
        double d = 0;

        for (var i = fastK.Count - 1; i >= 0; i--)
        {
            k = smaFastK.AddAndCalc(fastK[i].OpenOn, fastK[i].Value).Value;
            d = smaK.AddAndCalc(fastK[i].OpenOn, k).Value;
        }

        return new StochasticsResult
        {
            OpenOn = candle.OpenOn,
            K = k,
            D = d
        };
    }

    private (TickOn, double) GetFastK()
    {
        var openOn = candles[0].OpenOn;
        var close = candles[0].Close;
        var low = candles.Select(c => c.Low).Min();
        var high = candles.Select(c => c.High).Max();

        var den = high.Value - low.Value;

        if (Math.Abs(den) < 0.00001f)
        {
            if (candles.Count == 1)
                return (openOn, 50.0);
            else
                return fastK[0];
        }
        else
        {
            var nom = close.Value - low.Value;

            return (openOn, Math.Min(100.0, Math.Max(0.0, 100.0 * nom / den)));
        }
    }
}