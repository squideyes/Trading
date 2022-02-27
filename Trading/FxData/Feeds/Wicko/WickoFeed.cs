// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public class WickoFeed
{
    private readonly Rate brickSize;
    private readonly MidOrAsk midOrAsk;

    private bool firstTick = true;
    private Candle lastCandle = null!;

    private TickOn openOn;
    private TickOn closeOn;
    private Rate open;
    private Rate high;
    private Rate low;
    private Rate close;

    public event EventHandler<CandleArgs>? OnCandle;

    public WickoFeed(Session session, Rate pips, MidOrAsk midOrAsk)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));

        brickSize = pips.Value * 10;
        this.midOrAsk = midOrAsk;
    }

    public Session Session { get; }

    private Candle GetCandle(Rate open, Rate high, Rate low, Rate close) =>
        new(openOn, closeOn, open, high, low, close);

    private void Rising(Tick tick)
    {
        Rate limit;

        while (close > (limit = open + brickSize))
        {
            var candle = GetCandle(open, limit, low, limit);

            lastCandle = candle;

            OnCandle?.Invoke(this, new CandleArgs(tick, candle));

            openOn = closeOn;
            open = low = limit;
        }
    }

    private void Falling(Tick tick)
    {
        Rate limit;

        while (close < (limit = open - brickSize))
        {
            var candle = GetCandle(open, high, limit, limit);

            lastCandle = candle;

            OnCandle?.Invoke(this, new CandleArgs(tick, candle));

            openOn = closeOn;
            open = high = limit;
        }
    }

    internal Candle OpenCandle => GetCandle(open, high, low, close);

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
                if (lastCandle == null! || (lastCandle.Trend == Trend.Rising))
                {
                    Rising(tick);

                    return;
                }

                var limit = lastCandle!.Open + brickSize;

                if (close > limit)
                {
                    var candle = GetCandle(lastCandle.Open, limit, low, limit);

                    lastCandle = candle;

                    OnCandle?.Invoke(this, new CandleArgs(tick, candle));

                    openOn = closeOn;
                    open = low = limit;

                    Rising(tick);
                }
            }
            else if (close < open)
            {
                if (lastCandle == null! || (lastCandle.Trend == Trend.Falling))
                {
                    Falling(tick);

                    return;
                }

                var limit = lastCandle!.Open - brickSize;

                if (close < limit)
                {
                    var candle = GetCandle(lastCandle.Open, high, limit, limit);

                    lastCandle = candle;

                    OnCandle?.Invoke(this, new CandleArgs(tick, candle));

                    openOn = closeOn;
                    open = high = limit;

                    Falling(tick);
                }
            }
        }
    }
}