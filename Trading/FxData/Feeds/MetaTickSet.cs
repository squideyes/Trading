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

namespace SquidEyes.Trading.FxData
{
    public class MetaTickSet : IEnumerable<MetaTick>
    {
        public readonly Dictionary<Pair, List<TickSet>> tickSets = new();

        private DateOnly minTradeDate;
        private DateOnly maxTradeDate;

        public MetaTickSet(Source source, Session session)
        {
            Source = source.Validated(nameof(source), v => v.IsEnumValue());

            Session = session;

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
        public List<Pair> Pairs => tickSets.Keys.ToList();

        public void Add(List<TickSet> tickSets)
        {
            var days = Session.Extent.ToDays();

            if (!tickSets.Count.Between(1, days))
                throw new ArgumentOutOfRangeException(nameof(tickSets));

            if (tickSets.Any(ts => ts.Source != Source))
                throw new ArgumentOutOfRangeException(nameof(tickSets));

            if (tickSets.Any(ts => ts.Pair != tickSets.First().Pair))
                throw new ArgumentOutOfRangeException(nameof(tickSets));

            foreach (var tickSet in tickSets)
            {
                if (!tickSet.Session.TradeDate.Between(minTradeDate, maxTradeDate))
                    throw new ArgumentOutOfRangeException(nameof(tickSets));
            }

            for (var i = 1; i < tickSets.Count; i++)
            {
                if (tickSets[i].Session.TradeDate <= tickSets[i - 1].Session.TradeDate)
                    throw new ArgumentOutOfRangeException(nameof(tickSets));
            }

            if (this.tickSets.Count > 0)
            {
                if (tickSets.Count != this.tickSets.First().Value.Count)
                    throw new ArgumentOutOfRangeException(nameof(tickSets));

                for (var i = 0; i < tickSets.Count; i++)
                {
                    if (tickSets[i].Session.TradeDate != 
                        this.tickSets.First().Value[i].Session.TradeDate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(tickSets));
                    }
                }
            }

            this.tickSets.Add(tickSets[0].Pair, tickSets);
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

                var tuples = new List<(Pair Pair, Tick Tick)>();

                foreach (var tickOn in datas.Keys.OrderBy(t => t))
                    tuples.AddRange(datas[tickOn].OrderBy(d => random.Next()));

                for (var i = 0; i < tuples.Count; i++)
                {
                    var tuple = tuples[i];

                    yield return new MetaTick(tuple.Pair, tuple.Tick);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}