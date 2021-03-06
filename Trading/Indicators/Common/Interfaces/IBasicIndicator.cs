// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.FxData;

namespace SquidEyes.Trading.Indicators;

public interface IBasicIndicator
{
    bool IsPrimed { get; }

    BasicResult AddAndCalc(ICandle candle);
}