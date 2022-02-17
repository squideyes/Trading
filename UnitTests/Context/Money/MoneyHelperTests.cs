using FluentAssertions;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.Orders;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class MoneyHelperTests
{
    [Theory]
    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 113460, 2269.21)]
    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 111460, 0.0)]
    [InlineData(Symbol.EURUSD, Side.Buy, 114960, 113460, -1701.9)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113480, 567.4)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113980, 0.0)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113380, 113480, -113.47)]
    [InlineData(Symbol.USDCAD, Side.Buy, 126180, 127200, 801.88)]
    [InlineData(Symbol.USDCAD, Side.Buy, 126180, 126180, 0.0)]
    [InlineData(Symbol.USDCAD, Side.Buy, 132180, 127180, -3931.43)]
    [InlineData(Symbol.USDCAD, Side.Sell, 129220, 127220, 1572.08)]
    [InlineData(Symbol.USDCAD, Side.Sell, 129220, 129220, 0.0)]
    [InlineData(Symbol.USDCAD, Side.Sell, 124220, 127220, -2358.12)]
    [InlineData(Symbol.EURJPY, Side.Buy, 105690, 115690, 9808.11)]
    [InlineData(Symbol.EURJPY, Side.Buy, 105690, 105690, 0.0)]
    [InlineData(Symbol.EURJPY, Side.Buy, 130690, 115690, -14712.16)]
    [InlineData(Symbol.EURJPY, Side.Sell, 127690, 115700, 11758.91)]
    [InlineData(Symbol.EURJPY, Side.Sell, 127690, 127690, 0.0)]
    [InlineData(Symbol.EURJPY, Side.Sell, 95690, 115700, -19624.32)]
    public void GetGrossProfitReturnsExpectedValue(
        Symbol symbol, Side side, int entry, int exit, double expected)
    {
        var helper = new MoneyHelper(
            MoneyData.GetUsdValueOf(MidBidOrAsk.Mid));

        var actual = helper.GetGrossProfit(
            Known.Pairs[symbol], side, 100000, entry, exit);

        actual.Should().Be(expected);
    }
}