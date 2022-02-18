// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using System.Collections.Generic;

namespace SquidEyes.UnitTests.Context;

public static class MoneyData
{
    private static readonly Dictionary<MidOrAsk, UsdValueOf> data = new();

    static MoneyData()
    {
        data[MidOrAsk.Mid] = GetData(MidOrAsk.Mid);
        data[MidOrAsk.Ask] = GetData(MidOrAsk.Ask);
    }

    public static UsdValueOf GetUsdValueOf(MidOrAsk midOrAsk) => data[midOrAsk];

    private static UsdValueOf GetData(MidOrAsk midOrAsk)
    {
        var usdValueOf = new UsdValueOf(midOrAsk);

        var session = new Session(Extent.Day, new DateOnly(2020, 1, 6));

        void Update(Symbol symbol, int bid, int ask)
        {
            usdValueOf!.Update(new MetaTick(Known.Pairs[symbol],
                new Tick(session.MinTickOn, bid, ask)));
        }

        Update(Symbol.AUDUSD, 71510, 71530);
        Update(Symbol.EURUSD, 113460, 113480);
        Update(Symbol.GBPUSD, 135370, 135380);
        Update(Symbol.NZDUSD, 66340, 66380);
        Update(Symbol.USDCAD, 127180, 127220);
        Update(Symbol.USDCHF, 92550, 92580);
        Update(Symbol.USDJPY, 115690, 115710);

        return usdValueOf;
    }
}