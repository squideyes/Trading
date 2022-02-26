using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public class WickoFeed
{
    public class WickoArgs : EventArgs
    {
        public WickoArgs(Wicko wicko)
        {
            Wicko = wicko;
        }

        public Wicko Wicko { get; }
    }

    private readonly Rate brickSize;
    private readonly MidOrAsk midOrAsk;

    private bool firstTick = true;
    private Wicko lastWicko = null!;

    private TickOn openOn;
    private TickOn closeOn;
    private Rate open;
    private Rate high;
    private Rate low;
    private Rate close;

    public event EventHandler<WickoArgs>? OnWicko;

    public WickoFeed(Session session, Rate pips, MidOrAsk midOrAsk)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));

        brickSize = pips.Value * 10;
        this.midOrAsk = midOrAsk;
    }

    public Session Session { get; }

    private Wicko GetNewWicko(Rate open, Rate high, Rate low, Rate close)
    {
        return new Wicko()
        {
            OpenOn = openOn,
            CloseOn = closeOn,
            Open = open,
            High = high,
            Low = low,
            Close = close
        };
    }

    private void Rising()
    {
        Rate limit;

        while (close > (limit = open + brickSize))
        {
            var wicko = GetNewWicko(open, limit, low, limit);

            lastWicko = wicko;

            OnWicko?.Invoke(this, new WickoArgs(wicko));

            openOn = closeOn;
            open = low = limit;
        }
    }

    private void Falling()
    {
        Rate limit;

        while (close < (limit = open - brickSize))
        {
            var wicko = GetNewWicko(open, high, limit, limit);

            lastWicko = wicko;

            OnWicko?.Invoke(this, new WickoArgs(wicko));

            openOn = closeOn;
            open = high = limit;
        }
    }

    internal Wicko OpenWicko => GetNewWicko(open, high, low, close);

    public void HandleTick(Tick tick)
    {
        if (!Session.InSession(tick.TickOn))
        {
            throw new InvalidOperationException(
                $"{tick.TickOn} is not within the \"{Session}\" session");
        }

        var rate = tick.ToRate(midOrAsk);

        if (firstTick)
        {
            firstTick = false;

            openOn = tick.TickOn;
            closeOn = tick.TickOn;
            open = high = low = close = rate;
        }
        else
        {
            closeOn = tick.TickOn;

            if (rate > high)
                high = rate;

            if (rate < low)
                low = rate;

            close = rate;

            if (close > open)
            {
                if (lastWicko == null || (lastWicko.Trend == Trend.Rising))
                {
                    Rising();

                    return;
                }

                var limit = lastWicko.Open + brickSize;

                if (close > limit)
                {
                    var wicko = GetNewWicko(
                        lastWicko.Open, limit, low, limit);

                    lastWicko = wicko;

                    OnWicko?.Invoke(this, new WickoArgs(wicko));

                    openOn = closeOn;
                    open = low = limit;

                    Rising();
                }
            }
            else if (close < open)
            {
                if (lastWicko == null || (lastWicko.Trend == Trend.Falling))
                {
                    Falling();

                    return;
                }

                var limit = lastWicko.Open - brickSize;

                if (close < limit)
                {
                    var wicko = GetNewWicko(lastWicko.Open, high, limit, limit);

                    lastWicko = wicko;

                    OnWicko?.Invoke(this, new WickoArgs(wicko));

                    openOn = closeOn;
                    open = high = limit;

                    Falling();
                }
            }
        }
    }
}