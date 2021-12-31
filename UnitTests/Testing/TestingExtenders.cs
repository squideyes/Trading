// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using System;
using System.Collections.Generic;

namespace SquidEyes.UnitTests.Testing;

internal static class TestingExtenders
{
    public static Type ToType(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.ArgumentException => typeof(ArgumentException),
            ErrorType.ArgumentNullException => typeof(ArgumentNullException),
            ErrorType.ArgumentOutOfRangeException => typeof(ArgumentOutOfRangeException),
            ErrorType.KeyNotFoundException => typeof(KeyNotFoundException),
            _ => throw new ArgumentOutOfRangeException(nameof(errorType))
        };
    }
}