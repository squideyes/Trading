// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.Trading.Context;

public class Pair : IEquatable<Pair>
{
    private readonly string format;

    public Pair(Symbol symbol, int digits)
    {
        Symbol = symbol.Validated(nameof(symbol), v => v.IsEnumValue());
        Digits = digits.Validated(nameof(digits), v => v.In(3, 5));

        format = "N" + digits;

        Factor = (int)MathF.Pow(10, digits);
        OnePip = MathF.Pow(10, -(digits - 1));
        OneTick = MathF.Pow(10, -digits);
        MaxValue = Round((OneTick * 1000000.0f) - OneTick);
        Base = symbol.ToString()[0..3].ToEnumValue<Currency>();
        Quote = symbol.ToString()[3..].ToEnumValue<Currency>();
    }

    public Symbol Symbol { get; }
    public int Digits { get; }
    public float Factor { get; }
    public Currency Base { get; }
    public Currency Quote { get; }
    public float OnePip { get; }
    public float OneTick { get; }
    public float MaxValue { get; }

    public float MinValue => OneTick;

    public bool IsPrice(float value)
    {
        if (value != Round(value))
            return false;

        return value >= OneTick && value <= MaxValue;
    }

    public override string ToString() => Symbol.ToString();

    public string Format(float value) => value.ToString(format);

    public float Round(float value) => MathF.Round(value, Digits);

    public bool Equals(Pair? other)
    {
        if (Equals(other, null))
            return false;

        return Symbol.Equals(other.Symbol);
    }

    public override bool Equals(object? other) => Equals(other as Pair);

    public override int GetHashCode() => Symbol.GetHashCode();

    public static bool operator ==(Pair left, Pair right) => left.Equals(right);

    public static bool operator !=(Pair left, Pair right) => !(left == right);
}