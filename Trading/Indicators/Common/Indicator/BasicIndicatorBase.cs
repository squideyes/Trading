// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.Indicators;

public abstract class BasicIndicatorBase
{
    public BasicIndicatorBase(
        int period, Pair pair, RateToUse rateToUse, int minPeriod)
    {
        Period = period.Validated(nameof(period), v => v >= minPeriod);
        Pair = pair ?? throw new ArgumentNullException(nameof(pair));
        RateToUse = rateToUse.Validated(nameof(rateToUse), v => v.IsEnumValue());
    }

    protected int Period { get; }
    protected Pair Pair { get; }
    protected RateToUse RateToUse { get; }

    protected static BasicResult GetBasicResult(TickOn openOn, double value) => new()
    {
        OpenOn = openOn,
        Value = value
    };
}