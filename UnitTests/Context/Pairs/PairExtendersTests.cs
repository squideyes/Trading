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
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static SquidEyes.Trading.Context.Symbol;

namespace SquidEyes.UnitTests.Context;

public class PairExtendersTests
{
    [Fact]
    public void WithExtraPairsFullCoverage()
    {
        var pairs = new List<Pair>
        {
            Known.Pairs[AUDUSD],
            Known.Pairs[EURGBP],
            Known.Pairs[EURJPY],
            Known.Pairs[USDJPY]
        };

        var dict = pairs.ToDictionary(p => p.Symbol)!;

        var extra = dict.Values.WithExtraPairs();

        extra.Count.Should().Be(6);

        extra[Known.Pairs[AUDUSD]].Should().Be(false);
        extra[Known.Pairs[EURGBP]].Should().Be(false);
        extra[Known.Pairs[EURJPY]].Should().Be(false);
        extra[Known.Pairs[USDJPY]].Should().Be(false);
        extra[Known.Pairs[EURUSD]].Should().Be(true);
        extra[Known.Pairs[GBPUSD]].Should().Be(true);
    }
}
