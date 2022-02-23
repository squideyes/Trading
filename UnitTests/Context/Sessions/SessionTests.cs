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
using Xunit;
using static SquidEyes.Trading.Context.Extent;
using static SquidEyes.Trading.Context.Session.Defaults;

namespace SquidEyes.UnitTests.Context;

public class SessionTests
{
    [Theory]
    [InlineData(Day, "01/06/2020", "01/05/2020 17:00:00.000",
        "01/06/2020 16:59:59.999", null, null)]
    [InlineData(Day, "01/07/2020", "01/06/2020 17:00:00.000",
        "01/07/2020 16:59:59.999", null, null)]
    [InlineData(Day, "01/08/2020", "01/07/2020 17:00:00.000",
        "01/08/2020 16:59:59.999", null, null)]
    [InlineData(Day, "01/09/2020", "01/08/2020 17:00:00.000",
        "01/09/2020 16:59:59.999", null, null)]
    [InlineData(Day, "01/10/2020", "01/09/2020 17:00:00.000",
        "01/10/2020 16:59:59.999", null, null)]
    [InlineData(Week, "01/06/2020", "01/05/2020 17:00:00.000",
        "01/10/2020 16:59:59.999", null, null)]
    [InlineData(Day, "01/06/2020", "01/05/2020 17:00:00.000",
        "01/06/2020 16:59:59.999", 20, 20)]
    [InlineData(Day, "01/07/2020", "01/06/2020 17:00:00.000",
        "01/07/2020 16:59:59.999", 20, 20)]
    [InlineData(Day, "01/08/2020", "01/07/2020 17:00:00.000",
        "01/08/2020 16:59:59.999", 20, 20)]
    [InlineData(Day, "01/09/2020", "01/08/2020 17:00:00.000",
        "01/09/2020 16:59:59.999", 20, 20)]
    [InlineData(Day, "01/10/2020", "01/09/2020 17:00:00.000",
        "01/10/2020 16:59:59.999", 20, 20)]
    [InlineData(Week, "01/06/2020", "01/05/2020 17:00:00.000",
        "01/10/2020 16:59:59.999", 20, 20)]
    public void GoodContructorArgs(Extent extent,
        string tradeDateString, string minTickOnString,
        string maxTickOnString, int? fromOpen, int? fromClose)
    {
        var endCaps = fromOpen.HasValue ?
            (fromOpen.Value, fromClose!.Value) : default;

        var tradeDate = DateOnly.Parse(tradeDateString);

        var session = new Session(extent, tradeDate, endCaps);

        session.Extent.Should().Be(extent);
        session.TradeDate.Should().Be(tradeDate);
        session.MinTickOn.Should().Be(minTickOnString);
        session.MaxTickOn.Should().Be(maxTickOnString);
        session.EndCaps.Should().Be(
            endCaps == default ? (FromOpen, FromClose) : endCaps);
    }

    //////////////////////////

    [Theory]
    [InlineData((Extent)0, null, FromOpen, FromClose)]
    [InlineData(Day, "01/05/2020", FromOpen, FromClose)]
    [InlineData(Day, "01/06/2020", -1, FromClose)]
    [InlineData(Day, "01/06/2020", FromOpen, 0)]
    [InlineData(Week, "01/05/2020", FromOpen, FromClose)]
    [InlineData(Week, "01/06/2020", -1, FromClose)]
    [InlineData(Week, "01/06/2020", FromOpen, 0)]
    public void BadContructorArgs(
        Extent extent, string baseDateString, int? fromOpen, int? fromClose)
    {
        FluentActions.Invoking(() =>
        {
            var endCaps = fromOpen.HasValue ?
                (fromOpen!.Value, fromClose!.Value) : default;

            var baseDate = baseDateString == null ?
                DateOnly.MinValue : DateOnly.Parse(baseDateString);

            _ = new Session(extent, baseDate, endCaps);
        })
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(Day)]
    [InlineData(Week)]
    public void ToStringReturnsExpectedValue(Extent extent)
    {
        var tradeDate = new DateOnly(2020, 1, 6);

        var endCaps = (FromOpen, FromClose);

        new Session(extent, tradeDate).ToString().Should().Be(
            $"{tradeDate:MM/dd/yyyy} ({extent}, EndCaps: {endCaps})");
    }

    //////////////////////////

    [Theory]
    [InlineData(Day, "01/06/2020 16:59:59.999", false)]
    [InlineData(Day, "01/06/2020 17:00:00.000", true)]
    [InlineData(Day, "01/07/2020 16:59:59.999", true)]
    [InlineData(Day, "01/07/2020 17:00:00.000", false)]
    public void InSessionWithMixedArgs(
        Extent extent, string tickOnString, bool result)
    {
        new Session(extent, new DateOnly(2020, 1, 7))
            .InSession((TickOn)tickOnString).Should().Be(result);
    }

    //////////////////////////

    [Theory]
    [InlineData(Day, FromClose, "01/06/2020 16:59:59.999", true)]
    [InlineData(Day, null, "01/06/2020 16:59:59.999", true)]
    [InlineData(Day, FromClose, "01/06/2020 16:30:00.000", true)]
    [InlineData(Day, null, "01/06/2020 16:30:00.000", true)]
    [InlineData(Day, FromClose, "01/06/2020 16:29:59.999", false)]
    [InlineData(Day, null, "01/06/2020 16:29:59.999", false)]
    [InlineData(Day, FromClose, "01/05/2020 17:00:00.000", false)]
    [InlineData(Day, null, "01/05/2020 17:00:00.000", false)]
    [InlineData(Week, FromClose, "01/10/2020 16:59:59.999", true)]
    [InlineData(Week, null, "01/10/2020 16:59:59.999", true)]
    [InlineData(Week, FromClose, "01/10/2020 16:30:00.000", true)]
    [InlineData(Week, null, "01/10/2020 16:30:00.000", true)]
    [InlineData(Week, FromClose, "01/10/2020 16:29:59.999", false)]
    [InlineData(Week, null, "01/10/2020 16:29:59.999", false)]
    [InlineData(Week, FromClose, "01/09/2020 17:00:00.000", false)]
    [InlineData(Week, null, "01/09/2020 17:00:00.000", false)]
    public void MustBeFlatWithMixedArgs(
        Extent extent, int? fromClose, string tickOnString, bool expected)
    {
        var endCaps = fromClose.HasValue ? (0, fromClose!.Value) : default;

        var session = new Session(
            extent, new DateOnly(2020, 1, 6), endCaps);

        session.MustBeFlat((TickOn)tickOnString).Should().Be(expected);
    }

    //////////////////////////

    [Theory]
    [InlineData(Day, FromOpen, FromClose, "01/05/2020 17:00:00.000", false)]
    [InlineData(Day, null, null, "01/05/2020 17:00:00.000", false)]
    [InlineData(Day, FromOpen, FromClose, "01/05/2020 17:14:59.999", false)]
    [InlineData(Day, null, null, "01/05/2020 17:14:59.999", false)]
    [InlineData(Day, FromOpen, FromClose, "01/05/2020 17:15:00.000", true)]
    [InlineData(Day, null, null, "01/05/2020 17:15:00.000", true)]
    [InlineData(Day, FromOpen, FromClose, "01/06/2020 16:29:59.999", true)]
    [InlineData(Day, null, null, "01/06/2020 16:29:59.999", true)]
    [InlineData(Day, FromOpen, FromClose, "01/06/2020 16:30:00.000", false)]
    [InlineData(Day, null, null, "01/06/2020 16:30:00.000", false)]
    [InlineData(Day, FromOpen, FromClose, "01/06/2020 16:59:59.999", false)]
    [InlineData(Day, null, null, "01/06/2020 16:59:59.999", false)]
    [InlineData(Week, FromOpen, FromClose, "01/05/2020 17:00:00.000", false)]
    [InlineData(Week, null, null, "01/05/2020 17:00:00.000", false)]
    [InlineData(Week, FromOpen, FromClose, "01/05/2020 17:14:59.999", false)]
    [InlineData(Week, null, null, "01/05/2020 17:14:59.999", false)]
    [InlineData(Week, FromOpen, FromClose, "01/05/2020 17:15:00.000", true)]
    [InlineData(Week, null, null, "01/05/2020 17:15:00.000", true)]
    [InlineData(Week, FromOpen, FromClose, "01/10/2020 16:29:59.999", true)]
    [InlineData(Week, null, null, "01/10/2020 16:29:59.999", true)]
    [InlineData(Week, FromOpen, FromClose, "01/10/2020 16:30:00.000", false)]
    [InlineData(Week, null, null, "01/10/2020 16:30:00.000", false)]
    [InlineData(Week, FromOpen, FromClose, "01/10/2020 16:59:59.999", false)]
    [InlineData(Week, null, null, "01/10/2020 16:59:59.999", false)]  
    public void CanTradeWithMixedArgs(Extent extent,
        int? fromOpen, int? fromClose, string tickOnString, bool expected)
    {
        var endCaps = fromOpen.HasValue ?
            (fromOpen!.Value, fromClose!.Value) : default;

        var session = new Session(
            extent, new DateOnly(2020, 1, 6), endCaps);

        session.CanTrade(tickOnString).Should().Be(expected);
    }

    //////////////////////////

    [Fact]
    public void GoodUntilReturnsExpectedValue()
    {
        var session = new Session(Day, new DateOnly(2020, 1, 6), (0, 10));

        session.AutoFlatOn.Should().Be(new DateTime(2020, 1, 6, 16, 50, 0));
    }

    //////////////////////////

    [Theory]
    [InlineData(Day)]
    [InlineData(Week)]
    public void SessionNotEqualToNullSession(Extent extent) => new Session(
        extent, new DateOnly(2020, 1, 6)).Equals(null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(Day)]
    [InlineData(Week)]
    public void GetHashCodeReturnsExpectedResult(Extent extent)
    {
        new Session(extent, TradeDate).GetHashCode().Should()
            .Be(new Session(extent, TradeDate).GetHashCode());

        new Session(extent, TradeDate).GetHashCode().Should().NotBe(
            new Session(extent, TradeDate.AddDays(7)).GetHashCode());
    }

    //////////////////////////

    [Theory]
    [InlineData(Day, true)]
    [InlineData(Day, false)]
    [InlineData(Week, true)]
    [InlineData(Week, false)]
    public void GenericEquals(Extent extent, bool result)
    {
        new Session(extent, TradeDate).Equals(result ? new Session(extent, TradeDate)
            : new Session(extent, TradeDate.AddDays(7))).Should().Be(result);
    }

    //////////////////////////

    [Theory]
    [InlineData(Day, true)]
    [InlineData(Day, false)]
    [InlineData(Week, true)]
    [InlineData(Week, false)]
    public void ObjectEqualsWithGoodSession(Extent extent, bool result)
    {
        new Session(extent, TradeDate).Equals(result ? new Session(extent, TradeDate)
            : new Session(extent, TradeDate.AddDays(7))).Should().Be(result);
    }

    //////////////////////////

    [Fact]
    public void ObjectEqualsWithNullSession() =>
        new Session(Day, TradeDate).Equals((object?)null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(Day, true)]
    [InlineData(Day, false)]
    [InlineData(Week, true)]
    [InlineData(Week, false)]
    public void EqualsOperator(Extent extent, bool result) => (new Session(
        extent, TradeDate) == (result ? new Session(extent, TradeDate)
        : new Session(extent, TradeDate.AddDays(7)))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(Day, true)]
    [InlineData(Day, false)]
    [InlineData(Week, true)]
    [InlineData(Week, false)]
    public void NotEqualsOperator(Extent extent, bool result) => (new Session(
        extent, TradeDate) != (result ? new Session(extent,
        TradeDate.AddDays(7)) : new Session(extent, TradeDate))).Should().Be(result);

    //////////////////////////

    private static DateOnly TradeDate => new(2020, 1, 6);
}