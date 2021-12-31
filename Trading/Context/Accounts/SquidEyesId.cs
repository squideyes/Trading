// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using System.Text;

namespace SquidEyes.Trading.Context;

public struct SquidEyesId
{
    public const int Length = 12;

    internal static readonly char[] ValidChars =
        "RCLTKJ4BQWHEPF9ZN8AG3V2UDYS675XM".ToCharArray();

    private static readonly Random random = new();

    public static string Create()
    {
        var sb = new StringBuilder(Length);

        for (var i = 0; i < Length; i++)
            sb.Append(ValidChars[random.Next(32)]);

        return sb.ToString();
    }

    public static bool IsValid(string value) =>
        value?.Length == Length && value.All(c => ValidChars.Contains(c));
}