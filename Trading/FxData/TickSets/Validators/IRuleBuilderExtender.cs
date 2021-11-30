// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentValidation;
using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData;

public static class IRuleBuilderExtenders
{
    public static IRuleBuilderOptionsConditions<T, DateTime> TickOn<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder, Session session)
    {
        return ruleBuilder.Custom((item, context) =>
        {
            if (!session.InSession(item))
                context.AddFailure($"'{context.PropertyName}' must be a valid TickOn ({session}).");
        });
    }
}
