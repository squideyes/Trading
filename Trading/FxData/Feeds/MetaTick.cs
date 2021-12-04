// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData
{
    public class MetaTick
    {
        public MetaTick(Pair pair, Tick tick)
        {
            Pair = pair;
            Tick = tick;
        }

        public Pair Pair { get; }
        public Tick Tick { get; }

        public override string ToString() => $"{Pair},{Tick}";
    }
}
