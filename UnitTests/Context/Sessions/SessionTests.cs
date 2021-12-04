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

namespace SquidEyes.UnitTests.Context;

public class SessionTests
{
    private class GoodContructorArgsData
        : Testing.TheoryData<Extent, DateOnly, DateTime, DateTime>
    {
        public GoodContructorArgsData()
        {
            Add(Day, DO("01/06/2020"), DT("01/05/2020 17:00:00.000"), DT("01/06/2020 16:59:59.999"));
            Add(Day, DO("01/07/2020"), DT("01/06/2020 17:00:00.000"), DT("01/07/2020 16:59:59.999"));
            Add(Day, DO("01/08/2020"), DT("01/07/2020 17:00:00.000"), DT("01/08/2020 16:59:59.999"));
            Add(Day, DO("01/09/2020"), DT("01/08/2020 17:00:00.000"), DT("01/09/2020 16:59:59.999"));
            Add(Day, DO("01/10/2020"), DT("01/09/2020 17:00:00.000"), DT("01/10/2020 16:59:59.999"));
            Add(Week, DO("01/06/2020"), DT("01/05/2020 17:00:00.000"), DT("01/10/2020 16:59:59.999"));
        }
    }

    [Theory]
    [ClassData(typeof(GoodContructorArgsData))]
    public void GoodContructorArgs(
        Extent extent, DateOnly tradeDate, DateTime minTickOn, DateTime maxTickOn)
    {
        var session = new Session(extent, tradeDate);

        session.Extent.Should().Be(extent);
        session.TradeDate.Should().Be(tradeDate);
        session.MinTickOn.Should().Be(minTickOn);
        session.MaxTickOn.Should().Be(maxTickOn);
    }

    //////////////////////////

    private class BadContructorArgsData : Testing.TheoryData<Extent, DateOnly>
    {
        public BadContructorArgsData()
        {
            Add(0, DateOnly.MinValue);
            Add(Day, DO("01/05/2020"));
            Add(Week, DO("01/05/2020"));
        }
    }

    [Theory]
    [ClassData(typeof(BadContructorArgsData))]
    public void BadContructorArgs(Extent extent, DateOnly baseDate)
    {
        FluentActions.Invoking(() => _ = new Session(extent, baseDate))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(Day)]
    [InlineData(Week)]
    public void ToStringReturnsExpectedValue(Extent extent)
    {
        var tradeDate = new DateOnly(2020, 1, 6);

        new Session(extent, tradeDate).ToString()
            .Should().Be($"{tradeDate:MM/dd/yyyy} ({extent})");
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
    public void InSessionWithMixedArgs(Extent extent, DateTime tickOn, bool result)
    {
        new Session(extent, new DateOnly(2020, 1, 7))
            .InSession(tickOn).Should().Be(result);
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
    public void EqualsOperator(Extent extent, bool result) =>  (new Session(
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
