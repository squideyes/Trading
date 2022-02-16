using FluentAssertions;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.Orders;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class MoneyHelperTests
{
    [Theory]
    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 113460, 2269.21f)]
    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 111460, 0.0f)]
    [InlineData(Symbol.EURUSD, Side.Buy, 114960, 113460, -1701.9f)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113480, 567.4f)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113980, 0.0f)]
    [InlineData(Symbol.EURUSD, Side.Sell, 113380, 113480, -113.47f)]
    [InlineData(Symbol.USDCAD, Side.Buy, 126180, 127200, 801.88f)]
    [InlineData(Symbol.USDCAD, Side.Buy, 126180, 126180, 0.0f)]
    [InlineData(Symbol.USDCAD, Side.Buy, 132180, 127180, -3931.43f)]
    [InlineData(Symbol.USDCAD, Side.Sell, 129220, 127220, 1572.08f)]
    [InlineData(Symbol.USDCAD, Side.Sell, 129220, 129220, 0.0f)]
    [InlineData(Symbol.USDCAD, Side.Sell, 124220, 127220, -2358.12f)]
    public void X(Symbol symbol, Side side, int entry, int exit, float expected)
    {
        var helper = new MoneyHelper(MoneyData.GetUsdValueOf(MidBidOrAsk.Mid));

        var actual = helper.GetGrossProfit(
            Known.Pairs[symbol], side, 100000, entry, exit);

        actual.Should().Be(expected);
    }
}