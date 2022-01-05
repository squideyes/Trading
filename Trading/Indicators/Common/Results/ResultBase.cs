// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.Indicators;

public abstract class ResultBase : IResult
{
    public ResultBase(ResultKind kind)
    {
        Kind = kind;
    }

    public ResultKind Kind { get; }

    public TickOn OpenOn { get; init; }
}