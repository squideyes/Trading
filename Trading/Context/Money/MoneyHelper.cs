﻿// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.FxData;
using SquidEyes.Trading.Orders;
using SquidEyes.Basics;

namespace SquidEyes.Trading.Context;

public class MoneyHelper
{
    private readonly UsdValueOf usdValueOf;
    private readonly IMinMargin minMargin;

    public MoneyHelper(UsdValueOf usdValueOf, IMinMargin minMargin)
    {
        this.usdValueOf = usdValueOf
            ?? throw new ArgumentNullException(nameof(usdValueOf));

        this.minMargin = minMargin ??
            throw new ArgumentNullException(nameof(minMargin));
    }

    public double GetPNL(Pair pair, Side side, int units, Rate entryRate, Rate exitRate)
    {
        ArgumentNullException.ThrowIfNull(pair);

        if (!side.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(side));

        double entry = entryRate.GetFloat(pair.Digits);

        double exit = exitRate.GetFloat(pair.Digits);

        var move = (exit - entry) * (side == Side.Buy ? 1.0 : -1.0);

        if (pair.Base == Currency.USD)
        {
            return Math.Round(move * units * (1.0 / exit), 2);
        }
        else if (pair.Quote == Currency.USD)
        {
            return Math.Round(exit * units * move, 2);
        }
        else
        {
            var yieldInBase = 1.0 / exit * units * move;

            var (@base, _) = Known.ConvertWith[pair];

            var baseUsdValueOf = usdValueOf
                .GetRateInUsd(@base.Base).GetFloat(@base.Digits);

            return Math.Round(yieldInBase * baseUsdValueOf, 2);
        }
    }

    public double GetMarginInUsd(
        Pair pair, int units, Leverage leverage, double cushion)
    {
        ArgumentNullException.ThrowIfNull(pair);

        if (units <= 0)
            throw new ArgumentOutOfRangeException(nameof(units));

        if (!leverage.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(leverage));

        if (!cushion.Between(0.0, 1.0))
            throw new ArgumentOutOfRangeException(nameof(cushion));

        var margin = units * Math.Max(1.0 / (int)leverage, minMargin[pair]);

        if (pair.Base != Currency.USD)
            margin *= usdValueOf.GetRateInUsd(pair.Base).GetFloat(pair.Digits);

        if (pair.Base != Currency.USD && pair.Quote == Currency.JPY)
            margin /= 100.0;

        return Math.Round(margin + (margin * cushion), 2);
    }
}