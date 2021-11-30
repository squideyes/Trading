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

namespace SquidEyes.UnitTests.Context;

public class ClockTests
{
    [Theory]
    [InlineData((Unit)0, 1)]
    [InlineData(Unit.Hours, 0)]
    [InlineData(Unit.Minutes, 0)]
    [InlineData(Unit.Seconds, 0)]
    [InlineData(Unit.Hours, 5)]
    [InlineData(Unit.Minutes, 60)]
    [InlineData(Unit.Seconds, 60)]
    public void BadConstructorArgs(Unit unit, int quantity)
    {
        FluentActions.Invoking(() => _ = new Clock(unit, quantity))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(Unit.Hours, 4)]
    [InlineData(Unit.Minutes, 59)]
    [InlineData(Unit.Seconds, 59)]
    public void GoodConstructorArgs(Unit unit, int maxQuantity)
    {
        for (int quantity = 1; quantity <= maxQuantity; quantity++)
        {
            var clock = new Clock(unit, quantity);

            clock.Unit.Should().Be(unit);
            clock.Quantity.Should().Be(quantity);
        }
    }

    [Theory]
    [InlineData(Unit.Hours, 4)]
    [InlineData(Unit.Minutes, 59)]
    [InlineData(Unit.Seconds, 59)]
    public void ClockToStringToClock(Unit unit, int maxQuantity)
    {
        for (int quantity = 1; quantity <= maxQuantity; quantity++)
        {
            var clock1 = new Clock(unit, quantity);

            var clockAsString = clock1.ToString();

            var clock2 = Clock.Parse(clockAsString);

            clock2.Should().Be(clock1);
        }
    }

    [Theory]
    [InlineData("H01", Unit.Hours, 1)]
    [InlineData("H04", Unit.Hours, 4)]
    [InlineData("M01", Unit.Minutes, 1)]
    [InlineData("M59", Unit.Minutes, 59)]
    [InlineData("S01", Unit.Seconds, 1)]
    [InlineData("S59", Unit.Seconds, 59)]
    public void ImplicitStringToClock(
        string code, Unit unit, int quantity)
    {
        var clock = Clock.Parse(code);

        clock.Unit.Should().Be(unit);
        clock.Quantity.Should().Be(quantity);
    }

    [Theory]
    [InlineData("H01", Unit.Hours, 1)]
    [InlineData("H04", Unit.Hours, 4)]
    [InlineData("M01", Unit.Minutes, 1)]
    [InlineData("M59", Unit.Minutes, 59)]
    [InlineData("S01", Unit.Seconds, 1)]
    [InlineData("S59", Unit.Seconds, 59)]
    public void ImplicitClockToString(
        string code, Unit unit, int quantity)
    {
        var clock = new Clock(unit, quantity);

        clock.Unit.Should().Be(unit);
        clock.Quantity.Should().Be(quantity);

        string asString = clock;

        asString.Should().Be(code);
    }

    [Fact]
    public void ClockNotEqualToNullClock() =>
        new Clock(Unit.Hours, 1).Equals(null).Should().BeFalse();

    [Fact]
    public void ClockNotEqualToNullObject() =>
        new Clock(Unit.Hours, 1).Equals((object?)null).Should().BeFalse();

    [Theory]
    [InlineData("H01", "H01", true)]
    [InlineData("H01", "H02", false)]
    [InlineData("H04", "H04", true)]
    [InlineData("M01", "M01", true)]
    [InlineData("M01", "M02", false)]
    [InlineData("M59", "M59", true)]
    [InlineData("S01", "S01", true)]
    [InlineData("S01", "S02", false)]
    [InlineData("S59", "S59", true)]
    public void ClockEqualClock(string clock1, string clock2, bool result) =>
        Clock.Parse(clock1).Equals(Clock.Parse(clock2)).Should().Be(result);

    [Theory]
    [InlineData("H1", "H1", true)]
    [InlineData("H1", "M1", false)]
    [InlineData("H1", "S1", false)]
    [InlineData("H1", "H2", false)]
    [InlineData("M1", "M1", true)]
    [InlineData("M1", "H1", false)]
    [InlineData("M1", "S1", false)]
    [InlineData("M1", "M2", false)]
    [InlineData("S1", "S1", true)]
    [InlineData("S1", "H1", false)]
    [InlineData("S1", "M1", false)]
    [InlineData("S1", "S2", false)]
    public void ClockEqualOperator(string c1, string c2, bool result) =>
        (Clock.Parse(c1) == Clock.Parse(c2)).Should().Be(result);

    [Theory]
    [InlineData("H1", "H1", false)]
    [InlineData("H1", "M1", true)]
    [InlineData("H1", "S1", true)]
    [InlineData("H1", "H2", true)]
    [InlineData("M1", "M1", false)]
    [InlineData("M1", "H1", true)]
    [InlineData("M1", "S1", true)]
    [InlineData("M1", "M2", true)]
    [InlineData("S1", "S1", false)]
    [InlineData("S1", "H1", true)]
    [InlineData("S1", "M1", true)]
    [InlineData("S1", "S2", true)]
    public void ClockNotEqualOperator(string c1, string c2, bool result) =>
        (Clock.Parse(c1) != Clock.Parse(c2)).Should().Be(result);
}
