// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Context
{
    public struct TickOn : IEquatable<TickOn>, IComparable<TickOn>
    {
        public TickOn()
            : this(Known.MinTickOnValue)
        {
        }

        public TickOn(DateTime value)
        {
            if (!IsTickOn(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
        }

        internal DateTime Value { get; init; }

        public bool IsEmpty => Value == default;

        public override string ToString() => Value.ToTickOnText();

        public int CompareTo(TickOn other) =>
            Value.CompareTo(other.Value);

        public bool Equals(TickOn other) => Value.Equals(other.Value);

        public override bool Equals(object? other) =>
            other is TickOn tickOn && Equals(tickOn);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool IsTickOn(DateTime value)
        {
            if (value.Kind != DateTimeKind.Unspecified)
                return false;

            if (value < Known.MinTickOnValue || value > Known.MaxTickOnValue)
                return false;

            DateOnly tradeDate;

            if (value.Hour >= 17)
                tradeDate = DateOnly.FromDateTime(value.Date).AddDays(1);
            else
                tradeDate = DateOnly.FromDateTime(value.Date);

            return Known.TradeDates.Contains(tradeDate);
        }

        public static TickOn Parse(string value) => new(DateTime.Parse(value));

        public static implicit operator TickOn(DateTime value) =>
            new(value);

        public static explicit operator DateTime(TickOn value) =>
            value.Value;

        public static bool operator ==(TickOn left, TickOn right) =>
            left.Equals(right);

        public static bool operator !=(TickOn left, TickOn right) =>
            !(left == right);

        public static bool operator <(TickOn left, TickOn right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(TickOn left, TickOn right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >(TickOn left, TickOn right) =>
            left.CompareTo(right) > 0;

        public static bool operator >=(TickOn left, TickOn right) =>
            left.CompareTo(right) >= 0;
    }
}
