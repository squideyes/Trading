using FluentAssertions;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class UsdValueOfTests
{
    [Theory]
    [InlineData(Currency.AUD, 071530)]
    [InlineData(Currency.CAD, 078604)]
    [InlineData(Currency.CHF, 108015)]
    [InlineData(Currency.EUR, 113480)]
    [InlineData(Currency.GBP, 135380)]
    [InlineData(Currency.JPY, 9)]
    [InlineData(Currency.NZD, 066380)]
    [InlineData(Currency.USD, 100000)]
    public void GetOneUnitOfUsd(Currency currency, int rate)
    {
        var usdValueOf = GetUsdValueOf();

        usdValueOf.GetOneUnitInUsd(currency).Should().Be(rate);
    }

    private static UsdValueOf GetUsdValueOf()
    {
        var usdValueOf = new UsdValueOf();

        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        void Update(Symbol symbol, int bid, int ask)
        {
            usdValueOf!.Update(new MetaTick(Known.Pairs[symbol],
                new Tick(session.MinTickOn, bid, ask)));
        }

        Update(Symbol.AUDUSD, 071510, 071530);
        Update(Symbol.USDCAD, 127180, 127220);
        Update(Symbol.USDCHF, 092550, 092580);
        Update(Symbol.USDJPY, 115690, 115710);
        Update(Symbol.EURUSD, 113460, 113480);
        Update(Symbol.GBPUSD, 135370, 135380);
        Update(Symbol.NZDUSD, 066340, 066380);

        return usdValueOf;
    }
}