// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using static SquidEyes.Trading.Context.Extent;

namespace SquidEyes.Trading.Context
{
    public class Session : IEquatable<Session>
    {
        public static class Defaults
        {
            public const int FromOpen = 15;
            public const int FromClose = 30;
        }

        public Session(Extent extent, DateOnly tradeDate,
            (int FromOpen, int FromClose) endCaps = default)
        {
            Extent = extent.Validated(nameof(extent), v => v.IsEnumValue());

            TradeDate = tradeDate.Validated(nameof(tradeDate),
                v => v.IsTradeDate() && extent.IsValidDayOfWeekForExtent(v));

            MinTickOn = tradeDate.AsFunc(d => new DateTime(d.Year, d.Month,
                d.Day, 17, 0, 0, DateTimeKind.Unspecified)).AddDays(-1);

            MaxTickOn = new TickOn(MinTickOn.Value.AddDays(
                extent == Day ? 0 : 4).AddMinutes(1440).AddMilliseconds(-1));

            if (endCaps == default)
                endCaps = (Defaults.FromOpen, Defaults.FromClose);

            if (!endCaps.FromOpen.Between(0, 60))
                throw new ArgumentOutOfRangeException(nameof(endCaps));

            if (!endCaps.FromClose.Between(5, 60))
                throw new ArgumentOutOfRangeException(nameof(endCaps));

            EndCaps = endCaps;
        }

        public Extent Extent { get; }
        public DateOnly TradeDate { get; }
        public TickOn MinTickOn { get; }
        public TickOn MaxTickOn { get; }
        public (int FromOpen, int FromClose) EndCaps { get; }

        public bool InSession(TickOn tickOn) => tickOn != default
            && tickOn >= MinTickOn && tickOn <= MaxTickOn;

        public bool MustGoFlat(TickOn tickOn)
        {
            TickOn minGoFlatTickOn = MaxTickOn.Value
                .AddMinutes(-EndCaps.FromClose).AddMilliseconds(1);

            return tickOn >= minGoFlatTickOn;
        }

        public bool CanTrade(TickOn tickOn) => !MustGoFlat(tickOn)
            && tickOn >= MinTickOn.Value.AddMinutes(EndCaps.FromOpen);

        public override string ToString() =>
            $"{TradeDate.ToDateText()} ({Extent}, EndCaps: {EndCaps})";

        public bool Equals(Session? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Extent == other.Extent
                && TradeDate == other.TradeDate;
        }

        public override bool Equals(object? other) => Equals(other as Session);

        public override int GetHashCode() => HashCode.Combine(Extent, TradeDate);

        public static bool operator ==(Session? a, Session? b)
        {
            if (Equals(a, null))
            {
                if (Equals(b, null))
                    return true;

                return false;
            }
            else if (Equals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Session? a, Session? b) => !(a == b);
    }
}