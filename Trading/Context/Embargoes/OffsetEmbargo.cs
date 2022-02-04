// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using System.Text;

namespace SquidEyes.Trading.Context;

public class OffsetEmbargo : EmbargoBase
{
    private readonly TimeSpan minOffset;
    private readonly TimeSpan maxOffset;
    private readonly DayOfWeek? dayOfWeek;

    public OffsetEmbargo(TimeSpan minOffset, TimeSpan maxOffset, DayOfWeek? dayOfWeek = null)
        : base(EmbargoKind.Offset)
    {
        this.minOffset = minOffset.Validated(nameof(minOffset),
            v => v >= TimeSpan.Zero && v < TimeSpan.FromDays(1));

        this.maxOffset = maxOffset.Validated(nameof(maxOffset),
            v => v >= minOffset && v < TimeSpan.FromDays(1));

        this.dayOfWeek = dayOfWeek.Validated(
            nameof(dayOfWeek), v => !v.HasValue || v.Value.IsWeekday());
    }

    public override bool IsEmbargoed(Session session, TickOn tickOn)
    {
        ArgumentNullException.ThrowIfNull(session);

        var tradeDate = tickOn.ToTradeDate(true);

        if (dayOfWeek.HasValue && tradeDate.DayOfWeek != dayOfWeek)
            return false;

        var (minTickOn, maxTickOn) = session.MinTickOn.Value
            .AsFunc(d => (d.Add(minOffset), d.Add(maxOffset)));

        return tickOn >= minTickOn && tickOn <= maxTickOn;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("Offset Embargo (");
        sb.Append(minOffset.ToTimeSpanText());
        sb.Append(" to ");
        sb.Append(maxOffset.ToTimeSpanText());

        if (dayOfWeek.HasValue)
        {
            sb.Append("; ");
            sb.Append(dayOfWeek);
        }

        sb.Append(')');

        return sb.ToString();
    }
}