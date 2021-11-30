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
using System.Linq;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class KnownTests
{
    [Fact]
    public void KnownBaselineUnchanged()
    {
        Known.MinYear.Should().Be(2014);
        Known.MaxYear.Should().Be(2028);
        Known.Pairs.Count.Should().Be(9);
        Known.Currencies.Count.Should().Be(8);
        Known.TradeDates.Count.Should().Be(3881);
        Known.ConvertWith.Count.Should().Be(9);
        Known.MinTradeDate.Should().Be(Known.TradeDates.First());
        Known.MaxTradeDate.Should().Be(Known.TradeDates.Last());
    }
}
