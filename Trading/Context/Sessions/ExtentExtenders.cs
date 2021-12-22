// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using static SquidEyes.Trading.Context.Extent;
using static System.DayOfWeek;

namespace SquidEyes.Trading.Context;

public static class ExtentExtenders
{
    public static int ToDays(this Extent extent)
    {
        return extent switch
        {
            Day => 1,
            Week => 5,
            _ => throw new ArgumentOutOfRangeException(nameof(extent))
        };
    }

    public static bool IsValidDayOfWeekForExtent(
        this Extent extent, DateOnly date)
    {
        return (extent, date.DayOfWeek) switch
        {
            (Day, Monday) => true,
            (Day, Tuesday) => true,
            (Day, Wednesday) => true,
            (Day, Thursday) => true,
            (Day, Friday) => true,
            (Week, Monday) => true,
            _ => false
        };
    }
}
