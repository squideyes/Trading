// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using Ardalis.GuardClauses;
using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public struct Tick : IEquatable<Tick>
{
    public Tick(TickOn tickOn, Rate bid, Rate ask)
    {
        Guard.Against.Default(tickOn, nameof(tickOn));
        Guard.Against.Default(bid, nameof(bid));
        Guard.Against.Default(ask, nameof(ask));
        Guard.Against.InvalidInput(ask, nameof(ask), v => ask >= bid);

        TickOn = tickOn;
        Bid = bid;
        Ask = ask;
    }

    public TickOn TickOn { get; }
    public Rate Bid { get; }
    public Rate Ask { get; }

    public Rate Mid => new((Bid.Value + Ask.Value) / 2);

    public Rate Spread => new(Ask.Value - Bid.Value);

    public bool IsEmpty => TickOn == default;

    public override string ToString() => $"{TickOn},{Bid},{Ask}";

    public string ToCsvString(Pair pair) =>
        ToCsvString(pair.Digits);

    public string ToCsvString(int digits) =>
        $"{TickOn},{Bid.ToString(digits)},{Ask.ToString(digits)}";

    public bool Equals(Tick other) => TickOn.Equals(other.TickOn)
        && Bid.Equals(other.Bid) && Ask.Equals(other.Ask);

    public override bool Equals(object? other) =>
        other is Tick tick && Equals(tick);

    public override int GetHashCode() =>
        HashCode.Combine(TickOn, Bid, Ask);

    public static bool operator ==(Tick left, Tick right) =>
        left.Equals(right);

    public static bool operator !=(Tick left, Tick right) =>
        !(left == right);
}
