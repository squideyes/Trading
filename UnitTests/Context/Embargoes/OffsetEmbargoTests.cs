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
using static System.DayOfWeek;

namespace SquidEyes.UnitTests.Context;

public class OffsetEmbargoTests
{
    [Theory]
    [InlineData("00:00:00.000", "23:59:59.999", null)]
    [InlineData("00:00:00.000", "23:59:59.999", Monday)]
    [InlineData("00:00:00.000", "23:59:59.999", Tuesday)]
    [InlineData("00:00:00.000", "23:59:59.999", Wednesday)]
    [InlineData("00:00:00.000", "23:59:59.999", Thursday)]
    [InlineData("00:00:00.000", "23:59:59.999", Friday)]
    public void GoodConstructorArgs(
        string minOffsetString, string maxOffsetString, DayOfWeek? dayOfWeek)
    {
        var minOffset = TimeSpan.Parse(minOffsetString);
        var maxOffset = TimeSpan.Parse(maxOffsetString);

        _ = new OffsetEmbargo(minOffset, maxOffset, dayOfWeek);
    }

    //////////////////////////

    [Theory]
    [InlineData("-00:00:00.001", "23:59:59.999", Monday)]
    [InlineData("1.00:00:00.000", "1.00:00:00.001", Monday)]
    [InlineData("00:00:00.001", "00:00:00.000", Monday)]
    [InlineData("00:00:00.000", "1.00:00:00.000", Monday)]
    [InlineData("-00:00:00.001", "23:59:59.999", Tuesday)]
    [InlineData("1.00:00:00.000", "1.00:00:00.001", Tuesday)]
    [InlineData("00:00:00.001", "00:00:00.000", Tuesday)]
    [InlineData("00:00:00.000", "1.00:00:00.000", Tuesday)]
    [InlineData("-00:00:00.001", "23:59:59.999", Wednesday)]
    [InlineData("1.00:00:00.000", "1.00:00:00.001", Wednesday)]
    [InlineData("00:00:00.001", "00:00:00.000", Wednesday)]
    [InlineData("00:00:00.000", "1.00:00:00.000", Wednesday)]
    [InlineData("-00:00:00.001", "23:59:59.999", Thursday)]
    [InlineData("1.00:00:00.000", "1.00:00:00.001", Thursday)]
    [InlineData("00:00:00.001", "00:00:00.000", Thursday)]
    [InlineData("00:00:00.000", "1.00:00:00.000", Thursday)]
    [InlineData("-00:00:00.001", "23:59:59.999", Friday)]
    [InlineData("1.00:00:00.000", "1.00:00:00.001", Friday)]
    [InlineData("00:00:00.001", "00:00:00.000", Friday)]
    [InlineData("00:00:00.000", "1.00:00:00.000", Friday)]
    [InlineData("00:00:00.000", "23:59:59.999", Saturday)]
    [InlineData("00:00:00.000", "23:59:59.999", Sunday)]
    [InlineData("00:00:00.000", "23:59:59.999", (DayOfWeek)8)]
    public void BadContructorArgs(
        string minOffsetString, string maxOffsetString, DayOfWeek? dayOfWeek)
    {
        var minOffset = TimeSpan.Parse(minOffsetString);
        var maxOffset = TimeSpan.Parse(maxOffsetString);

        FluentActions.Invoking(() =>
            _ = new OffsetEmbargo(minOffset, maxOffset, dayOfWeek))
                .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(Saturday)]
    [InlineData(Sunday)]
    public void BadDayOfWeek(DayOfWeek dayOfWeek)
    {
        FluentActions.Invoking(() => _ = GetEmbargo(dayOfWeek))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(Day, Monday, "01/05/2020 17:00:00.000", false)]
    [InlineData(Day, Monday, "01/05/2020 17:59:59.999", false)]
    [InlineData(Day, Monday, "01/05/2020 18:00:00.000", true)]
    [InlineData(Day, Monday, "01/05/2020 18:59:59.999", true)]
    [InlineData(Day, Monday, "01/05/2020 19:00:00.000", false)]
    [InlineData(Day, Monday, "01/06/2020 16:59:59.999", false)]
    [InlineData(Day, Tuesday, "01/06/2020 17:00:00.000", false)]
    [InlineData(Day, Tuesday, "01/06/2020 17:59:59.999", false)]
    [InlineData(Day, Tuesday, "01/06/2020 18:00:00.000", true)]
    [InlineData(Day, Tuesday, "01/06/2020 18:59:59.999", true)]
    [InlineData(Day, Tuesday, "01/06/2020 19:00:00.000", false)]
    [InlineData(Day, Tuesday, "01/07/2020 16:59:59.999", false)]
    [InlineData(Day, Wednesday, "01/07/2020 17:00:00.000", false)]
    [InlineData(Day, Wednesday, "01/07/2020 17:59:59.999", false)]
    [InlineData(Day, Wednesday, "01/07/2020 18:00:00.000", true)]
    [InlineData(Day, Wednesday, "01/07/2020 18:59:59.999", true)]
    [InlineData(Day, Wednesday, "01/07/2020 19:00:00.000", false)]
    [InlineData(Day, Wednesday, "01/08/2020 16:59:59.999", false)]
    [InlineData(Day, Thursday, "01/08/2020 17:00:00.000", false)]
    [InlineData(Day, Thursday, "01/08/2020 17:59:59.999", false)]
    [InlineData(Day, Thursday, "01/08/2020 18:00:00.000", true)]
    [InlineData(Day, Thursday, "01/08/2020 18:59:59.999", true)]
    [InlineData(Day, Thursday, "01/08/2020 19:00:00.000", false)]
    [InlineData(Day, Thursday, "01/09/2020 16:59:59.999", false)]
    [InlineData(Day, Friday, "01/09/2020 17:00:00.000", false)]
    [InlineData(Day, Friday, "01/09/2020 17:59:59.999", false)]
    [InlineData(Day, Friday, "01/09/2020 18:00:00.000", true)]
    [InlineData(Day, Friday, "01/09/2020 18:59:59.999", true)]
    [InlineData(Day, Friday, "01/09/2020 19:00:00.000", false)]
    [InlineData(Day, Friday, "01/10/2020 16:59:59.999", false)]
    [InlineData(Week, Monday, "01/05/2020 18:00:00.000", true)]
    [InlineData(Week, Monday, "01/05/2020 18:59:59.999", true)]
    [InlineData(Week, Monday, "01/05/2020 19:00:00.000", false)]
    [InlineData(Week, Monday, "01/06/2020 16:59:59.999", false)]
    [InlineData(Week, Tuesday, "01/06/2020 17:00:00.000", false)]
    [InlineData(Week, Tuesday, "01/06/2020 17:59:59.999", false)]
    [InlineData(Week, Tuesday, "01/06/2020 18:00:00.000", true)]
    [InlineData(Week, Tuesday, "01/06/2020 18:59:59.999", true)]
    [InlineData(Week, Tuesday, "01/06/2020 19:00:00.000", false)]
    [InlineData(Week, Tuesday, "01/07/2020 16:59:59.999", false)]
    [InlineData(Week, Wednesday, "01/07/2020 17:00:00.000", false)]
    [InlineData(Week, Wednesday, "01/07/2020 17:59:59.999", false)]
    [InlineData(Week, Wednesday, "01/07/2020 18:00:00.000", true)]
    [InlineData(Week, Wednesday, "01/07/2020 18:59:59.999", true)]
    [InlineData(Week, Wednesday, "01/07/2020 19:00:00.000", false)]
    [InlineData(Week, Wednesday, "01/08/2020 16:59:59.999", false)]
    [InlineData(Week, Thursday, "01/08/2020 17:00:00.000", false)]
    [InlineData(Week, Thursday, "01/08/2020 17:59:59.999", false)]
    [InlineData(Week, Thursday, "01/08/2020 18:00:00.000", true)]
    [InlineData(Week, Thursday, "01/08/2020 18:59:59.999", true)]
    [InlineData(Week, Thursday, "01/08/2020 19:00:00.000", false)]
    [InlineData(Week, Thursday, "01/09/2020 16:59:59.999", false)]
    [InlineData(Week, Friday, "01/09/2020 17:00:00.000", false)]
    [InlineData(Week, Friday, "01/09/2020 17:59:59.999", false)]
    [InlineData(Week, Friday, "01/09/2020 18:00:00.000", true)]
    [InlineData(Week, Friday, "01/09/2020 18:59:59.999", true)]
    [InlineData(Week, Friday, "01/09/2020 19:00:00.000", false)]
    [InlineData(Week, Friday, "01/10/2020 16:59:59.999", false)]
    public void IsEmbargoedWithMixedArgs(
        Extent extent, DayOfWeek? dayOfWeek, string tickOnString, bool result)
    {
        var session = new Session(extent, new DateOnly(2020, 1, 6));

        TickOn tickOn = DateTime.Parse(tickOnString);

        GetEmbargo(dayOfWeek).IsEmbargoed(session, tickOn).Should().Be(result);
    }

    ////////////////////////////

    [Fact]
    public void GoodToString()
    {
        GetEmbargo(Monday).ToString().Should().Be(
            "Offset Embargo (01:00:00.000 to 01:59:59.999; Monday)");
    }

    //////////////////////////

    private static OffsetEmbargo GetEmbargo(DayOfWeek? dayOfWeek)
    {
        var minOffset = new TimeSpan(1, 0, 0);
        var maxOffset = new TimeSpan(0, 1, 59, 59, 999);

        return new OffsetEmbargo(minOffset, maxOffset, dayOfWeek);
    }
}