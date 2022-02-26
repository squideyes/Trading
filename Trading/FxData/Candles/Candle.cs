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
using System.Text;

namespace SquidEyes.Trading.FxData;

public class Candle : IEquatable<Candle>, ICandle
{
    internal Candle(Session session, Tick tick, TimeSpan? interval = null)
    {
        if (interval.HasValue)
        {
            if (interval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(interval));

            OpenOn = tick.TickOn.ToOpenOn(session, interval.Value);

            var closeOn = OpenOn.Value.Add(interval.Value).AddMilliseconds(-1);

            if (closeOn <= session.MaxTickOn.Value)
                CloseOn = closeOn;
            else
                CloseOn = session.MaxTickOn;
        }
        else
        {
            OpenOn = CloseOn = tick.TickOn;
        }

        Open = High = Low = Close = tick.Mid;
    }

    public Candle(Session session, TickOn openOn, TickOn closeOn,
        Rate open, Rate high, Rate low, Rate close, bool validate = true)
    {
        if (validate)
        {
            ArgumentNullException.ThrowIfNull(session);

            if (!session.InSession(openOn))
                throw new ArgumentOutOfRangeException(nameof(session));

            if (!session.InSession(closeOn))
                throw new ArgumentOutOfRangeException(nameof(session));

            //if (closeOn <= openOn)
            //    throw new ArgumentOutOfRangeException(nameof(session));

            if (open.IsDefaultValue())
                throw new ArgumentOutOfRangeException(nameof(open));

            if (high.IsDefaultValue())
                throw new ArgumentOutOfRangeException(nameof(high));

            if (low.IsDefaultValue())
                throw new ArgumentOutOfRangeException(nameof(low));

            if (close.IsDefaultValue())
                throw new ArgumentOutOfRangeException(nameof(close));

            if (high < low)
                throw new ArgumentOutOfRangeException(nameof(high));

            if (open < low)
                throw new ArgumentOutOfRangeException(nameof(open));

            if (open > high)
                throw new ArgumentOutOfRangeException(nameof(open));

            if (close < low)
                throw new ArgumentOutOfRangeException(nameof(close));

            if (close > high)
                throw new ArgumentOutOfRangeException(nameof(close));
        }

        OpenOn = openOn;
        CloseOn = closeOn;
        Open = open;
        High = high;
        Low = low;
        Close = close;
    }

    public TickOn OpenOn { get; }
    public Rate Open { get; }

    public TickOn CloseOn { get; private set; }
    public Rate High { get; private set; }
    public Rate Low { get; private set; }
    public Rate Close { get; private set; }

    internal void Adjust(Tick tick, TimeSpan? interval = null)
    {
        if (tick.IsDefaultValue())
            throw new ArgumentOutOfRangeException(nameof(tick));

        if (interval == null)
            CloseOn = tick.TickOn;

        var rate = tick.Mid;

        if (rate > High)
            High = rate;
        else if (rate < Low)
            Low = rate;

        Close = rate;
    }

    public string ToCsvString() => ToString();

    public string ToCsvString(Pair pair) => ToString(pair);

    public override string ToString() =>
        $"{OpenOn},{CloseOn},{Open},{High},{Low},{Close}";

    public string ToString(Pair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var sb = new StringBuilder();

        sb.Append(OpenOn);
        sb.AppendDelimited(CloseOn);
        sb.AppendDelimited(Open.ToString(pair.Digits));
        sb.AppendDelimited(High.ToString(pair.Digits));
        sb.AppendDelimited(Low.ToString(pair.Digits));
        sb.AppendDelimited(Close.ToString(pair.Digits));

        return sb.ToString();
    }

    public bool Equals(Candle? other)
    {
        return !Equals(other!, null!)
            && OpenOn.Equals(other.OpenOn)
            && CloseOn.Equals(other.CloseOn)
            && Open.Equals(other.Open)
            && High.Equals(other.High)
            && Low.Equals(other.Low)
            && Close.Equals(other.Close);
    }

    public override bool Equals(object? other) => Equals(other as Candle);

    public override int GetHashCode() =>
        HashCode.Combine(OpenOn, CloseOn, Open, High, Low, Close);

    public Candle Clone() => (Candle)MemberwiseClone();

    public static bool operator ==(Candle left, Candle right) => left.Equals(right);

    public static bool operator !=(Candle left, Candle right) => !(left == right);
}