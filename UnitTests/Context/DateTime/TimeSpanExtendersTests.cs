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

public class TimeSpanExtendersTests
{
    [Theory]
    [InlineData("0.02:03:04.005", true, "02:03:04.005")]
    [InlineData("0.02:03:04.005", false, "0.02:03:04.005")]
    [InlineData("1.02:03:04.005", true, "1.02:03:04.005")]
    [InlineData("1.02:03:04.005", false, "1.02:03:04.005")]
    [InlineData("02:03:04.005", true, "02:03:04.005")]
    [InlineData("02:03:04.005", false, "0.02:03:04.005")]
    public void ToTimeSpanTextWithGoodValue(
        string valueString, bool daysOptional, string result)
    {
        TimeSpan.Parse(valueString)
            .ToTimeSpanText(daysOptional).Should().Be(result);
    }
}