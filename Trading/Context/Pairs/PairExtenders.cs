// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.Trading.Context;

public static class PairExtenders
{
    private static List<Pair> GetExtraPairs(this Pair pair)
    {
        if (!Known.Pairs.ContainsKey(pair.Symbol))
            throw new ArgumentOutOfRangeException(nameof(pair));

        var pairs = new HashSet<Pair>();

        var (@base, quote) = Known.ConvertWith[pair];

        pairs.Add(pair);
        pairs.Add(@base);
        pairs.Add(quote);

        return pairs.ToList();
    }

    public static Dictionary<Pair, bool> WithExtraPairs(
        this IEnumerable<Pair> pairs)
    {
        if (!pairs.HasItems())
            throw new ArgumentOutOfRangeException(nameof(pairs));

        var dict = pairs.ToDictionary(p => p, p => false);

        foreach (var pair in pairs)
        {
            foreach (var extraPair in GetExtraPairs(pair))
            {
                if (!dict.ContainsKey(extraPair))
                    dict.Add(extraPair, true);
            }
        }

        return dict;
    }
}