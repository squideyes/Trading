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
using static SquidEyes.Trading.Context.Extent;

namespace SquidEyes.UnitTests.Context;

public class OneTimeEmbargoTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GoodConstructorArgs(bool isAdHoc) =>
        GetEmbargo(isAdHoc).Kind.Should().Be(isAdHoc ? AdHoc : OneTime);

    //////////////////////////

    [Theory]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    public void BadConstructorArgs(bool badMinTickOn, bool badMaxTickOn, bool isAdHoc)
    {
        var session = new Session(Day, new DateOnly(2020, 1, 6));
        var minTickOn = badMinTickOn ? default : session.MinTickOn;
        var maxTickOn = badMaxTickOn ? default : session.MaxTickOn;

        FluentActions.Invoking(() => _ = new OneTimeEmbargo(minTickOn,
            maxTickOn, isAdHoc)).Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData("01/05/2020 17:00:00.000", false)]
    [InlineData("01/05/2020 17:59:59.999", false)]
    [InlineData("01/05/2020 18:00:00.000", true)]
    [InlineData("01/05/2020 18:59:59.999", true)]
    [InlineData("01/05/2020 19:00:00.000", false)]
    [InlineData("01/06/2020 16:59:59.999", false)]
    public void IsEmbargoedWithGoodAndBadArgs(DateTime tickOn, bool result)
    {
        var session = new Session(Day, new DateOnly(2020, 1, 6));

        GetEmbargo(true).IsEmbargoed(session, tickOn).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToStringReturnsExpectedValue(bool isAdHoc)
    {
        var prefix = isAdHoc ? nameof(AdHoc) : nameof(OneTime);

        GetEmbargo(isAdHoc).ToString().Should().Be(
            $"{prefix} Embargo (01/05/2020 18:00:00.000 to 01/05/2020 18:59:59.999)");
    }

    //////////////////////////

    private static OneTimeEmbargo GetEmbargo(bool isAdHoc)
    {
        var minTickOn = new DateTime(2020, 1, 5, 18, 0, 0);
        var maxTickOn = new DateTime(2020, 1, 5, 18, 59, 59, 999);

        return new OneTimeEmbargo(minTickOn, maxTickOn, isAdHoc);
    }
}