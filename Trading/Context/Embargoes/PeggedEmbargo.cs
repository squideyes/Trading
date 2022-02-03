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

public class PeggedEmbargo : EmbargoBase
{
    private readonly PegTo pegTo;
    private readonly int minutes;

    public PeggedEmbargo(PegTo pegTo, int minutes)
        : base(GetEmbargoKind(pegTo))
    {
        this.pegTo = pegTo.Validated(nameof(pegTo), v => v.IsEnumValue());
        
        this.minutes = minutes.Validated(nameof(minutes), v => v.Between(1, 720));
    }

    public override bool IsEmbargoed(Session session, TickOn tickOn)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (tickOn == default)
            throw new ArgumentOutOfRangeException(nameof(tickOn));

        TickOn minTickOn;
        TickOn maxTickOn;

        if (Kind == EmbargoKind.Open)
        {
            minTickOn = session.MinTickOn;

            maxTickOn = session.MinTickOn.Value
                .AddMinutes(minutes).AddMilliseconds(-1);
        }
        else
        {
            minTickOn = session.MaxTickOn.Value
                .AddMinutes(-minutes).AddMilliseconds(1);

            maxTickOn = session.MaxTickOn;
        }

        return tickOn >= minTickOn && tickOn <= maxTickOn;
    }

    public override string ToString() =>
        $"Pegged Embargo ({pegTo}; {minutes} Minutes)";

    private static EmbargoKind GetEmbargoKind(PegTo pegTo)
    {
        return pegTo switch
        {
            PegTo.Open => EmbargoKind.Open,
            PegTo.Close => EmbargoKind.Close,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}