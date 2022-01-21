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

public class ExtentExtendersTests
{
    [Theory]
    [InlineData(5, Extent.Day, false)]
    [InlineData(6, Extent.Day, true)]
    [InlineData(7, Extent.Day, true)]
    [InlineData(8, Extent.Day, true)]
    [InlineData(9, Extent.Day, true)]
    [InlineData(10, Extent.Day, true)]
    [InlineData(11, Extent.Day, false)]
    [InlineData(5, Extent.Week, false)]
    [InlineData(6, Extent.Week, true)]
    [InlineData(7, Extent.Week, false)]
    [InlineData(8, Extent.Week, false)]
    [InlineData(9, Extent.Week, false)]
    [InlineData(10, Extent.Week, false)]
    [InlineData(11, Extent.Week, false)]
    public void IsValidDayOfWeekWithMixedArgs(int day, Extent extent, bool result) =>
        extent.IsValidDayOfWeekForExtent(new DateOnly(2020, 1, day)).Should().Be(result);
}