// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.Trading.Context;

public class OneTimeEmbargo : EmbargoBase
{
    private readonly TickOn minTickOn;
    private readonly TickOn maxTickOn;

    public OneTimeEmbargo(TickOn minTickOn, TickOn maxTickOn, bool isAdHoc)
        : base(isAdHoc ? EmbargoKind.AdHoc : EmbargoKind.OneTime)
    {
        this.minTickOn = minTickOn.Validated(
            nameof(minTickOn), v => !v.IsDefaultValue());

        if (maxTickOn.IsDefaultValue() || maxTickOn.TradeDate != minTickOn.TradeDate)
            throw new ArgumentOutOfRangeException(nameof(maxTickOn));

        this.maxTickOn = maxTickOn.Validated(nameof(maxTickOn), v => v > minTickOn);
    }

    public override bool IsEmbargoed(Session session, TickOn tickOn)
    {
        ArgumentNullException.ThrowIfNull(session);

        return tickOn >= minTickOn && tickOn <= maxTickOn;
    }

    public override string ToString() =>
        $"{Kind} Embargo ({minTickOn} to {maxTickOn})";
}