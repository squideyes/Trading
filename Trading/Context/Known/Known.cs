// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using System.Collections.Immutable;
using static SquidEyes.Trading.Context.Symbol;
using static System.DayOfWeek;

namespace SquidEyes.Trading.Context;

public static class Known
{
    public const int MinYear = 2018;
    public const int MaxYear = 2028;

    static Known()
    {
        Pairs = GetPairs();
        ConvertWith = GetConvertWith();
        TradeDates = GetTradeDates();
        MinTradeDate = TradeDates.First();
        MinTickOnValue = MinTradeDate.ToMinTickOnValue(false);
        MaxTradeDate = TradeDates.Last();
        MaxTickOnValue = MaxTradeDate.ToMaxTickOnValue(false);
        Currencies = ImmutableSortedSet.CreateRange(EnumList.FromAll<Currency>());
    }

    public static IReadOnlyDictionary<Symbol, Pair> Pairs { get; }
    public static IReadOnlyDictionary<Pair, (Pair Base, Pair Quote)> ConvertWith { get; }
    public static ImmutableSortedSet<DateOnly> TradeDates { get; }
    public static DateOnly MinTradeDate { get; }
    public static DateTime MinTickOnValue { get; }
    public static DateOnly MaxTradeDate { get; }
    public static DateTime MaxTickOnValue { get; }
    public static ImmutableSortedSet<Currency> Currencies { get; }

    public static ImmutableSortedSet<DateOnly> GetTradeDates(int year, int month)
    {
        if (!year.Between(MinYear, MaxYear))
            throw new ArgumentOutOfRangeException(nameof(year));

        if (!month.Between(1, 12))
            throw new ArgumentOutOfRangeException(nameof(month));

        var minDate = new DateOnly(year, month, 1);

        while (!minDate.IsTradeDate())
            minDate = minDate.AddDays(1);

        var maxDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        while (!maxDate.IsTradeDate())
            maxDate = maxDate.AddDays(-1);

        return GetTradeDatesInRange(minDate, maxDate).ToImmutableSortedSet();
    }

    private static bool IsHoliday(DateOnly value)
    {
        return (value.Month, value.Day, value.DayOfWeek) switch
        {
            (1, 1, Monday) => true,
            (1, 1, Tuesday) => true,
            (1, 1, Wednesday) => true,
            (1, 1, Thursday) => true,
            (1, 1, Friday) => true,
            (12, 25, Monday) => true,
            (12, 25, Tuesday) => true,
            (12, 25, Wednesday) => true,
            (12, 25, Thursday) => true,
            (12, 25, Friday) => true,
            _ => false,
        };
    }

    private static List<DateOnly> GetTradeDatesInRange(DateOnly minDate, DateOnly maxDate)
    {
        var tradeDates = new List<DateOnly>();

        for (var date = minDate; date <= maxDate; date = date.AddDays(1))
        {
            if (date.IsWeekday() && !IsHoliday(date))
                tradeDates.Add(date);
        }

        return tradeDates;
    }

    private static ImmutableSortedSet<DateOnly> GetTradeDates()
    {
        var minDate = new DateOnly(MinYear, 1, 1);

        while (minDate.DayOfWeek != Monday || IsHoliday(minDate))
            minDate = minDate.AddDays(1);

        var maxDate = new DateOnly(MaxYear, 12, 31);

        while (maxDate.DayOfWeek != Friday || IsHoliday(minDate))
            maxDate = maxDate.AddDays(-1);

        return GetTradeDatesInRange(minDate, maxDate).ToImmutableSortedSet();
    }

    private static IReadOnlyDictionary<Symbol, Pair> GetPairs()
    {
        var pairs = new Dictionary<Symbol, Pair>();

        void Add(Symbol symbol, int digits) =>
            new Pair(symbol, digits).AsAction(p => pairs.Add(p.Symbol, p));

        Add(EURUSD, 5);
        Add(USDJPY, 3);
        Add(GBPUSD, 5);
        Add(AUDUSD, 5);
        Add(USDCAD, 5);
        Add(USDCHF, 5);
        Add(NZDUSD, 5);
        Add(EURJPY, 3);
        Add(EURGBP, 5);

        return pairs;
    }

    private static IReadOnlyDictionary<Pair, (Pair, Pair)> GetConvertWith()
    {
        Dictionary<Pair, (Pair, Pair)> convertWith = new();

        void AddLookups(Symbol symbol, Pair toBase, Pair toQuote) =>
            convertWith.Add(Pairs[symbol], (toBase, toQuote));

        AddLookups(AUDUSD, Pairs[AUDUSD], Pairs[AUDUSD]);
        AddLookups(EURUSD, Pairs[EURUSD], Pairs[EURUSD]);
        AddLookups(GBPUSD, Pairs[GBPUSD], Pairs[GBPUSD]);
        AddLookups(NZDUSD, Pairs[NZDUSD], Pairs[NZDUSD]);
        AddLookups(USDCAD, Pairs[USDCAD], Pairs[USDCAD]);
        AddLookups(USDCHF, Pairs[USDCHF], Pairs[USDCHF]);
        AddLookups(USDJPY, Pairs[USDJPY], Pairs[USDJPY]);
        AddLookups(EURGBP, Pairs[EURUSD], Pairs[GBPUSD]);
        AddLookups(EURJPY, Pairs[EURUSD], Pairs[USDJPY]);

        return convertWith;
    }
}
