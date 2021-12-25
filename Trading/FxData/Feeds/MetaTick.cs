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
    public class MetaTick : IEquatable<MetaTick>
    {
        public MetaTick(Pair pair, Tick tick)
        {
            Pair = pair;
            Tick = tick;
        }

        public Pair Pair { get; }
        public Tick Tick { get; }

        public override int GetHashCode() => HashCode.Combine(Pair, Tick);

        public bool Equals(MetaTick? other)
        {
            if (other is null)
                return false;

            return Pair.Equals(other.Pair) && Tick.Equals(other.Tick);
        }

        public override bool Equals(object? other) => Equals(other as MetaTick);

        public override string ToString() => $"{Pair},{Tick}";

        public static bool operator ==(MetaTick left, MetaTick right) => left.Equals(right);

        public static bool operator !=(MetaTick left, MetaTick right) => !(left == right);
    }
}
