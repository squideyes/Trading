using SquidEyes.Basics;
using SquidEyes.Trading.FxData;
using System.Collections.Concurrent;
using static SquidEyes.Trading.Context.Currency;

namespace SquidEyes.Trading.Context;

public class UsdValueOf
{
    private readonly ConcurrentDictionary<Pair, Rate> rates = new();

    public void Update(MetaTick metaTick)
    {
        if (metaTick.Pair.Base != USD && metaTick.Pair.Quote != USD)
            return;

        metaTick.Tick.AsFunc(
            m => rates.AddOrUpdate(metaTick.Pair, m.Ask, (p, v) => m.Ask));
    }

    public Rate GetOneUnitInUsd(Currency currency)
    {
        Rate GetRate(Symbol symbol) =>
            rates.GetOrAdd(Known.Pairs[symbol], p => default);

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