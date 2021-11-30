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

namespace SquidEyes.Trading.FxData
{
    public class TickValidator : AbstractValidator<Tick>
    {
        public TickValidator()
        {
            RuleFor(v => v.TickOn)
                .NotEmpty();

            RuleFor(v => v.Bid)
                .NotEmpty();

            RuleFor(v => v.Ask)
                .NotEmpty();

            //RuleFor(v => v)
            //    .Cascade(CascadeMode.Continue)
            //    .Must(v => pair.IsPrice(v.Bid))
            //    .WithName(nameof(Tick.TickOn))
            //    .WithMessage($"'Bid' must be a valid {pair} price.")
            //    .Must(v => pair.IsPrice(v.Ask))
            //    .WithName(nameof(Tick.Ask))
            //    .WithMessage($"'Ask' must be a valid {pair} price.")
            //    .Must(v => v.Ask > v.Bid)
            //    .WithName(nameof(Tick.Bid) + "/" + nameof(Tick.Ask))
            //    .WithMessage("'Ask' must be greater then or equal to 'Bid'.");
        }
    }
}
