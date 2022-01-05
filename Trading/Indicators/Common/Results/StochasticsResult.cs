﻿// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Indicators;

public class StochasticsResult : ResultBase
{
    public StochasticsResult()
        : base(ResultKind.StochasticsResult)
    {
    }

    public double K { get; init; }
    public double D { get; init; }
}