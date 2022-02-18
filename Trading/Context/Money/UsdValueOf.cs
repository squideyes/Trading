// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.Trading.FxData;
using System.Collections.Concurrent;
using static SquidEyes.Trading.Context.Currency;

namespace SquidEyes.Trading.Context;

public class UsdValueOf
{
    private readonly ConcurrentDictionary<Pair, Rate> rates = new();

    private readonly MidOrAsk midOrAsk;

    public UsdValueOf(MidOrAsk midOrAsk)
    {
        this.midOrAsk = midOrAsk.Validated(nameof(midOrAsk), v => v.IsEnumValue());
    }

    public void Update(MetaTick metaTick)
    {
        if (Equals(metaTick, null!))
            throw new ArgumentNullException(nameof(metaTick));

        if (metaTick.Pair.Base != USD && metaTick.Pair.Quote != USD)
            return;

        var rate = midOrAsk switch
        {
            MidOrAsk.Mid => metaTick.Tick.Mid,
            MidOrAsk.Ask => metaTick.Tick.Ask,
            _ => throw new ArgumentOutOfRangeException(nameof(metaTick))
        };

        rates.AddOrUpdate(metaTick.Pair, rate, (p, v) => rate);
    }

    public Rate GetRateInUsd(Currency currency)
    {
        Rate GetRate(Symbol symbol) => rates.GetOrAdd(Known.Pairs[symbol], p => default);

        Rate GetReciprocal(Symbol symbol) => Known.Pairs[symbol].AsFunc(
            p => new Rate(1.0f / GetRate(symbol).GetFloat(p.Digits), p.Digits));

        return currency switch
        {
            AUD => GetRate(Symbol.AUDUSD),
            CAD => GetReciprocal(Symbol.USDCAD),
            CHF => GetReciprocal(Symbol.USDCHF),
            JPY => GetReciprocal(Symbol.USDJPY),
            EUR => GetRate(Symbol.EURUSD),
            GBP => GetRate(Symbol.GBPUSD),
            NZD => GetRate(Symbol.NZDUSD),
            USD => new Rate(100000),
            _ => throw new ArgumentOutOfRangeException(nameof(currency))
        };
    }
}