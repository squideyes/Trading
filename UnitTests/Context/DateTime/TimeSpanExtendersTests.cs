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
    private class ToTimeSpanTextWithGoodValueData
        : Testing.TheoryData<TimeSpan, bool, string>
    {
        public ToTimeSpanTextWithGoodValueData()
        {
            Add(TS("0.02:03:04.005"), true, "02:03:04.005");
            Add(TS("0.02:03:04.005"), false, "0.02:03:04.005");
            Add(TS("1.02:03:04.005"), true, "1.02:03:04.005");
            Add(TS("1.02:03:04.005"), false, "1.02:03:04.005");
            Add(TS("02:03:04.005"), true, "02:03:04.005");
            Add(TS("02:03:04.005"), false, "0.02:03:04.005");
        }
    }

    [Theory]
    [ClassData(typeof(ToTimeSpanTextWithGoodValueData))]
    public void ToTimeSpanTextWithGoodValue(
        TimeSpan value, bool daysOptional, string result)
    {
        value.ToTimeSpanText(daysOptional).Should().Be(result);
    }
}