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

public class AccountIdTests
{
    [Fact]
    public void CreateCreatesExpectedAccountId() =>
        AccountId.Create("AAAAAAAAAAAA", 1).Should().Be("AAAAAAAAAAAA(0001)");

    [Theory]
    [InlineData(null, 1)]
    [InlineData("AAAAAAAAAAAA", 0)]
    [InlineData("AAAAAAAAAAAA", 10000)]
    public void CreateWithBadArgsThrowsError(string value, int ordinal)
    {
        FluentActions.Invoking(() => AccountId.Create(value, ordinal))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("AAAAAAAAAAAA(0001)", "AAAAAAAAAAAA", 1, true)]
    [InlineData("AAAAAAAAAAAA(9999)", "AAAAAAAAAAAA", 9999, true)]
    [InlineData("AAAAAAAAAAAI(0001)", null, 0, false)]
    [InlineData("AAAAAAAAAAAAA0001)", null, 0, false)]
    [InlineData("AAAAAAAAAAAA(0001A", null, 0, false)]
    [InlineData("AAAAAAAAAAAA(AAAA)", null, 0, false)]
    public void TryParseShouldReturnExpectedValues(string value,
        string expectedTraderId, int expectedOrdinal, bool isValid)
    {
        AccountId.TryParse(value, out var traderId, out var ordinal)
            .Should().Be(isValid);

        if (isValid)
        {
            traderId.Should().Be(expectedTraderId);
            ordinal.Should().Be(expectedOrdinal);
        }
    }

    [Theory]
    [InlineData("AAAAAAAAAAAI(0001)")]
    [InlineData("AAAAAAAAAAAAA0001)")]
    [InlineData("AAAAAAAAAAAA(0001A")]
    [InlineData("AAAAAAAAAAAA(AAAA)")]
    public void ParseWithBadArg(string value)
    {
        FluentActions.Invoking(() => _ = AccountId.Parse(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("AAAAAAAAAAAA(0001)", "AAAAAAAAAAAA", 1)]
    [InlineData("AAAAAAAAAAAA(9999)", "AAAAAAAAAAAA", 9999)]
    public void ParseWithGoodArgs(
        string value, string expectedTraderId, int expectedOrdinal)
    {
        var (traderId, ordinal) = AccountId.Parse(value);

        traderId.Should().Be(expectedTraderId);
        ordinal.Should().Be(expectedOrdinal);
    }

    [Theory]
    [InlineData("AAAAAAAAAAAA(0001)", true)]
    [InlineData("AAAAAAAAAAAA(9999)", true)]
    [InlineData("AAAAAAAAAAAI(0001)", false)]
    [InlineData("AAAAAAAAAAAAA0001)", false)]
    [InlineData("AAAAAAAAAAAA(0001A", false)]
    [InlineData("AAAAAAAAAAAA(AAAA)", false)]
    public void IsValidShouldReturnExpectedValues(string value, bool isValid) =>
        AccountId.TryParse(value, out _, out _).Should().Be(isValid);
}