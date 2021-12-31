// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using Xunit;
using FluentAssertions;
using SquidEyes.Trading.Context;

namespace SquidEyes.UnitTests.Context
{
    public class SquidEyesIdTests
    {
        [Fact]
        public void CreateReturnsValidSquidEyesId() =>
            SquidEyesId.Create().Length.Should().Be(SquidEyesId.Length);

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
        public void IsValidReturnsExpectedValue(string value, bool expected) =>
            SquidEyesId.IsValid(value).Should().Be(expected);
    }
}