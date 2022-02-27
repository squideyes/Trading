// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData
{
    public class Wicko
    {
        public Symbol Symbol { get; internal set; }
        public TickOn OpenOn { get; internal set; }
        public TickOn CloseOn { get; internal set; }
        public Rate Open { get; internal set; }
        public Rate High { get; internal set; }
        public Rate Low { get; internal set; }
        public Rate Close { get; internal set; }

        public Trend Trend
        {
            get
            {
                if (Open < Close)
                    return Trend.Rising;
                else if (Open > Close)
                    return Trend.Falling;
                else
                    return Trend.NoTrend;
            }
        }

        public Rate Spread => (int)MathF.Abs(Close.Value - Open.Value);

        public override string ToString() =>
            $"{OpenOn:MM/dd/yyyy HH:mm:ss.fff},{CloseOn:MM/dd/yyyy HH:mm:ss.fff},{Open},{High},{Low},{Close}";
    }
}