// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.FxData;

public class CandleArgs : EventArgs
{
    internal CandleArgs(Tick tick, ICandle candle)
    {
        Tick = tick;
        Candle = candle;
    }

    public Tick Tick { get; }
    public ICandle Candle { get; }
}