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
    private const int MIN_PERIOD = 2;

    public BasicIndicatorBase(int period, Pair pair, RateToUse rateToUse)
    {
        Period = period.Validated(nameof(period), v => v >= MIN_PERIOD);
        Pair = pair ?? throw new ArgumentNullException(nameof(pair));
        RateToUse = rateToUse.Validated(nameof(rateToUse), v => v.IsEnumValue());
    }

    protected int Period { get; }
    protected Pair Pair { get; }
    protected RateToUse RateToUse { get; }

    static protected BasicResult GetBasicResult(TickOn openOn, double value)
    {
        return new BasicResult()
        {
            OpenOn = openOn,
            Value = value
        };
    }
}