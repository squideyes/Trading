// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System;
using System.Collections.Generic;
using System.IO;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.Testing
{
    internal static class IndicatorData
    {
        public class StochasticsBaseLine
        {
            public ICandle? Candle { get; init; }
            public double K { get; init; }
            public double D { get; init; }
        }

        public class ValueBaseline
        {
            public ICandle? Candle { get; init; }
            public double Value { get; init; }
        }

        public class ThreeBandBaseline
        {
            public ICandle? Candle { get; init; }
            public double Upper { get; init; }
            public double Middle { get; init; }
            public double Lower { get; init; }
        }

        public class MacdBaseline
        {
            public ICandle? Candle { get; init; }
            public double Value { get; init; }
            public double Average { get; init; }
            public double Difference { get; init; }
        }

        private static List<ICandle> candles = null!;

        public static List<StochasticsBaseLine> GetStochasticsBaselines() => new()
        {
            //GetStochasticsBaseline(14, 36, 12375.5f, 12383.75f, 12365.75f, 12365.75f, 0, 0),
        };

        internal static List<ThreeBandBaseline> GetBollingerBandsBaselines() => new()
        {
            //GetThreeBandBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75f, 12870.75f, 12870.75),
        };

        internal static List<ThreeBandBaseline> GetKeltnerChannelBaselines() => new()
        {
            //GetThreeBandBaseline(14, 36, 12860.25f, 12860.5f, 12853.75f, 12858.5f, 12867.70833, 12857.58333, 12847.45833),
        };

        internal static List<ValueBaseline> GetAtrBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 7.75),
        };

        internal static List<ValueBaseline> GetCciBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12860.25f, 12860.5f, 12853.75f, 12858.5f, 0),
        };

        internal static List<ValueBaseline> GetDemaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetEmaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetKamaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetSmmaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 1287.075),
        };

        internal static List<ValueBaseline> GetSmaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetTemaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetWmaBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetLinRegBaselines() => new()
        {
            //GetValueBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 12870.75),
        };

        internal static List<ValueBaseline> GetStdDevBaselines() => new()
        {
        };

        internal static List<MacdBaseline> GetMacdBaselines() => new()
        {
            //GetMacdBaseline(14, 36, 12872.75f, 12873.75f, 12866, 12870.75f, 0, 0, 0),
        };

        private static ValueBaseline GetValueBaseline(int hour, int minute,
            int open, int high, int low, int close, double value) => new()
            {
                Candle = GetCandle(hour, minute, open, high, low, close),
                Value = value
            };

        private static StochasticsBaseLine GetStochasticsBaseline(int hour, int minute,
            int open, int high, int low, int close, double k, double d)
        {
            return new StochasticsBaseLine()
            {
                Candle = GetCandle(hour, minute, open, high, low, close),
                K = k,
                D = d
            };
        }

        private static ICandle GetCandle(
            int hour, int minute, int open, int high, int low, int close)
        {
            var tradeDate = new DateOnly(2021, 3, 30);
            var session = new Session(Extent.Day, tradeDate);
            var openOn = new DateTime(2021, 3, 30, hour, minute, 0);
            var closeOn = openOn.AddMinutes(1).AddMilliseconds(-1);

            return new Candle(session, openOn, closeOn,
                new Rate(open), new Rate(high), new Rate(low), new Rate(close));
        }

        private static ThreeBandBaseline GetThreeBandBaseline(int hour, int minute,
            int open, int high, int low, int close, double upper, double middle, double lower)
        {
            return new ThreeBandBaseline()
            {
                Candle = GetCandle(hour, minute, open, high, low, close),
                Upper = upper,
                Middle = middle,
                Lower = lower
            };
        }

        private static MacdBaseline GetMacdBaseline(int hour, int minute, int open,
            int high, int low, int close, double value, double average, double difference)
        {
            return new MacdBaseline()
            {
                Candle = GetCandle(hour, minute, open, high, low, close),
                Value = value,
                Average = average,
                Difference = difference
            };
        }


        public static List<ICandle> GetCandles()
        {
            if (candles != null)
                return candles;

            var tickSet = new TickSet(Source.SquidEyes,
                Known.Pairs[Symbol.EURUSD], new DateOnly(2020, 1, 6));

            using var stream = new MemoryStream(DC_EURUSD_20200106_EST_STS);

            tickSet.LoadFromStream(stream, SaveAs.STS);

            candles = new List<ICandle>();

            var feed = new IntervalFeed(tickSet.Pair, tickSet.Session);

            feed.OnCandle += (s, e) => candles.Add(e.Candle);

            foreach (var tick in tickSet)
                feed.HandleTick(tick);

            return candles;
        }
    }
}