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

public abstract class EmbargoBase : IEmbargo
{
    public EmbargoBase(EmbargoKind kind) =>
        Kind = kind.Validated(nameof(kind), v => v.IsEnumValue());

    public EmbargoKind Kind { get; }

    public abstract bool IsEmbargoed(Session session, TickOn tickOn);
}