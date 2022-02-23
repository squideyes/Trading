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

public class DateOnlyExtendersTests
{
    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(7, true)]
    [InlineData(8, true)]
    [InlineData(9, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void IsWeekdayWithMixedArgs(int day, bool result) =>
        new DateOnly(2020, 1, day).IsWeekday().Should().Be(result);

    //////////////////////////

    [Fact]
    public void ToDateOnlyTextWithGoodValue() =>
        new DateOnly(1, 2, 3).ToDateText().Should().Be("02/03/0001");
}