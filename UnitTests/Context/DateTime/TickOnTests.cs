﻿// ********************************************************
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

public class TickOnTests
{
    [Fact]
    public void ConstructWithGoodArgs()
    {
        foreach (var tradeDate in Known.TradeDates)
        {
            var minTickOn = tradeDate.ToMinTickOnValue(false);
            var maxTickOn = tradeDate.ToMaxTickOnValue(false);

            new TickOn(minTickOn).Value.Should().Be(minTickOn);
            new TickOn(maxTickOn).Value.Should().Be(maxTickOn);
        }
    }

    //////////////////////////

    [Fact]
    public void NoArgsConstruct()
    {
        var tickOn = new TickOn();

        tickOn.Value.Kind.Should().Be(DateTimeKind.Unspecified);
        tickOn.Should().Be(new TickOn(Known.MinTickOnValue));
        tickOn.Value.Should().Be(Known.MinTickOnValue);
        tickOn.IsEmpty.Should().Be(false);
    }

    //////////////////////////

    [Fact]
    public void SetToDefault()
    {
        TickOn tickOn = default;

        tickOn.Value.Kind.Should().Be(DateTimeKind.Unspecified);
        tickOn.Should().Be(default);
        tickOn.Value.Should().Be(default);
        tickOn.IsEmpty.Should().Be(true);
    }

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ContructWithBadArgs(bool minValue)
    {
        var dateTime = minValue ? Known.MinTickOnValue.AddMilliseconds(-1)
            : Known.MaxTickOnValue.AddMilliseconds(1);

        FluentActions.Invoking(() => _ = new TickOn(dateTime))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    private class IsTickOnWithMixedArgsData
        : Testing.TheoryData<DateTime, bool>
    {
        public IsTickOnWithMixedArgsData()
        {
            Add(Known.MinTickOnValue.AddMilliseconds(-1), false);
            Add(Known.MinTickOnValue, true);
            Add(Known.MaxTickOnValue, true);
            Add(Known.MaxTickOnValue.AddMilliseconds(1), false);
        }
    }

    [Theory]
    [ClassData(typeof(IsTickOnWithMixedArgsData))]
    public void IsTickOnWithMixedArgs(DateTime value, bool result) =>
        TickOn.IsTickOn(value).Should().Be(result);

    //////////////////////////

    private class IsTickOnWithGoodArgsData
    : Testing.TheoryData<DateTime, bool>
    {
        public IsTickOnWithGoodArgsData()
        {
            Add(DT("01/05/2020 16:59:59.999", Utc), false);
            Add(DT("01/05/2020 17:00:00.000", Utc), false);
            Add(DT("01/06/2020 16:59:59.999", Utc), false);
            Add(DT("01/06/2020 17:00:00.000", Utc), false);
            Add(DT("01/07/2020 16:59:59.999", Utc), false);
            Add(DT("01/07/2020 17:00:00.000", Utc), false);
            Add(DT("01/08/2020 16:59:59.999", Utc), false);
            Add(DT("01/08/2020 17:00:00.000", Utc), false);
            Add(DT("01/09/2020 16:59:59.999", Utc), false);
            Add(DT("01/09/2020 17:00:00.000", Utc), false);
            Add(DT("01/10/2020 16:59:59.999", Utc), false);
            Add(DT("01/10/2020 17:00:00.000", Utc), false);
            Add(DT("01/05/2020 16:59:59.999", Unspecified), false);
            Add(DT("01/05/2020 17:00:00.000", Unspecified), true);
            Add(DT("01/06/2020 16:59:59.999", Unspecified), true);
            Add(DT("01/06/2020 17:00:00.000", Unspecified), true);
            Add(DT("01/07/2020 16:59:59.999", Unspecified), true);
            Add(DT("01/07/2020 17:00:00.000", Unspecified), true);
            Add(DT("01/08/2020 16:59:59.999", Unspecified), true);
            Add(DT("01/08/2020 17:00:00.000", Unspecified), true);
            Add(DT("01/09/2020 16:59:59.999", Unspecified), true);
            Add(DT("01/09/2020 17:00:00.000", Unspecified), true);
            Add(DT("01/10/2020 16:59:59.999", Unspecified), true);
            Add(DT("01/10/2020 17:00:00.000", Unspecified), false);
        }
    }

    [Theory]
    [ClassData(typeof(IsTickOnWithGoodArgsData))]
    public void IsTickOnWithGoodArgs(DateTime tickOn, bool result) =>
        tickOn.IsTickOn().Should().Be(result);

    //////////////////////////

    [Fact]
    public void TickOnNotEqualToNullTickOn() =>
          new TickOn().Equals(null).Should().BeFalse();

    ////////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        GetTickOn(0).GetHashCode().Should().Be(GetTickOn(0).GetHashCode());

        GetTickOn(0).GetHashCode().Should().NotBe(GetTickOn(1).GetHashCode());
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GenericEquals(bool result) => GetTickOn(1)
        .Equals(result ? GetTickOn(1) : GetTickOn(2)).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ObjectEquals(bool result) => GetTickOn(1)
        .Equals(result ? GetTickOn(1) : GetTickOn(2)).Should().Be(result);

    ////////////////////////////

    [Fact]
    public void NullNotEquals() =>
        GetTickOn(1).Equals(null).Should().BeFalse();

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EqualsOperator(bool result) => (GetTickOn(1)
        == (result ? GetTickOn(1) : GetTickOn(2))).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NotEqualsOperator(bool result) => (GetTickOn(1)
        != (result ? GetTickOn(2) : GetTickOn(1))).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(1, 2, -1)]
    [InlineData(2, 2, 0)]
    [InlineData(3, 2, 1)]
    public void CompareToWithMixedArgs(int v1, int v2, int result) =>
        GetTickOn(v1).CompareTo(GetTickOn(v2)).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(3, 3, false)]
    [InlineData(2, 3, true)]
    [InlineData(1, 3, true)]
    public void LessThanOperator(int v1, int v2, bool result) =>
        (GetTickOn(v1) < GetTickOn(v2)).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(1, 1, false)]
    [InlineData(2, 1, true)]
    [InlineData(3, 1, true)]
    public void GreaterThanOperator(int v1, int v2, bool result) =>
        (GetTickOn(v1) > GetTickOn(v2)).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(2, 4, true)]
    [InlineData(2, 3, true)]
    [InlineData(2, 2, true)]
    [InlineData(2, 1, false)]
    public void LessThanOrEqualToOperator(int v1, int v2, bool result) =>
        (GetTickOn(v1) <= GetTickOn(v2)).Should().Be(result);

    ////////////////////////////

    [Theory]
    [InlineData(4, 2, true)]
    [InlineData(3, 2, true)]
    [InlineData(2, 2, true)]
    [InlineData(1, 2, false)]
    public void GreaterThanOrEqualToOperator(int v1, int v2, bool result) =>
        (GetTickOn(v1) >= GetTickOn(v2)).Should().Be(result);

    ////////////////////////////

    [Fact]
    public void FromTickOnToDateTimeToTickOn()
    {
        TickOn tickOn = Known.MinTickOnValue;

        var dateTime = (DateTime)tickOn;

        tickOn.Value.Should().Be(dateTime);

        tickOn.Should().Be(dateTime);
    }

    ////////////////////////////

    [Fact]
    public void ParseDateTimeToTickOn()
    {
        TickOn.Parse("01/05/2020 17:00:00.000").Should().Be(
            new DateTime(2020, 1, 5, 17, 0, 0, 0, Unspecified));
    }

    ////////////////////////////

    private static TickOn GetTickOn(int days) =>
        new(Known.MinTickOnValue.AddDays(days));
}

