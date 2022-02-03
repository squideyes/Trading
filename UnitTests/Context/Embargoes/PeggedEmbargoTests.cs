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
using static SquidEyes.Trading.Context.EmbargoKind;

namespace SquidEyes.UnitTests.Context;

public class PeggedEmbargoTests
{
    [Theory]
    [InlineData(Extent.Day, PegTo.Open)]
    [InlineData(Extent.Day, PegTo.Close)]
    [InlineData(Extent.Week, PegTo.Open)]
    [InlineData(Extent.Week, PegTo.Close)]
    public void GoodContructorArgs(Extent extent, PegTo pegTo)
    {
        const int MINUTES = 15;

        var embargo = new PeggedEmbargo(pegTo, MINUTES);

        embargo.Kind.Should().Be(pegTo == PegTo.Open ? Open : Close);

        var session = new Session(extent, new DateOnly(2020, 1, 6));

        if (pegTo == PegTo.Open)
        {
            embargo.IsEmbargoed(session, session.MinTickOn).Should().BeTrue();

            embargo.IsEmbargoed(session, session.MinTickOn.Value
                .AddMinutes(MINUTES)).Should().BeFalse();

            embargo.IsEmbargoed(session, session.MinTickOn.Value
                .AddMinutes(MINUTES).AddMilliseconds(-1)).Should().BeTrue();
        }
        else
        {
            embargo.IsEmbargoed(session, session.MaxTickOn.Value
                .AddMinutes(-MINUTES)).Should().BeFalse();

            embargo.IsEmbargoed(session, session.MaxTickOn.Value.AddMinutes(-MINUTES)
                .AddMilliseconds(1)).Should().BeTrue();

            embargo.IsEmbargoed(session, session.MaxTickOn).Should().BeTrue();
        }
    }

    //////////////////////////

    [Theory]
    [InlineData((PegTo)0, 15)]
    [InlineData(PegTo.Open, 0)]
    [InlineData(PegTo.Open, 721)]
    [InlineData(PegTo.Close, 0)]
    [InlineData(PegTo.Close, 721)]
    public void BadContructorArgs(PegTo pegTo, int minutes)
    {
        FluentActions.Invoking(() => _ = new PeggedEmbargo(pegTo, minutes))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(PegTo.Open)]
    [InlineData(PegTo.Close)]
    public void IsEmbargoedWithBadTickOn(PegTo pegTo)
    {
        var embargo = new PeggedEmbargo(pegTo, 15);

        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        var tickOn = pegTo == PegTo.Open ? session.MinTickOn : session.MaxTickOn;

        embargo.IsEmbargoed(session, tickOn.Value.AddDays(1)).Should().BeFalse();
    }

    //////////////////////////

    [Fact]
    public void IsEmbargoedWithBadSession()
    {
        var embargo = new PeggedEmbargo(PegTo.Open, 15);

        var tickOn = new Session(Extent.Day, new DateOnly(2020, 1, 6)).MinTickOn;

        FluentActions.Invoking(() => _ = embargo.IsEmbargoed(null!, tickOn))
            .Should().Throw<ArgumentNullException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(PegTo.Open)]
    [InlineData(PegTo.Close)]
    public void GoodToString(PegTo pegTo)
    {
        new PeggedEmbargo(pegTo, 15).ToString()
            .Should().Be($"Pegged Embargo ({pegTo}; 15 Minutes)");
    }
}