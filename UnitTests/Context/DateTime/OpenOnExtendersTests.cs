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
using static System.TimeSpan;

namespace SquidEyes.UnitTests.Context;

public class OpenOnExtendersTests
{
    [Theory]
    [InlineData("00:00:00.000", false)]
    [InlineData("00:00:04.999", false)]
    [InlineData("00:00:05.000", true)]
    [InlineData("00:00:05.001", false)]
    [InlineData("01:00:00.000", true)]
    [InlineData("01:00:05.000", false)]
    public void IsLongIntervalWithMixedArgs(string intervalString, bool result) =>
        Parse(intervalString).Ticks.IsInterval().Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData("00:00:00.000", false)]
    [InlineData("00:00:04.999", false)]
    [InlineData("00:00:05.000", true)]
    [InlineData("00:00:05.001", false)]
    [InlineData("01:00:00.000", true)]
    [InlineData("01:00:05.000", false)]
    public void IsTimeSpanIntervalWithMixedArgs(string intervalString, bool result) =>
        Parse(intervalString).IsInterval().Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData("01/06/2020 16:00:45.000", false)]
    [InlineData("01/06/2020 16:00:59.999", false)]
    [InlineData("01/06/2020 17:00:00.000", true)]
    [InlineData("01/06/2020 17:00:00.001", false)]
    [InlineData("01/06/2020 17:00:14.999", false)]
    [InlineData("01/06/2020 17:00:15.000", true)]
    public void IsOpenOnWithMixedArgs(string tickOnString, bool result)
    {
        var session = new Session(Extent.Day, new DateOnly(2020, 1, 7));

        ((TickOn)tickOnString).IsOpenOn(session, FromSeconds(15)).Should().Be(result);
    }

    ////////////////////////////

    [Fact]
    public void IsOpenOnWithBadInterval()
    {
        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        FluentActions.Invoking(() => _ = session.MinTickOn.IsOpenOn(
            session, Zero)).Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Theory]    
    [InlineData("01/05/2020 17:00:00.000", "01/05/2020 17:00:00.000")]
    [InlineData("01/05/2020 17:00:14.999", "01/05/2020 17:00:00.000")]
    [InlineData("01/05/2020 17:00:15.000", "01/05/2020 17:00:15.000")]
    public void ToOpenOnWithGoodArgs(string tickOnString, DateTime result)
    {
        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        ((TickOn)tickOnString).ToOpenOn(session, FromSeconds(15)).Should().Be(result);
    }

    ////////////////////////////

    [Fact]
    public void ToOpenOnWithBadTickOn()
    {
        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        FluentActions.Invoking(() => _ = new TickOn().ToOpenOn(session,
            FromSeconds(5))).Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Fact]
    public void ToOpenOnWithBadInterval()
    {
        var session = new Session(Extent.Day, new DateOnly(2021, 1, 6));

        FluentActions.Invoking(() => _ = session.MinTickOn.ToOpenOn(
            session, Zero)).Should().Throw<ArgumentOutOfRangeException>();
    }
}