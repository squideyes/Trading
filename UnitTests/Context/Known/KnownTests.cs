// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Trading.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class KnownTests
{
    [Fact]
    public void KnownBaselineUnchanged()
    {
        Known.MinYear.Should().Be(2018);

        Known.MaxYear.Should().Be(2028);

        foreach (var symbol in Enum.GetValues<Symbol>())
            Known.Pairs.ContainsKey(symbol).Should().BeTrue();

        foreach (var currency in Enum.GetValues<Currency>())
            Known.Currencies.Contains(currency).Should().BeTrue();

        Known.TradeDates.Count.Should().Be(2850);

        foreach (var pair in Known.Pairs.Values)
            Known.ConvertWith.ContainsKey(pair);

        Known.MinTradeDate.Should().Be(Known.TradeDates.First());

        Known.MinTradeDate.DayOfWeek.Should().Be(DayOfWeek.Monday);

        Known.MaxTradeDate.Should().Be(Known.TradeDates.Last());

        Known.MaxTradeDate.DayOfWeek.Should().Be(DayOfWeek.Friday);

        Known.MinTickOnValue.Should().Be(
            new DateTime(2018, 1, 7, 17, 0, 0, DateTimeKind.Unspecified));

        Known.MaxTickOnValue.Should().Be(
            new DateTime(2028, 12, 29, 16, 59, 59, 999, DateTimeKind.Unspecified));
    }

    [Fact]
    public void GetTradeDatesByYearMonthReturnsExpectedDates()
    {
        var byMonth = new Dictionary<(int, int), List<DateOnly>>();

        foreach (var tradeDate in Known.TradeDates)
        {
            var key = (tradeDate.Year, tradeDate.Month);

            if (!byMonth.ContainsKey(key))
                byMonth.Add(key, new List<DateOnly>());

            byMonth[key].Add(tradeDate);
        }

        for (var year = Known.MinYear; year <= Known.MaxYear; year++)
        {
            for (var month = 1; month < 12; month++)
            {
                var tradeDates = Known.GetTradeDates(year, month);

                var lookup = byMonth[(year, month)];

                tradeDates.Count.Should().Be(lookup.Count);

                for (var i = 0; i < lookup.Count; i++)
                    tradeDates[i].Should().Be(lookup[i]);
            }
        }
    }
}