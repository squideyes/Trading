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
    private class GoodContructorArgsData
        : Testing.TheoryData<Extent, DateOnly, DateTime, DateTime, (int, int)>
    {
        public GoodContructorArgsData()
        {
            Add(Day, DO("01/06/2020"), DT("01/05/2020 17:00:00.000"),
                DT("01/06/2020 16:59:59.999"), default);
            Add(Day, DO("01/07/2020"), DT("01/06/2020 17:00:00.000"),
                DT("01/07/2020 16:59:59.999"), default);
            Add(Day, DO("01/08/2020"), DT("01/07/2020 17:00:00.000"),
                DT("01/08/2020 16:59:59.999"), default);
            Add(Day, DO("01/09/2020"), DT("01/08/2020 17:00:00.000"),
                DT("01/09/2020 16:59:59.999"), default);
            Add(Day, DO("01/10/2020"), DT("01/09/2020 17:00:00.000"),
                DT("01/10/2020 16:59:59.999"), default);
            Add(Week, DO("01/06/2020"), DT("01/05/2020 17:00:00.000"),
                DT("01/10/2020 16:59:59.999"), default);

            Add(Day, DO("01/06/2020"), DT("01/05/2020 17:00:00.000"),
                DT("01/06/2020 16:59:59.999"), (20, 20));
            Add(Day, DO("01/07/2020"), DT("01/06/2020 17:00:00.000"),
                DT("01/07/2020 16:59:59.999"), (20, 20));
            Add(Day, DO("01/08/2020"), DT("01/07/2020 17:00:00.000"),
                DT("01/08/2020 16:59:59.999"), (20, 20));
            Add(Day, DO("01/09/2020"), DT("01/08/2020 17:00:00.000"),
                DT("01/09/2020 16:59:59.999"), (20, 20));
            Add(Day, DO("01/10/2020"), DT("01/09/2020 17:00:00.000"),
                DT("01/10/2020 16:59:59.999"), (20, 20));
            Add(Week, DO("01/06/2020"), DT("01/05/2020 17:00:00.000"),
                DT("01/10/2020 16:59:59.999"), (20, 20));
        }
    }

    [Theory]
    [ClassData(typeof(GoodContructorArgsData))]
    public void GoodContructorArgs(Extent extent, DateOnly tradeDate,
        DateTime minTickOn, DateTime maxTickOn, (int FromOpen, int FromClose) endCaps)
    {
        var session = new Session(extent, tradeDate, endCaps);

        session.Extent.Should().Be(extent);
        session.TradeDate.Should().Be(tradeDate);
        session.MinTickOn.Should().Be(minTickOn);
        session.MaxTickOn.Should().Be(maxTickOn);
        session.EndCaps.Should().Be(
            endCaps == default ? (FromOpen, FromClose) : endCaps);
    }

    //////////////////////////

    private class BadContructorArgsData :
        Testing.TheoryData<Extent, DateOnly, int?, int?>
    {
        public BadContructorArgsData()
        {
            Add(0, DateOnly.MinValue, FromOpen, FromClose);
            Add(Day, DO("01/05/2020"), FromOpen, FromClose);
            Add(Day, DO("01/06/2020"), -1, FromClose);
            Add(Day, DO("01/06/2020"), FromOpen, 0);
            Add(Week, DO("01/05/2020"), FromOpen, FromClose);
            Add(Week, DO("01/06/2020"), -1, FromClose);
            Add(Week, DO("01/06/2020"), FromOpen, 0);
        }
    }

    [Theory]
    [ClassData(typeof(BadContructorArgsData))]
    public void BadContructorArgs(
        Extent extent, DateOnly baseDate, int? fromOpen, int? fromClose)
    {
        FluentActions.Invoking(() =>
        {
            var endCaps = fromOpen.HasValue ?
                (fromOpen!.Value, fromClose!.Value) : default;

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

    private class InSessionWithMixedArgsData : Testing.TheoryData<Extent, DateTime, bool>
    {
        public InSessionWithMixedArgsData()
        {
            Add(Day, DT("01/06/2020 16:59:59.999"), false);
            Add(Day, DT("01/06/2020 17:00:00.000"), true);
            Add(Day, DT("01/07/2020 16:59:59.999"), true);
            Add(Day, DT("01/07/2020 17:00:00.000"), false);
        }
    }

    [Theory]
    [ClassData(typeof(InSessionWithMixedArgsData))]
    public void InSessionWithMixedArgs(
        Extent extent, DateTime tickOn, bool result)
    {
        new Session(extent, new DateOnly(2020, 1, 7))
            .InSession(tickOn).Should().Be(result);
    }

    //////////////////////////

    private class MustBeFlatWithMixedArgsData :
        Testing.TheoryData<Extent, int?, TickOn, bool>
    {
        public MustBeFlatWithMixedArgsData()
        {
            Add(Day, FromClose, DT("01/06/2020 16:59:59.999"), true);
            Add(Day, null, DT("01/06/2020 16:59:59.999"), true);
            Add(Day, FromClose, DT("01/06/2020 16:30:00.000"), true);
            Add(Day, null, DT("01/06/2020 16:30:00.000"), true);
            Add(Day, FromClose, DT("01/06/2020 16:29:59.999"), false);
            Add(Day, null, DT("01/06/2020 16:29:59.999"), false);
            Add(Day, FromClose, DT("01/05/2020 17:00:00.000"), false);
            Add(Day, null, DT("01/05/2020 17:00:00.000"), false);
            Add(Week, FromClose, DT("01/10/2020 16:59:59.999"), true);
            Add(Week, null, DT("01/10/2020 16:59:59.999"), true);
            Add(Week, FromClose, DT("01/10/2020 16:30:00.000"), true);
            Add(Week, null, DT("01/10/2020 16:30:00.000"), true);
            Add(Week, FromClose, DT("01/10/2020 16:29:59.999"), false);
            Add(Week, null, DT("01/10/2020 16:29:59.999"), false);
            Add(Week, FromClose, DT("01/09/2020 17:00:00.000"), false);
            Add(Week, null, DT("01/09/2020 17:00:00.000"), false);
        }
    }

    [Theory]
    [ClassData(typeof(MustBeFlatWithMixedArgsData))]
    public void MustBeFlatWithMixedArgs(
        Extent extent, int? fromClose, TickOn tickOn, bool expected)
    {
        var endCaps = fromClose.HasValue ? (0, fromClose!.Value) : default;

        var session = new Session(
            extent, new DateOnly(2020, 1, 6), endCaps);

        session.MustBeFlat(tickOn).Should().Be(expected);
    }

    //////////////////////////

    private class CanTradeWithMixedArgsData :
        Testing.TheoryData<Extent, int?, int?, TickOn, bool>
    {
        public CanTradeWithMixedArgsData()
        {
            Add(Day, FromOpen, FromClose, DT("01/05/2020 17:00:00.000"), false);
            Add(Day, null, null, DT("01/05/2020 17:00:00.000"), false);
            Add(Day, FromOpen, FromClose, DT("01/05/2020 17:14:59.999"), false);
            Add(Day, null, null, DT("01/05/2020 17:14:59.999"), false);
            Add(Day, FromOpen, FromClose, DT("01/05/2020 17:15:00.000"), true);
            Add(Day, null, null, DT("01/05/2020 17:15:00.000"), true);
            Add(Day, FromOpen, FromClose, DT("01/06/2020 16:29:59.999"), true);
            Add(Day, null, null, DT("01/06/2020 16:29:59.999"), true);
            Add(Day, FromOpen, FromClose, DT("01/06/2020 16:30:00.000"), false);
            Add(Day, null, null, DT("01/06/2020 16:30:00.000"), false);
            Add(Day, FromOpen, FromClose, DT("01/06/2020 16:59:59.999"), false);
            Add(Day, null, null, DT("01/06/2020 16:59:59.999"), false);
            Add(Week, FromOpen, FromClose, DT("01/05/2020 17:00:00.000"), false);
            Add(Week, null, null, DT("01/05/2020 17:00:00.000"), false);
            Add(Week, FromOpen, FromClose, DT("01/05/2020 17:14:59.999"), false);
            Add(Week, null, null, DT("01/05/2020 17:14:59.999"), false);
            Add(Week, FromOpen, FromClose, DT("01/05/2020 17:15:00.000"), true);
            Add(Week, null, null, DT("01/05/2020 17:15:00.000"), true);
            Add(Week, FromOpen, FromClose, DT("01/10/2020 16:29:59.999"), true);
            Add(Week, null, null, DT("01/10/2020 16:29:59.999"), true);
            Add(Week, FromOpen, FromClose, DT("01/10/2020 16:30:00.000"), false);
            Add(Week, null, null, DT("01/10/2020 16:30:00.000"), false);
            Add(Week, FromOpen, FromClose, DT("01/10/2020 16:59:59.999"), false);
            Add(Week, null, null, DT("01/10/2020 16:59:59.999"), false);
        }
    }

    [Theory]
    [ClassData(typeof(CanTradeWithMixedArgsData))]
    public void CanTradeWithMixedArgs(Extent extent,
        int? fromOpen, int? fromClose, TickOn tickOn, bool expected)
    {
        var endCaps = fromOpen.HasValue ?
            (fromOpen!.Value, fromClose!.Value) : default;

        var session = new Session(
            extent, new DateOnly(2020, 1, 6), endCaps);

        session.CanTrade(tickOn).Should().Be(expected);
    }

    //////////////////////////

    [Fact]
    public void GoodUntilReturnsExpectedValue()
    {
        var session = new Session(Day, new DateOnly(2020, 1, 6), (0, 10));

        session.GoodUntil.Should().Be(new DateTime(2020, 1, 6, 16, 50, 0));
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