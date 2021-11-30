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
using static System.DateTimeKind;

namespace SquidEyes.UnitTests.Context;

public class DateTimeExtendersTests
{
    [Fact]
    public void ToCandleOnTextWithGoodValue()
    {
        new DateTime(1, 2, 3, 4, 5, 6, 7).ToCandleOnText()
            .Should().Be("02/03/0001 04:05:06");
    }

    //////////////////////////

    [Fact]
    public void ToDateTimeTextWithGoodValue() =>
        new DateTime(1, 2, 3).ToDateText().Should().Be("02/03/0001");

    //////////////////////////

    [Fact]
    public void ToTickOnTextWithGoodValue()
    {
        new DateTime(1, 2, 3, 4, 5, 6, 7).ToTickOnText()
            .Should().Be("02/03/0001 04:05:06.007");
    }
}
