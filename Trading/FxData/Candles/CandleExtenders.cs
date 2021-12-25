using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData
{
    public static class CandleExtenders
    {
        public static bool InSession(this ICandle candle, Session session) =>
            session.InSession(candle.OpenOn) && session.InSession(candle.CloseOn);
    }
}
