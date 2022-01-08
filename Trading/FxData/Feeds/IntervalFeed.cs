// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public class IntervalFeed : ICandleFeed
{
    private DateTime? lastOpenOn = null;
    private Candle? candle = null;

    public event EventHandler<CandleArgs>? OnCandle;

    public IntervalFeed(Pair pair, Session session, int seconds = 60)
    {
        Pair = pair ?? throw new ArgumentNullException(nameof(pair));

        Session = session ?? throw new ArgumentNullException(nameof(session));

        Interval = seconds.Validated(nameof(seconds),
            v => v.Between(1, 3600), v => TimeSpan.FromSeconds(v));
    }

    public Pair Pair { get; }
    public Session Session { get; }
    public TimeSpan Interval { get; }

    public void HandleTick(Tick tick)
    {
        if (tick == default)
            throw new ArgumentNullException(nameof(tick));

        var openOn = tick.TickOn.ToOpenOn(Session, Interval);

        if (candle == null)
        {
            candle = new Candle(Session, tick, Interval);
        }
        else if (lastOpenOn < openOn)
        {
            OnCandle?.Invoke(this, new CandleArgs(tick, candle));

            candle = new Candle(Session, tick, Interval);
        }
        else
        {
            candle.Adjust(tick, Interval);
        }

        lastOpenOn = openOn;
    }

    public void RaiseOnCandleAndReset(Tick tick)
    {
        if (tick == default)
            throw new ArgumentOutOfRangeException(nameof(tick));

        OnCandle?.Invoke(this, new CandleArgs(tick, candle!));

        candle = null;
    }
}