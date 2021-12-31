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
    public static class TradeDateExtenders
    {
        private static TimeOnly minTickOnTime = new(17, 0);
        private static TimeOnly maxTickOnTime = new(16, 59, 59, 999);

        public static DateTime ToMinTickOnValue(this DateOnly value, bool validate)
        {
            if (validate && !value.IsTradeDate())
                throw new ArgumentOutOfRangeException(nameof(value));

            return value.ToDateTime(minTickOnTime).AddDays(-1);
        }

        public static DateTime ToMaxTickOnValue(this DateOnly value, bool validate)
        {
            if (validate && !value.IsTradeDate())
                throw new ArgumentOutOfRangeException(nameof(value));

            return value.ToDateTime(maxTickOnTime);
        }

        public static bool IsTradeDate(this DateOnly value) =>
            Known.TradeDates.Contains(value);

        public static DateOnly ToNextTradeDate(this DateOnly value) =>
            ToThisOrNextTradeDate(value.AddDays(1));

        public static DateOnly ToThisOrNextTradeDate(this DateOnly value)
        {
            if (value < Known.TradeDates.First())
                throw new ArgumentOutOfRangeException(nameof(value));

            var maxDate = Known.TradeDates.Last();

            while (value <= maxDate && !value.IsTradeDate())
                value = value.AddDays(1);

            if (value > maxDate)
                throw new ArgumentOutOfRangeException(nameof(value));

            return value;
        }

        public static DateOnly ToPrevTradeDate(this DateOnly value) =>
            ToThisOrPrevTradeDate(value.AddDays(-1));

        public static DateOnly ToThisOrPrevTradeDate(this DateOnly value)
        {
            if (value > Known.TradeDates.Last())
                throw new ArgumentOutOfRangeException(nameof(value));

            var minDate = Known.TradeDates.First();

            while (value >= minDate && !value.IsTradeDate())
                value = value.AddDays(-1);

            if (value < minDate)
                throw new ArgumentOutOfRangeException(nameof(value));

            return value;
        }
    }
}