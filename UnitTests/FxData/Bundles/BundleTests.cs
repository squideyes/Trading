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
using SquidEyes.Trading.FxData;
using System;
using System.IO;
using System.Linq;
using Xunit;
using SquidEyes.Basics;

namespace SquidEyes.UnitTests.FxData
{
    public class BundleTests
    {
        [Fact]
        public void ContructorWithGoodArgs()
        {
            var pair = Known.Pairs[Symbol.EURUSD];

            var tradeDate = new DateOnly(2020, 1, 6);

            var bundle = GetEmptyBundle();

            bundle.Add(GetTickSet(pair, tradeDate));

            bundle.Pair.Should().Be(pair);
            bundle.Year.Should().Be(2020);
            bundle.Month.Should().Be(1);
            bundle.Count.Should().Be(1);
            bundle.BlobName.Should().Be("SE/BUNDLES/EURUSD/2020/SE_EURUSD_2020_01_EST.stsb");
            bundle.FileName.Should().Be("SE_EURUSD_2020_01_EST.stsb");
            bundle.Source.Should().Be(Source.SquidEyes);
            bundle.IsComplete().Should().BeFalse();
        }

        [Theory]
        [InlineData((Source)0, Symbol.EURUSD, 2020, 1)]
        [InlineData(Source.SquidEyes, (Symbol)0, 2020, 1)]
        [InlineData(Source.SquidEyes, Symbol.EURUSD, 2000, 1)]
        [InlineData(Source.SquidEyes, Symbol.EURUSD, 2020, 0)]
        public void ConstructorWithBadArgs(Source source, Symbol symbol, int year, int month)
        {
            FluentActions.Invoking(() => _ = new Bundle(source, Known.Pairs[symbol], year, month))
                .Should().Throw<Exception>();
        }

        [Fact]
        public void IsCompleteReturnsExpectedValue()
        {
            var bundle = GetEmptyBundle();

            var tradeDates = Known.GetTradeDates(2020, 1);

            var pair = Known.Pairs[Symbol.EURUSD];

            for (var i = 0; i < tradeDates.Count - 1; i++)
            {
                bundle.Add(GetTickSet(pair, tradeDates[i]));

                bundle.IsComplete().Should().BeFalse();
            }

            bundle.Add(GetTickSet(pair, tradeDates.Last()));

            bundle.IsComplete().Should().BeTrue();
        }

        [Fact]
        public void BundleHasExpectedMetadata()
        {
            var bundle = GetEmptyBundle();

            bundle.Add(GetTickSet(bundle.Pair, new DateOnly(2020, 1, 6)));

            bundle.GetMetadata()["Source"].Should().Be("SquidEyes");
            bundle.GetMetadata()["Pair"].Should().Be("EURUSD");
            bundle.GetMetadata()["Year"].Should().Be("2020");
            bundle.GetMetadata()["Month"].Should().Be("1");
            bundle.GetMetadata()["Days"].Should().Be("6");

            bundle.Add(GetTickSet(bundle.Pair, new DateOnly(2020, 1, 7)));

            bundle.GetMetadata()["Days"].Should().Be("6,7");
        }

        [Fact]
        public void GoodFullPathGenerated()
        {
            GetEmptyBundle().GetFullPath("C:\\TickData").Should().Be(
                "C:\\TickData\\SE\\BUNDLES\\EURUSD\\2020\\SE_EURUSD_2020_01_EST.stsb");
        }

        [Fact]
        public void ToStringReturnsFileName() =>
            GetEmptyBundle().AsAction(b => b.ToString().Should().Be(b.FileName));

        [Fact]
        public void CanRoundtripThroughStream()
        {
            var pair = Known.Pairs[Symbol.EURUSD];

            var tradeDate = new DateOnly(2020, 1, 6);

            var source = GetEmptyBundle();

            source.Add(GetTickSet(pair, tradeDate));

            var stream = new MemoryStream();

            source.SaveToStream(stream);

            stream.Position = 0;

            var target = GetEmptyBundle();

            target.LoadFromStream(stream);

            source.Source.Should().Be(target.Source);
            source.Pair.Should().Be(target.Pair);
            source.Year.Should().Be(target.Year);
            source.Month.Should().Be(target.Month);
            source.Count.Should().Be(target.Count);

            for (var x = 0; x < target.Count; x++)
            {
                var tickSet1 = target[x];
                var tickSet2 = source[x];

                tickSet1.Count.Should().Be(tickSet2.Count);

                for (var y = 0; y < tickSet1.Count; y++)
                    tickSet1[y].Should().Be(tickSet2[y]);
            }
        }

        private static TickSet GetTickSet(
            Pair pair, DateOnly tradeDate, int bid = 1, int ask = 2)
        {
            var tickSet = new TickSet(Source.SquidEyes, pair, tradeDate);

            tickSet.Add(new Tick(tickSet.Session.MinTickOn, bid, ask));

            return tickSet;
        }

        private static Bundle GetEmptyBundle() =>
            new(Source.SquidEyes, Known.Pairs[Symbol.EURUSD], 2020, 1);
    }
}