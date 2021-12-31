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
    private class IsIntervalWithGoodAndBadArgsData
        : Testing.TheoryData<TimeSpan, bool>
    {
        public IsIntervalWithGoodAndBadArgsData()
        {
            Add(TS("00:00:00.000"), false);
            Add(TS("00:00:04.999"), false);
            Add(TS("00:00:05.000"), true);
            Add(TS("00:00:05.001"), false);
            Add(TS("01:00:00.000"), true);
            Add(TS("01:00:05.000"), false);
        }
    }

    [Theory]
    [ClassData(typeof(IsIntervalWithGoodAndBadArgsData))]
    public void IsLongIntervalWithGoodAndBadArgs(TimeSpan interval, bool result) =>
        interval.Ticks.IsInterval().Should().Be(result);

    [Theory]
    [ClassData(typeof(IsIntervalWithGoodAndBadArgsData))]
    public void IsTimeSpanIntervalWithGoodAndBadArgs(TimeSpan interval, bool result) =>
        interval.IsInterval().Should().Be(result);

    ////////////////////////////

    private class IsOpenOnWithMixedArgsData
        : Testing.TheoryData<TickOn, bool>
    {
        public IsOpenOnWithMixedArgsData()
        {

            Add(DT("01/06/2020 16:00:45.000"), false);
            Add(DT("01/06/2020 16:00:59.999"), false);
            Add(DT("01/06/2020 17:00:00.000"), true);
            Add(DT("01/06/2020 17:00:00.001"), false);
            Add(DT("01/06/2020 17:00:14.999"), false);
            Add(DT("01/06/2020 17:00:15.000"), true);
        }
    }

    [Theory]
    [ClassData(typeof(IsOpenOnWithMixedArgsData))]
    public void IsOpenOnWithMixedArgs(TickOn tickOn, bool result)
    {
        var session = new Session(Extent.Day, new DateOnly(2020, 1, 7));

        tickOn.IsOpenOn(session, FromSeconds(15)).Should().Be(result);
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

    private class ToOpenOnWithGoodArgsData
        : Testing.TheoryData<TickOn, DateTime>
    {
        public ToOpenOnWithGoodArgsData()
        {
            Add(DT("01/05/2020 17:00:00.000"), DT("01/05/2020 17:00:00.000"));
            Add(DT("01/05/2020 17:00:14.999"), DT("01/05/2020 17:00:00.000"));
            Add(DT("01/05/2020 17:00:15.000"), DT("01/05/2020 17:00:15.000"));
        }
    }

    [Theory]
    [ClassData(typeof(ToOpenOnWithGoodArgsData))]
    public void ToOpenOnWithGoodArgs(TickOn tickOn, DateTime result)
    {
        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        tickOn.ToOpenOn(session, FromSeconds(15)).Should().Be(result);
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