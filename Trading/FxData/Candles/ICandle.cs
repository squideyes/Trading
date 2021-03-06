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

public interface ICandle
{
    TickOn OpenOn { get; }
    TickOn CloseOn { get; }
    Rate Open { get; }
    Rate High { get; }
    Rate Low { get; }
    Rate Close { get; }
}