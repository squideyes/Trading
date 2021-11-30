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

public class DayOfWeekExtendersTests
{
    [Theory]
    [InlineData(DayOfWeek.Sunday, false)]
    [InlineData(DayOfWeek.Monday, true)]
    [InlineData(DayOfWeek.Tuesday, true)]
    [InlineData(DayOfWeek.Wednesday, true)]
    [InlineData(DayOfWeek.Thursday, true)]
    [InlineData(DayOfWeek.Friday, true)]
    [InlineData(DayOfWeek.Saturday, false)]
    public void IsWeekdayWithMixedArgs(DayOfWeek dayOfWeek, bool result) =>
        dayOfWeek.IsWeekday().Should().Be(result);
}
