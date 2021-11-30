// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

//using SquidEyes.Basics;
//using SquidEyes.Trading.Context;
//using System.Collections;

//namespace SquidEyes.Trading.FxData;

//public class RenkoFeed : IEnumerable<Candle>
//{
//    private static Candle GetCandle(
//        DateTime openOn, DateTime closeOn, float open, float close)
//    {
//        if (close > open)
//            return new Candle(openOn, closeOn, open, close, open, close);
//        else
//            return new Candle(openOn, closeOn, open, open, close, close);
//    }

//    private readonly List<Candle> candles = new();

//    private DateTime? openOn = null;
//    private float anchor = 0.0f;
//    private float upTrigger;
//    private float downTrigger;

//    public event EventHandler<CandleArgs>? OnCandle;

//    public RenkoFeed(Pair pair, int brickTicks)
//    {
//        Pair = pair ?? throw new ArgumentNullException(nameof(pair));

//        BrickTicks = brickTicks.Validated(nameof(brickTicks),
//            v => v.Between(1, 100), v => pair.Round(v * pair.OneTick));
//    }

//    public Pair Pair { get; }
//    public float BrickTicks { get; }

//    public int Count => candles.Count;

//    public Candle this[int index] => candles[index];

//    public void HandleTick(Tick tick)
//    {
//        Candle candle;

//        if (anchor == 0.0f)
//        {
//            openOn = tick.TickOn;
//            anchor = tick.Mid;
//            upTrigger = Pair.Round(anchor + BrickTicks);
//            downTrigger = Pair.Round(anchor - BrickTicks);
//        }

//        if (downTrigger < tick.Mid && tick.Mid < upTrigger)
//            return;

//        float close;

//        var inTrend = Math.Abs(Pair.Round(anchor - tick.Mid)) < (BrickTicks * 2.0f);

//        if (tick.Mid <= downTrigger)
//        {
//            close = downTrigger;

//            upTrigger = Pair.Round(downTrigger + (BrickTicks * 2.0f));

//            downTrigger = Pair.Round(downTrigger - BrickTicks);

//            if (inTrend)
//            {
//                candle = GetCandle(openOn!.Value, tick.TickOn, anchor, close);

//                anchor = close;
//            }
//            else
//            {
//                candle = GetCandle(openOn!.Value,
//                    tick.TickOn, Pair.Round(close + BrickTicks), close);

//                anchor = Pair.Round(close + (BrickTicks * 1.0f));
//            }
//        }
//        else
//        {
//            close = upTrigger;

//            downTrigger = Pair.Round(upTrigger - (BrickTicks * 2.0f));

//            upTrigger = Pair.Round(upTrigger + BrickTicks);

//            if (inTrend)
//            {
//                candle = GetCandle(
//                    openOn!.Value, tick.TickOn, anchor, close);

//                anchor = close;
//            }
//            else
//            {
//                candle = GetCandle(openOn!.Value,
//                    tick.TickOn, Pair.Round(close - BrickTicks), close);

//                anchor = Pair.Round(close - (BrickTicks * 1.0f));
//            }
//        }

//        candles.Add(candle);

//        OnCandle?.Invoke(this, new CandleArgs(tick, candle));

//        openOn = tick.TickOn;
//    }

//    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

//    public IEnumerator<Candle> GetEnumerator() => candles.GetEnumerator();
//}
