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

namespace SquidEyes.Trading.Context
{
    public class MoneyHelper
    {
        private readonly UsdValueOf usdValueOf;

        public MoneyHelper(UsdValueOf usdValueOf)
        {
            this.usdValueOf = usdValueOf 
                ?? throw new ArgumentNullException(nameof(usdValueOf));
        }

        public double GetGrossProfit(
            Pair pair, Side side, int units, Rate entryRate, Rate exitRate)
        {
            if (Equals(pair, null!))
                throw new ArgumentNullException(nameof(pair));

            double entry = entryRate.GetFloat(pair.Digits);

            double exit = exitRate.GetFloat(pair.Digits);

            var move = (exit - entry) * (side == Side.Buy ? 1.0 : -1.0);

            if (pair.Base == Currency.USD)
            {
                return Math.Round(move * units * (1.0 / exit), 2);
            }
            else if (pair.Quote == Currency.USD)
            {
                return Math.Round(exit * units * move, 2);
            }
            else
            {
                var yieldInBase = 1.0 / exit * units * move;

                var (@base, _) = Known.ConvertWith[pair];

                var baseUsdValueOf = usdValueOf
                    .GetRateInUsd(@base.Base).GetFloat(@base.Digits);

                return Math.Round(yieldInBase * baseUsdValueOf, 2);
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