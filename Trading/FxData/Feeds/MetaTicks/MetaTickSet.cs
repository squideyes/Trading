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
using System.Collections;

namespace SquidEyes.Trading.FxData;

public class MetaTickSet : IEnumerable<MetaTick>
{
    public readonly Dictionary<Pair, List<TickSet>> tickSets = new();

    private DateOnly minTradeDate;
    private DateOnly maxTradeDate;

    public MetaTickSet(Source source, Session session, HashSet<Pair> pairs)
    {
        Source = source.Validated(nameof(source), v => v.IsEnumValue());

        Session = session ?? throw new ArgumentNullException(nameof(session));

        if (!pairs.HasItems())
            throw new ArgumentOutOfRangeException(nameof(pairs));

        Pairs = new Dictionary<Pair, bool>();

        void AddOrUpdate(Pair pair, bool canTrade)
        {
            if (Pairs.ContainsKey(pair))
            {
                if (canTrade)
                    Pairs[pair] = canTrade;
            }
            else
            {
                Pairs.Add(pair, canTrade);
            }
        }

        foreach (var pair in pairs)
        {
            var (basePair, quotePair) = Known.ConvertWith[pair];

            AddOrUpdate(pair, true);
            AddOrUpdate(basePair, false);
            AddOrUpdate(quotePair, false);
        }

        foreach (var pair in Pairs.Keys)
            tickSets.Add(pair, new List<TickSet>());

        minTradeDate = session.TradeDate;

        maxTradeDate = session.Extent switch
        {
            Extent.Day => session.TradeDate,
            Extent.Week => session.TradeDate.AddDays(4),
            _ => throw new ArgumentOutOfRangeException(nameof(session))
        };
    }

    public Source Source { get; }
    public Session Session { get; }
    public Dictionary<Pair, bool> Pairs { get; }

    public void Add(List<TickSet> tickSets)
    {
        if (!tickSets.HasItems())
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        if (tickSets.Any(ts => ts.Source != Source))
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        if (tickSets.Any(ts => ts.Session != tickSets[0].Session))
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        var tradeDate = tickSets[0].Session.TradeDate;

        if (!tradeDate.Between(minTradeDate, maxTradeDate))
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        if (this.tickSets.First().Value.Count >= 1
            && tradeDate <= this.tickSets.First().Value.Last().Session.TradeDate)
        {
            throw new ArgumentOutOfRangeException(nameof(tickSets));
        }

        if (tickSets.Count != Pairs.Count)
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        if (!tickSets.Select(ts => ts.Pair).IsUnique())
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        if (tickSets.Any(ts => !Pairs.ContainsKey(ts.Pair)))
            throw new ArgumentOutOfRangeException(nameof(tickSets));

        foreach (var tickSet in tickSets)
            this.tickSets[tickSet.Pair].Add(tickSet);
    }

    public IEnumerator<MetaTick> GetEnumerator()
    {
        if (tickSets.Count == 1)
        {
            var pair = tickSets.Keys.First();

            foreach (var tickSet in tickSets.Values.First())
            {
                foreach (var tick in tickSet)
                    yield return new MetaTick(pair, tick);
            }
        }
        else
        {
            var random = new Random(5555);

            var datas = new Dictionary<TickOn, List<(Pair Pair, Tick Tick)>>();

            foreach (var pair in tickSets.Keys)
            {
                foreach (var tickSet in tickSets[pair])
                {
                    foreach (var tick in tickSet)
                    {
                        if (!datas.ContainsKey(tick.TickOn))
                            datas.Add(tick.TickOn, new List<(Pair, Tick)>());

                        datas[tick.TickOn].Add((pair, tick));
                    }
                }
            }

            foreach (var tickOn in datas.Keys.OrderBy(t => t))
            {
                foreach (var data in datas[tickOn].OrderBy(d => random.Next()))
                    yield return new MetaTick(data.Pair, data.Tick);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}