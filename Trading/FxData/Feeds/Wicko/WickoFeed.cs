// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;
using SquidEyes.Basics;

namespace SquidEyes.Trading.FxData;

public class WickoFeed
{
    private readonly Rate brickTicks;
    private readonly MidOrAsk midOrAsk;

    private bool firstTick = true;
    private Candle lastCandle = null!;
    private int candleSetId = -1;

    private TickOn openOn;
    private TickOn closeOn;
    private Rate open;
    private Rate high;
    private Rate low;
    private Rate close;

    public event EventHandler<CandleArgs>? OnCandle;

    public WickoFeed(Session session, Rate brickTicks, MidOrAsk midOrAsk)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));

        if (brickTicks < 5)
            throw new ArgumentOutOfRangeException(nameof(brickTicks));

        if (!midOrAsk.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(midOrAsk));

        this.brickTicks = brickTicks;
        this.midOrAsk = midOrAsk;
    }

    public Session Session { get; }

    private Candle GetCandle(Rate open, Rate high, Rate low, Rate close) =>
        new(openOn, closeOn, open, high, low, close);

    private void Rising(Tick tick)
    {
        Rate limit;

        var firstTime = true;

        while (close > (limit = open + brickTicks))
        {
            if (firstTime)
            {
                candleSetId++;

                firstTime = false;
            }

            var candle = GetCandle(open, limit, low, limit);

            lastCandle = candle;

            RaiseCandle(candleSetId, tick, candle);

            openOn = closeOn;
            open = low = limit;
        }
    }

    private void Falling(Tick tick)
    {
        Rate limit;

        var firstTime = true;

        while (close < (limit = open - brickTicks))
        {
            if (firstTime)
            {
                candleSetId++;

                firstTime = false;
            }

            var candle = GetCandle(open, high, limit, limit);

            lastCandle = candle;

            RaiseCandle(candleSetId, tick, candle);

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

                var limit = lastCandle!.Open + brickTicks;

                if (close > limit)
                {
                    var candle = GetCandle(lastCandle.Open, limit, low, limit);

                    lastCandle = candle;

                    RaiseCandle(++candleSetId, tick, candle);

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

                var limit = lastCandle!.Open - brickTicks;

                if (close < limit)
                {
                    var candle = GetCandle(lastCandle.Open, high, limit, limit);

                    lastCandle = candle;

                    RaiseCandle(++candleSetId, tick, candle);

                    openOn = closeOn;
                    open = high = limit;

                    Falling(tick);
                }
            }
        }
    }

    private void RaiseCandle(int candleSetId, Tick tick, Candle candle) =>
        OnCandle?.Invoke(this, new CandleArgs(candleSetId, tick, candle));
}