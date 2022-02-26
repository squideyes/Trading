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

    private readonly Rate size;
    private readonly RateToUse rateToUse;

    private bool firstTick = true;
    private Wicko lastWicko = null!;

    private DateTime openOn;
    private DateTime closeOn;
    private Rate open;
    private Rate high;
    private Rate low;
    private Rate close;

    public event EventHandler<WickoArgs>? OnWicko;

    public WickoFeed(Rate pips, RateToUse rateToUse)
    {
        size = pips.Value * 10;
        this.rateToUse = rateToUse;
    }

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

        while (close > (limit = open.Value + size.Value))
        {
            var wicko = GetNewWicko(open, limit, low, limit);

            lastWicko = wicko;

            OnWicko?.Invoke(this, new WickoArgs(wicko));

            openOn = closeOn;
            open = limit;
            low = limit;
        }
    }

    private void Falling()
    {
        Rate limit;

        while (close < (limit = open.Value - size.Value))
        {
            var wicko = GetNewWicko(open, high, limit, limit);

            lastWicko = wicko;

            OnWicko?.Invoke(this, new WickoArgs(wicko));

            openOn = closeOn;
            open = limit;
            high = limit;
        }
    }

    internal Wicko OpenWicko => GetNewWicko(open, high, low, close);

    public void HandleTick(Tick tick)
    {
        var rate = tick.ToRate(rateToUse);

        if (firstTick)
        {
            firstTick = false;

            openOn = tick.TickOn.Value;
            closeOn = tick.TickOn.Value;
            open = rate;
            high = rate;
            low = rate;
            close = rate;
        }
        else
        {
            closeOn = tick.TickOn.Value;

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

                Rate limit = lastWicko.Open.Value + size.Value;

                if (close > limit)
                {
                    var wicko = GetNewWicko(
                        lastWicko.Open, limit, low, limit);

                    lastWicko = wicko;

                    OnWicko?.Invoke(this, new WickoArgs(wicko));

                    openOn = closeOn;
                    open = limit;
                    low = limit;

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

                Rate limit = lastWicko.Open.Value - size.Value;

                if (close < limit)
                {
                    var wicko = GetNewWicko(lastWicko.Open, high, limit, limit);

                    lastWicko = wicko;

                    OnWicko?.Invoke(this, new WickoArgs(wicko));

                    openOn = closeOn;
                    open = limit;
                    high = limit;

                    Falling();
                }
            }
        }
    }
}