// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.Trading.Context
{
    public class Clock : IEquatable<Clock>
    {
        public Clock(Unit unit, int quantity)
        {
            int GetMaxQuantity()
            {
                return unit switch
                {
                    Unit.Hours => 4,
                    Unit.Minutes => 59,
                    Unit.Seconds => 59,
                    _ => throw new ArgumentOutOfRangeException(nameof(unit))
                };
            }

            Unit = unit.Validated(nameof(unit), v => v.IsEnumValue());

            Quantity = quantity.Validated(
                nameof(quantity), v => v.Between(1, GetMaxQuantity()));
        }

        public Unit Unit { get; }
        public int Quantity { get; }

        public override string ToString() => $"{Unit.ToString()[0]}{Quantity:00}";

        public static Clock Parse(string value)
        {
            if (!value.IsNonEmptyAndTrimmed())
                throw new ArgumentOutOfRangeException(nameof(value));

            var unit = value[0] switch
            {
                'H' => Unit.Hours,
                'M' => Unit.Minutes,
                'S' => Unit.Seconds,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };

            return new Clock(unit, int.Parse(value[1..]));
        }

        public bool Equals(Clock? other) =>
            Unit.Equals(other?.Unit) && Quantity.Equals(other?.Quantity);

        public override bool Equals(object? other) => Equals(other as Clock);

        public override int GetHashCode() => HashCode.Combine(Unit, Quantity);

        public static implicit operator Clock(string value) => Parse(value);

        public static implicit operator string(Clock value) => value.ToString();

        public static bool operator ==(Clock left, Clock right) => left.Equals(right);

        public static bool operator !=(Clock left, Clock right) => !(left == right);
    }
}