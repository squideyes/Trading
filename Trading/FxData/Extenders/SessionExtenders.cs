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

public static class SessionExtenders
{
    public static bool InSession(this Session session, Tick tick) =>
        session.InSession(tick.TickOn);

    public static bool InSession(this Session session, ICandle candle) =>
        session.InSession(candle.OpenOn) && session.InSession(candle.CloseOn);
}
