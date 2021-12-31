// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using Xunit;
using SquidEyes.Trading.Context;
using FluentAssertions;

namespace SquidEyes.UnitTests.Context
{
    public class MiscExtendersTests
    {
        [Theory]
        [InlineData("RCLTKJ4BQWHE", true)]
        [InlineData("PF9ZN8AG3V2U", true)]
        [InlineData("DYS675XMAAAA", true)]
        [InlineData("AAAAAAAAAAA1", false)]
        [InlineData("AAAAAAAAAAAI", false)]
        [InlineData("AAAAAAAAAAA0", false)]
        [InlineData("AAAAAAAAAAAO", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("AAAAAAAAAAA", false)]
        [InlineData("AAAAAAAAAAAAA", false)]
        public void IsSquidEyesIdReturnsExpectedValue(string value, bool expected) =>
            value.IsSquidEyesId().Should().Be(expected);

        [Theory]
        [InlineData("AAAAAAAAAAAA(0001)", true)]
        [InlineData("AAAAAAAAAAAA(9999)", true)]
        [InlineData("AAAAAAAAAAAI(0001)", false)]
        [InlineData("AAAAAAAAAAAAA0001)", false)]
        [InlineData("AAAAAAAAAAAA(0001A", false)]
        [InlineData("AAAAAAAAAAAA(AAAA)", false)]
        public void IsAccountIdShouldReturnExpectedValues(string value, bool isValid) =>
            value.IsAccountId().Should().Be(isValid);
    }
}