// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Context;

public static class OpenOnExtenders
{
    private static readonly long fiveSeconds = TimeSpan.FromSeconds(5).Ticks;
    private static readonly long maxIntervalTicks = TimeSpan.FromHours(1).Ticks;

    internal static bool IsInterval(this TimeSpan value) => IsInterval(value.Ticks);

    internal static bool IsInterval(this long value) =>
        value > 0 && value <= maxIntervalTicks && value % fiveSeconds == 0L;

    public static bool IsOpenOn(this TickOn value, Session session, TimeSpan interval)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (!IsInterval(interval))
            throw new ArgumentOutOfRangeException(nameof(interval));

        return session.InSession(value) && (value.Value.Ticks % interval.Ticks) == 0L;
    }

    public static DateTime ToOpenOn(
        this TickOn value, Session session, TimeSpan interval)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (!IsInterval(interval.Ticks))
            throw new ArgumentOutOfRangeException(nameof(interval));

        if (!session.InSession(value))
            throw new ArgumentOutOfRangeException(nameof(interval));

        return new(value.Value.Ticks - (value.Value.Ticks % interval.Ticks));
    }
}