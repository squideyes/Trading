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

public class TradeDateExtendersTests
{
    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(7, true)]
    [InlineData(8, true)]
    [InlineData(9, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void IsTradeDateWithMixedArgs(int day, bool result) =>
        new DateOnly(2020, 1, day).IsTradeDate().Should().Be(result);

    //////////////////////////

    private class ToTradeDateWithGoodArgsData
        : Testing.TheoryData<DateTime, DateOnly>
    {
        public ToTradeDateWithGoodArgsData()
        {
            Add(DT("01/05/2020 17:00:00.000"), DO("01/06/2020"));
            Add(DT("01/06/2020 16:59:59.999"), DO("01/06/2020"));
            Add(DT("01/06/2020 17:00:00.000"), DO("01/07/2020"));
            Add(DT("01/07/2020 16:59:59.999"), DO("01/07/2020"));
            Add(DT("01/07/2020 17:00:00.000"), DO("01/08/2020"));
            Add(DT("01/08/2020 16:59:59.999"), DO("01/08/2020"));
            Add(DT("01/08/2020 17:00:00.000"), DO("01/09/2020"));
            Add(DT("01/09/2020 16:59:59.999"), DO("01/09/2020"));
            Add(DT("01/09/2020 17:00:00.000"), DO("01/10/2020"));
            Add(DT("01/10/2020 16:59:59.999"), DO("01/10/2020"));
        }
    }

    [Theory]
    [ClassData(typeof(ToTradeDateWithGoodArgsData))]
    public void ToTradeDateWithGoodArgs(DateTime tickOn, DateOnly date) =>
        tickOn.ToTradeDate(true).Should().Be(date);

    //////////////////////////

    private class ToTradeDateWithBadArgsData : Testing.TheoryData<DateTime>
    {
        public ToTradeDateWithBadArgsData()
        {
            Add(DT("01/05/2020 16:59:59.999"));
            Add(DT("01/10/2020 17:00:00.000"));
        }
    }

    [Theory]
    [ClassData(typeof(ToTradeDateWithBadArgsData))]
    public void ToTradeDateWithBadArgs(DateTime tickOn)
    {
        FluentActions.Invoking(() => _ = tickOn.ToTradeDate(true))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(3, 6)]
    [InlineData(6, 7)]
    [InlineData(7, 8)]
    [InlineData(8, 9)]
    [InlineData(9, 10)]
    [InlineData(10, 13)]
    public void ToNextTradeDateReturnsExpectedResult(int day, int nextDay)
    {
        new DateOnly(2020, 1, day).ToNextTradeDate()
            .Should().Be(new DateOnly(2020, 1, nextDay));
    }

    //////////////////////////

    [Theory]
    [InlineData(13, 10)]
    [InlineData(10, 9)]
    [InlineData(9, 8)]
    [InlineData(8, 7)]
    [InlineData(7, 6)]
    [InlineData(5, 3)]
    public void ToPrevTradeDateReturnsExpectedResult(int day, int prevDay)
    {
        new DateOnly(2020, 1, day).ToPrevTradeDate()
            .Should().Be(new DateOnly(2020, 1, prevDay));
    }

    //////////////////////////

    [Theory]
    [InlineData(3, 3)]
    [InlineData(4, 6)]
    [InlineData(5, 6)]
    [InlineData(6, 6)]
    [InlineData(7, 7)]
    [InlineData(8, 8)]
    [InlineData(9, 9)]
    [InlineData(10, 10)]
    [InlineData(11, 13)]
    [InlineData(12, 13)]
    [InlineData(13, 13)]
    public void ToThisOrNextTradeDateReturnsExpectedResult(int day, int nextDay)
    {
        new DateOnly(2020, 1, day).ToThisOrNextTradeDate()
            .Should().Be(new DateOnly(2020, 1, nextDay));
    }

    //////////////////////////

    [Theory]
    [InlineData(13, 13)]
    [InlineData(12, 10)]
    [InlineData(11, 10)]
    [InlineData(10, 10)]
    [InlineData(9, 9)]
    [InlineData(8, 8)]
    [InlineData(7, 7)]
    [InlineData(6, 6)]
    [InlineData(5, 3)]
    [InlineData(4, 3)]
    [InlineData(3, 3)]
    public void ToThisOrPrtevTradeDateReturnsExpectedResult(int day, int prevDay)
    {
        new DateOnly(2020, 1, day).ToThisOrPrevTradeDate()
            .Should().Be(new DateOnly(2020, 1, prevDay));
    }
}
