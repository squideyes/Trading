namespace SquidEyes.Trading.FxData;

public static class MiscExtenders
{
    public static Rate ToRate(this Tick tick, RateToUse rateToUse)
    {
        return rateToUse switch
        {
            RateToUse.Bid => tick.Bid,
            RateToUse.Ask => tick.Ask,
            RateToUse.Mid => tick.Mid,
            _=> throw new ArgumentOutOfRangeException(nameof(rateToUse))
        };
    }
}