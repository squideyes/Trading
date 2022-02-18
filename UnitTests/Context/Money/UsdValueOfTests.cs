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
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class UsdValueOfTests
{
    [Theory]
    [InlineData(Currency.AUD, MidOrAsk.Mid, 71520)]
    [InlineData(Currency.AUD, MidOrAsk.Ask, 71530)]
    [InlineData(Currency.CAD, MidOrAsk.Mid, 78616)]
    [InlineData(Currency.CAD, MidOrAsk.Ask, 78604)]
    [InlineData(Currency.CHF, MidOrAsk.Mid, 108032)]
    [InlineData(Currency.CHF, MidOrAsk.Ask, 108015)]
    [InlineData(Currency.EUR, MidOrAsk.Mid, 113470)]
    [InlineData(Currency.EUR, MidOrAsk.Ask, 113480)]
    [InlineData(Currency.GBP, MidOrAsk.Mid, 135375)]
    [InlineData(Currency.GBP, MidOrAsk.Ask, 135380)]
    [InlineData(Currency.JPY, MidOrAsk.Mid, 9)]
    [InlineData(Currency.JPY, MidOrAsk.Ask, 9)]
    [InlineData(Currency.NZD, MidOrAsk.Mid, 66360)]
    [InlineData(Currency.NZD, MidOrAsk.Ask, 66380)]
    [InlineData(Currency.USD, MidOrAsk.Mid, 100000)]
    [InlineData(Currency.USD, MidOrAsk.Ask, 100000)]
    public void GetRateInUsdReturnedExpectedValue(
        Currency currency, MidOrAsk midOrAsk, int rate)
    {
        var usdValueOf = MoneyData.GetUsdValueOf(midOrAsk);

        usdValueOf.GetRateInUsd(currency).Should().Be(rate);
    }
}