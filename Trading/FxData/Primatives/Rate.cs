// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Shared.Helpers;

namespace SquidEyes.Trading.FxData;

public struct Rate : IEquatable<Rate>, IComparable<Rate>
{
    public const int MinValue = 1;
    public const int MaxValue = 999999;

    public Rate()
        : this(MinValue)
    {
    }

    public Rate(int value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public Rate(float value, int digits)
        : this((int)(value * GetFactor(digits)))
    {
    }

    internal int Value { get; }

    public bool IsEmpty => Value == default;

    public override string ToString() => Value.ToString();

    public string ToString(int digits)
    {
        return digits switch
        {
            5 => GetFloat(digits).ToString("N5"),
            3 => GetFloat(digits).ToString("N3"),
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public float GetFloat(int digits) =>
        FastMath.Round(Value / GetFactor(digits), digits);

    public bool Equals(Rate other) => Value.Equals(other.Value);

    public int CompareTo(Rate other) => Value.CompareTo(other.Value);

    public override bool Equals(object? other) =>
        other is Rate rate && Equals(rate);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool IsRate(float value, int digits)
    {
        bool IsRounded() => value.Equals(FastMath.Round(value, digits));

        return digits switch
        {
            5 => value >= 0.00001f && value <= 9.99999f && IsRounded(),
            3 => value >= 0.001f && value <= 999.999f && IsRounded(),
            _ => false
        };
    }

    private static float GetFactor(int digits)
    {
        return digits switch
        {
            5 => 100000.0f,
            3 => 1000.0f,
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public static bool operator ==(Rate left, Rate right) =>
        left.Equals(right);

    public static bool operator !=(Rate left, Rate right) =>
        !(left == right);

    public static bool operator <(Rate left, Rate right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(Rate left, Rate right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(Rate left, Rate right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(Rate left, Rate right) =>
        left.CompareTo(right) >= 0;
}