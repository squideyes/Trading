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
    [InlineData(Currency.AUD, MidBidOrAsk.Bid, 71510)]
    [InlineData(Currency.AUD, MidBidOrAsk.Mid, 71520)]
    [InlineData(Currency.AUD, MidBidOrAsk.Ask, 71530)]
    [InlineData(Currency.CAD, MidBidOrAsk.Bid, 78629)]
    [InlineData(Currency.CAD, MidBidOrAsk.Mid, 78616)]
    [InlineData(Currency.CAD, MidBidOrAsk.Ask, 78604)]
    [InlineData(Currency.CHF, MidBidOrAsk.Bid, 108050)]
    [InlineData(Currency.CHF, MidBidOrAsk.Mid, 108032)]
    [InlineData(Currency.CHF, MidBidOrAsk.Ask, 108015)]
    [InlineData(Currency.EUR, MidBidOrAsk.Bid, 113460)]
    [InlineData(Currency.EUR, MidBidOrAsk.Mid, 113470)]
    [InlineData(Currency.EUR, MidBidOrAsk.Ask, 113480)]
    [InlineData(Currency.GBP, MidBidOrAsk.Bid, 135370)]
    [InlineData(Currency.GBP, MidBidOrAsk.Mid, 135375)]
    [InlineData(Currency.GBP, MidBidOrAsk.Ask, 135380)]
    [InlineData(Currency.JPY, MidBidOrAsk.Bid, 9)]
    [InlineData(Currency.JPY, MidBidOrAsk.Mid, 9)]
    [InlineData(Currency.JPY, MidBidOrAsk.Ask, 9)]
    [InlineData(Currency.NZD, MidBidOrAsk.Bid, 66340)]
    [InlineData(Currency.NZD, MidBidOrAsk.Mid, 66360)]
    [InlineData(Currency.NZD, MidBidOrAsk.Ask, 66380)]
    [InlineData(Currency.USD, MidBidOrAsk.Bid, 100000)]
    [InlineData(Currency.USD, MidBidOrAsk.Mid, 100000)]
    [InlineData(Currency.USD, MidBidOrAsk.Ask, 100000)]
    public void GetRateInUsdReturnedExpectedValue(
        Currency currency, MidBidOrAsk midBidOrAsk, int rate)
    {
        var usdValueOf = MoneyData.GetUsdValueOf(midBidOrAsk);

        usdValueOf.GetRateInUsd(currency).Should().Be(rate);
    }
}