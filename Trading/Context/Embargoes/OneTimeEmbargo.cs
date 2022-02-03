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
    private readonly DateTime minTickOn;
    private readonly DateTime maxTickOn;

    public OneTimeEmbargo(Session session, DateTime minTickOn, 
        DateTime maxTickOn, bool isAdHoc = false)
        : base(isAdHoc ? EmbargoKind.AdHoc : EmbargoKind.OneTime)
    {
        ArgumentNullException.ThrowIfNull(session);

        this.minTickOn = minTickOn.Validated(
            nameof(minTickOn), v => session.InSession(minTickOn));

        this.maxTickOn = maxTickOn.Validated(nameof(maxTickOn),
            v => session.InSession(v) && maxTickOn > minTickOn
                && minTickOn.ToTradeDate() == maxTickOn.ToTradeDate());
    }

    public override bool IsEmbargoed(Session session, TickOn tickOn)
    {
        ArgumentNullException.ThrowIfNull(session);

        return tickOn >= minTickOn && tickOn <= maxTickOn;
    }

    public override string ToString() =>
        $"OneTime Embargo ({minTickOn.ToTickOnText()} to {maxTickOn.ToTickOnText()}";
}