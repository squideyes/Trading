using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public static class MiscExtenders
{
    public static Rate ToRate(this Tick tick, MidOrAsk midOrAsk)
    {
        return midOrAsk switch
        {
            MidOrAsk.Mid => tick.Mid,
            MidOrAsk.Ask => tick.Ask,
            _=> throw new ArgumentOutOfRangeException(nameof(midOrAsk))
        };
    }
}