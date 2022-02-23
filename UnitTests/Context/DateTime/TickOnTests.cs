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
using System.Globalization;
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

        tickOn.Value.Kind.Should().Be(Unspecified);
        tickOn.Should().Be(new TickOn(Known.MinTickOnValue));
        tickOn.Value.Should().Be(Known.MinTickOnValue);
        tickOn.IsEmpty.Should().Be(false);
    }

    //////////////////////////

    [Fact]
    public void SetToDefault()
    {
        TickOn tickOn = default;

        tickOn.Value.Kind.Should().Be(Unspecified);
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

    [Theory]
    [InlineData(false, -1, false)]
    [InlineData(false, 0, true)]
    [InlineData(true, 0, true)]
    [InlineData(true, 1, false)]
    public void IsTickOnWithMixedArgs(bool useMaxValue, int offset, bool result)
    {
        var value = (useMaxValue ? Known.MaxTickOnValue
            : Known.MinTickOnValue).AddMilliseconds(offset);

        TickOn.IsTickOn(value).Should().Be(result);
    }

    //////////////////////////

    [Theory]
    [InlineData("01/05/2020 16:59:59.999", true, false)]
    [InlineData("01/05/2020 17:00:00.000", true, false)]
    [InlineData("01/05/2020 23:59:59.999", true, false)]
    [InlineData("01/06/2020 00:00:00.000", true, false)]
    [InlineData("01/06/2020 16:59:59.999", true, false)]
    [InlineData("01/06/2020 17:00:00.000", true, false)]
    [InlineData("01/06/2020 23:59:59.999", true, false)]
    [InlineData("01/07/2020 00:00:00.000", true, false)]
    [InlineData("01/07/2020 16:59:59.999", true, false)]
    [InlineData("01/07/2020 17:00:00.000", true, false)]
    [InlineData("01/07/2020 23:59:59.999", true, false)]
    [InlineData("01/08/2020 00:00:00.000", true, false)]
    [InlineData("01/08/2020 16:59:59.999", true, false)]
    [InlineData("01/08/2020 17:00:00.000", true, false)]
    [InlineData("01/08/2020 23:59:59.999", true, false)]
    [InlineData("01/09/2020 00:00:00.000", true, false)]
    [InlineData("01/09/2020 16:59:59.999", true, false)]
    [InlineData("01/09/2020 17:00:00.000", true, false)]
    [InlineData("01/09/2020 23:59:59.999", true, false)]
    [InlineData("01/10/2020 00:00:00.000", true, false)]
    [InlineData("01/10/2020 16:59:59.999", true, false)]
    [InlineData("01/10/2020 17:00:00.000", true, false)]
    [InlineData("01/05/2020 16:59:59.999", false, false)]
    [InlineData("01/05/2020 17:00:00.000", false, true)]
    [InlineData("01/05/2020 23:59:59.999", false, true)]
    [InlineData("01/06/2020 00:00:00.000", false, true)]
    [InlineData("01/06/2020 16:59:59.999", false, true)]
    [InlineData("01/06/2020 17:00:00.000", false, true)]
    [InlineData("01/06/2020 23:59:59.999", false, true)]
    [InlineData("01/07/2020 00:00:00.000", false, true)]
    [InlineData("01/07/2020 16:59:59.999", false, true)]
    [InlineData("01/07/2020 17:00:00.000", false, true)]
    [InlineData("01/07/2020 23:59:59.999", false, true)]
    [InlineData("01/08/2020 00:00:00.000", false, true)]
    [InlineData("01/08/2020 16:59:59.999", false, true)]
    [InlineData("01/08/2020 17:00:00.000", false, true)]
    [InlineData("01/08/2020 23:59:59.999", false, true)]
    [InlineData("01/09/2020 00:00:00.000", false, true)]
    [InlineData("01/09/2020 16:59:59.999", false, true)]
    [InlineData("01/09/2020 17:00:00.000", false, true)]
    [InlineData("01/09/2020 23:59:59.999", false, true)]
    [InlineData("01/10/2020 00:00:00.000", false, true)]
    [InlineData("01/10/2020 16:59:59.999", false, true)]
    [InlineData("01/10/2020 17:00:00.000", false, false)]
    public void IsTickOnWithGoodArgs(string tickOnString, bool utc, bool result)
    {
        var styles = utc ? DateTimeStyles.AssumeUniversal : DateTimeStyles.None;

        DateTime.Parse(tickOnString, null, styles).IsTickOn().Should().Be(result);
    }

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

    [Fact]
    public void StringToTickOn()
    {
        TickOn tickOn = "01/05/2020 17:00:00.000";

        tickOn.Should().Be(new DateTime(2020, 1, 5, 17, 0, 0, 0, Unspecified));
    }

    ////////////////////////////

    private static TickOn GetTickOn(int days) => new(Known.MinTickOnValue.AddDays(days));
}