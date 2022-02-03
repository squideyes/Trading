// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Context;

public class AccountId
{
    public static bool IsValid(string value) =>
        TryParse(value, out var _, out var _);

    public static (string TraderId, int Ordinal) Parse(string value)
    {
        if (!TryParse(value, out var traderId, out var ordinal))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (traderId, ordinal);
    }

    public static bool TryParse(
        string value, out string traderId, out int ordinal)
    {
        traderId = default!;
        ordinal = default;

        if (value?.Length != 18)
            return false;

        traderId = value[0..12];

        if (!traderId.IsSquidEyesId())
            return false;

        if (value[12] != '(')
            return false;

        if (value[17] != ')')
            return false;

        return int.TryParse(value[13..^1], out ordinal);
    }

    public static string Create(string traderId, int ordinal)
    {
        if (!traderId.IsSquidEyesId())
            throw new ArgumentOutOfRangeException(nameof(traderId));

        if (ordinal < 1 || ordinal > 9999)
            throw new ArgumentOutOfRangeException(nameof(ordinal));

        return $"{traderId}({ordinal:0000})";
    }
}