// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Indicators;

public class ChannelResult : ResultBase
{
    public ChannelResult()
        : base(ResultKind.ChannelResult)
    {
    }

    public double Upper { get; init; }
    public double Middle { get; init; }
    public double Lower { get; init; }

    public override string ToString() =>
        $"{OpenOn:MM/dd/yyyy HH:mm},{Upper},{Middle},{Lower}";
}