// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using System.Text;
using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public class Candle : IEquatable<Candle>, ICandle
{
    internal Candle(Session session, Tick tick, TimeSpan? interval = null)
    {
        if (interval.HasValue)
        {
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
            if (!session.InSession(openOn))
                throw new ArgumentOutOfRangeException(nameof(session));

            if (!session.InSession(closeOn))
                throw new ArgumentOutOfRangeException(nameof(session));

            if (closeOn <= openOn)
                throw new ArgumentOutOfRangeException(nameof(session));

            if (open == default)
                throw new ArgumentOutOfRangeException(nameof(open));

            if (high == default)
                throw new ArgumentOutOfRangeException(nameof(high));

            if (low == default)
                throw new ArgumentOutOfRangeException(nameof(low));

            if (close == default)
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
        if (!interval.HasValue)
            CloseOn = tick.TickOn;

        var mid = tick.Mid;

        if (mid > High)
            High = mid;
        else if (mid < Low)
            Low = mid;

        Close = mid;
    }

    public string ToCsvString() => ToString();

    public string ToCsvString(Pair pair) => ToString(pair);

    public override string ToString() =>
        $"{OpenOn},{CloseOn},{Open},{High},{Low},{Close}";

    public string ToString(Pair pair)
    {
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
        return other! != null!
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