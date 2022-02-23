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

public class EmbargoSetTests
{
    [Theory]
    [InlineData("01/05/2020 17:00:00.000", null)]
    [InlineData("01/05/2020 17:59:59.999", null)]
    [InlineData("01/05/2020 18:00:00.000", Offset)]
    [InlineData("01/05/2020 18:59:59.999", Offset)]
    [InlineData("01/05/2020 19:00:00.000", null)]
    [InlineData("01/06/2020 12:59:59.999", null)]
    [InlineData("01/06/2020 13:00:00.000", AdHoc)]
    [InlineData("01/06/2020 13:59:59.000", AdHoc)]
    [InlineData("01/06/2020 14:00:00.000", null)]
    [InlineData("01/06/2020 14:59:59.999", null)]
    [InlineData("01/06/2020 15:00:00.000", OneTime)]
    [InlineData("01/06/2020 15:59:59.000", OneTime)]
    [InlineData("01/06/2020 16:00:00.000", null)]
    [InlineData("01/06/2020 16:59:59.000", null)]
    public void MultipleEmbargoesHonored(
        string tickOnString, EmbargoKind? kind)
    {
        TickOn tickOn = DateTime.Parse(tickOnString);

        var embargoes = new EmbargoSet(
            new Session(Extent.Day, new DateOnly(2020, 1, 6)));

        var minOffset = TimeSpan.FromHours(1);

        var maxOffset = TimeSpan.FromHours(2)
            .Add(TimeSpan.FromMilliseconds(-1));

        embargoes.Add(new OffsetEmbargo(
            minOffset, maxOffset, DayOfWeek.Monday));

        embargoes.Add(new OneTimeEmbargo(
            new DateTime(2020, 1, 6, 13, 0, 0),
            new DateTime(2020, 1, 6, 13, 59, 59, 999), true));

        embargoes.Add(new OneTimeEmbargo(
            new DateTime(2020, 1, 6, 15, 0, 0),
            new DateTime(2020, 1, 6, 15, 59, 59, 999), false));

        var (isEmbargoed, embargo) = embargoes.IsEmbargoed(tickOn);

        if (kind.HasValue)
            embargo!.Kind.Should().Be(kind);
        else
            isEmbargoed.Should().BeFalse();
    }
}