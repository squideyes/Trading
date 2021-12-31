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
    public static class TimeSpanExtenders
    {
        public static string ToTimeSpanText(
            this TimeSpan value, bool daysOptional = true)
        {
            if (daysOptional && value < TimeSpan.FromDays(1))
                return value.ToString("hh\\:mm\\:ss\\.fff");
            else
                return value.ToString("d\\.hh\\:mm\\:ss\\.fff");
        }
    }
}