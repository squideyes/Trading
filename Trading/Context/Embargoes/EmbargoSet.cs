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

public class EmbargoSet : ListBase<IEmbargo>
{
    public EmbargoSet(Session session)
    {
        ArgumentNullException.ThrowIfNull(session);

        Session = session;
    }

    public Session Session { get; }

    public void Add(IEmbargo embargo)
    {
        ArgumentNullException.ThrowIfNull(embargo);

        Items.Add(embargo);
    }

    public (bool IsEmbargoed, IEmbargo? Embargo) IsEmbargoed(
        Session session, TickOn tickOn)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (!Session.InSession(tickOn))
            return (false, null);

        foreach (var embargo in Items)
        {
            if (embargo.IsEmbargoed(session, tickOn))
                return (true, embargo);
        }

        return (false, null);
    }
}