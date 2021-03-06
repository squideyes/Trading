// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Indicators;

public class MacdResult : ResultBase
{
    public MacdResult()
        : base(ResultKind.MacdResult)
    {
    }

    public double Value { get; init; }
    public double Average { get; init; }
    public double Difference { get; init; }
}