// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Context;

public static class DateTimeExtenders
{
    public static string ToCandleOnText(this DateTime value) =>
        value.ToString("MM/dd/yyyy HH:mm:ss");

    public static string ToTickOnText(this DateTime value) =>
        value.ToString("MM/dd/yyyy HH:mm:ss.fff");

    public static string ToDateText(this DateTime value) =>
        value.ToString("MM/dd/yyyy");

    public static DateOnly ToTradeDate(
        this DateTime value, bool validate = true)
    {
        DateOnly tradeDate;

        if (value.Hour >= 17)
            tradeDate = DateOnly.FromDateTime(value.Date.AddDays(1));
        else
            tradeDate = DateOnly.FromDateTime(value.Date);

        if (validate && !Known.TradeDates.Contains(tradeDate))
            throw new ArgumentOutOfRangeException(nameof(value));

        return tradeDate;
    }

    public static bool IsTickOn(this DateTime value) => TickOn.IsTickOn(value);
}