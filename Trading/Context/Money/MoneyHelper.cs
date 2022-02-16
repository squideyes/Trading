// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.FxData;
using SquidEyes.Trading.Orders;
using SquidEyes.Trading.Shared.Helpers;

namespace SquidEyes.Trading.Context
{
    public class MoneyHelper
    {
        private readonly UsdValueOf usdValueOf;

        public MoneyHelper(UsdValueOf usdValueOf)
        {
            this.usdValueOf = usdValueOf;
        }

        public double GetGrossProfit(Pair pair, Side side, int units, Rate entryRate, Rate exitRate)
        {
            if (Equals(pair, null!))
                throw new ArgumentNullException(nameof(pair));

            var entry = entryRate.GetFloat(pair.Digits);

            var exit = exitRate.GetFloat(pair.Digits);

            var move = (exit - entry) * (side == Side.Buy ? 1.0f : -1.0f);

            if (pair.Base == Currency.USD)
            {
                return MathF.Round(move * units * (1.0f / exit), 2);
            }
            else if (pair.Quote == Currency.USD)
            {
                return MathF.Round(exit * units * move, 2);
            }
            else
            {
                var eurToUsdConversionRate = usdValueOf.GetRateInUsd(pair.Base);

                return 0; // eurToUsdConversionRate.GetFloat(pair.Digits) * yieldInBase;
            }
        }




        // Units traded * margin percent * value of 
        public double MarginNeeded()
        {
            return default;
        }

        //  entrymargin, initialmargin, maint margin

        // Fee assessed separately


        // Entry to Current Price, converted to USD
        public double UnrealizedProfit()
        {
            return default;
        }

        // Unrealized P&L (all open trades) + Realized in USD
        public double LiquidationValue()
        {
            return 0;
        }
    }
}