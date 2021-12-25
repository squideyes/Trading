using FluentAssertions;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using System.Linq;
using Xunit;

namespace SquidEyes.UnitTests.FxData
{
    public class BundleTests
    {
        [Fact]
        public void ContructorWithGoodArgs()
        {
            var pair = Known.Pairs[Symbol.EURUSD];

            var tradeDate = new DateOnly(2020, 1, 6);

            var tickSet = new TickSet(Source.SquidEyes, pair, tradeDate);

            var bundle = GetEmptyBundle();

            bundle.Add(tickSet);

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
                var tickSet = new TickSet(Source.SquidEyes, pair, tradeDates[i]);

                bundle.Add(tickSet);

                bundle.IsComplete().Should().BeFalse();
            }

            bundle.Add(new TickSet(Source.SquidEyes, pair, tradeDates.Last()));

            bundle.IsComplete().Should().BeTrue();
        }

        [Fact]
        public void BundleHasExpectedMetadata()
        {
            var bundle = GetEmptyBundle();

            bundle.Add(new TickSet(Source.SquidEyes, 
                Known.Pairs[Symbol.EURUSD], new DateOnly(2020, 1, 6)));

            bundle.GetMetadata()["Source"].Should().Be("SquidEyes");
            bundle.GetMetadata()["Pair"].Should().Be("EURUSD");
            bundle.GetMetadata()["Year"].Should().Be("2020");
            bundle.GetMetadata()["Month"].Should().Be("1");
            bundle.GetMetadata()["Days"].Should().Be("6");

            bundle.Add(new TickSet(Source.SquidEyes,
                Known.Pairs[Symbol.EURUSD], new DateOnly(2020, 1, 7)));

            bundle.GetMetadata()["Days"].Should().Be("6,7");
        }

        [Fact]
        public void GoodFullPathGenerated()
        {
            var bundle = GetEmptyBundle();

            bundle.GetFullPath("C:\\TickData").Should().Be(
                "C:\\TickData\\SE\\BUNDLES\\EURUSD\\2020\\SE_EURUSD_2020_01_EST.stsb");
        }

        [Fact]
        public void ToStringReturnsFileName()
        {
            var bundle = GetEmptyBundle();

            bundle.ToString().Should().Be(bundle.FileName);
        }

        private static Bundle GetEmptyBundle() =>
            new(Source.SquidEyes, Known.Pairs[Symbol.EURUSD], 2020, 1);
    }
}
