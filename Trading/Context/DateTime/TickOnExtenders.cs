// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Context;

public static class TickOnExtenders
{
    public static DateOnly ToTradeDate(this TickOn value, bool validate)
    {
        DateOnly tradeDate;

        if (value.Value.Hour >= 17)
            tradeDate = DateOnly.FromDateTime(value.Value.Date.AddDays(1));
        else
            tradeDate = DateOnly.FromDateTime(value.Value.Date);

        if (validate && !Known.TradeDates.Contains(tradeDate))
            throw new ArgumentOutOfRangeException(nameof(value));

        return tradeDate;
    }
}