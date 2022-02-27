// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public static class MiscExtenders
{
    public static Rate ToRate(this Tick tick, MidOrAsk midOrAsk)
    {
        return midOrAsk switch
        {
            MidOrAsk.Mid => tick.Mid,
            MidOrAsk.Ask => tick.Ask,
            _=> throw new ArgumentOutOfRangeException(nameof(midOrAsk))
        };
    }
}