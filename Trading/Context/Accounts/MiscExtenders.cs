// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.Context;

public static class MiscExtenders
{
    public static bool IsSquidEyesId(this string value) =>
        SquidEyesId.IsValid(value);

    public static bool IsAccountId(this string value) =>
        AccountId.IsValid(value);
}