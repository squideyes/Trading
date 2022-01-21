// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.Trading.FxData;

public static class FloatExtenders
{
    public static bool IsRateValue(this float value, int digits) =>
        Rate.IsRate(value, digits);
}