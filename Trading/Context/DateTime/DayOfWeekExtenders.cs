// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using static System.DayOfWeek;


namespace SquidEyes.Trading.Context
{
    public static class DayOfWeekExtenders
    {
        public static bool IsWeekday(this DayOfWeek dayOfWeek) =>
            dayOfWeek >= Monday && dayOfWeek <= Friday;
    }
}