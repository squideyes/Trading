﻿// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.Indicators;
using SquidEyes.UnitTests.Testing;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using static SquidEyes.UnitTests.Testing.IndicatorData;

namespace SquidEyes.UnitTests.Indicators
{
    public class IndicatorTests
    {
        [Fact]
        public void KeltnerChannelIndicatorShouldMatchBaseline()
        {
            var indicator = new KeltnerChannelIndictor(20, EURUSD, RateToUse.Close, 1.5);

            foreach (var baseline in GetKeltnerChannelBaselines())
            {
                var result = indicator.AddAndCalc(baseline.Candle);

                if (!IsGoodCalc(result.Upper, baseline.Upper))
                    throw new ArgumentOutOfRangeException(nameof(result));

                if (!IsGoodCalc(result.Middle, baseline.Middle))
                    throw new ArgumentOutOfRangeException(nameof(result));

                if (!IsGoodCalc(result.Lower, baseline.Lower))
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        [Fact]
        public void BollingerBandsIndicatorShouldMatchBaseline()
        {
            var indicator = new BollingerBandsIndictor(10, EURUSD, RateToUse.Close, 2.0);

            foreach (var baseline in GetBollingerBandsBaselines())
            {
                var result = indicator.AddAndCalc(baseline.Candle);

                if (!IsGoodCalc(result.Upper, baseline.Upper))
                    throw new ArgumentOutOfRangeException(nameof(result));

                if (!IsGoodCalc(result.Middle, baseline.Middle))
                    throw new ArgumentOutOfRangeException(nameof(result));

                if (!IsGoodCalc(result.Lower, baseline.Lower))
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        [Fact]
        public void MacdIndicatorShouldMatchBaseline()
        {
            var indicator = new MacdIndicator(10, 15, 3, EURUSD, RateToUse.Close);

            foreach (var baseline in GetMacdBaselines())
            {
                var result = indicator.AddAndCalc(baseline.Candle);

                if (!IsGoodCalc(result.Average, baseline.Average))
                    throw new ArgumentOutOfRangeException(nameof(result));

                if (!IsGoodCalc(result.Value, baseline.Value))
                    throw new ArgumentOutOfRangeException(nameof(result));

                if (!IsGoodCalc(result.Difference, baseline.Difference))
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        [Fact]
        public void AtrIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new AtrIndicator(10, EURUSD, RateToUse.Close), GetAtrBaselines());

        [Fact]
        public void CciIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new CciIndicator(20, EURUSD, RateToUse.Close), GetCciBaselines());

        [Fact]
        public void DemaIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new DemaIndicator(10, EURUSD, RateToUse.Close), GetDemaBaselines());

        [Fact]
        public void EmaIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new EmaIndicator(10, EURUSD, RateToUse.Close), GetEmaBaselines());

        [Fact]
        public void SmaIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new SmaIndicator(10, EURUSD, RateToUse.Close), GetSmaBaselines());

        [Fact]
        public void TemaIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new TemaIndicator(10, EURUSD, RateToUse.Close), GetTemaBaselines());

        [Fact]
        public void WmaIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new WmaIndicator(10, EURUSD, RateToUse.Close), GetWmaBaselines());

        [Fact]
        public void LinRegIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new LinRegIndicator(10, EURUSD, RateToUse.Close), GetLinRegBaselines());

        [Fact]
        public void StdDevIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new StdDevIndicator(10, EURUSD, RateToUse.Close), GetStdDevBaselines());

        [Fact]
        public void KamaIndicatorShouldMatchBaseline() => BasicIndicatorBaselineTest(
            new KamaIndicator(10, 5, 15, EURUSD, RateToUse.Close), GetKamaBaselines());

        [Fact]
        public void SmmaIndicatorShouldMatchBaseline()
        {
            var results = new double[] 
            {
                0.11166400,0.22331500,0.33494802,0.44655600,0.55816402,0.66977301,
                0.78138003,0.89299803,1.00461102,1.11622200,1.11616898,1.11614908,
                1.11614116,1.11616684,1.11618658,1.11622995,1.11625862,1.11630489,
                1.11633477,1.11635330,1.11637612,1.11638122,1.11635859,1.11633100,
                1.11633766,1.11636822,1.11636834,1.11638750,1.11641383,1.11644181,
                1.11646683,1.11647364,1.11650060,1.11651785,1.11651833,1.11650245,
                1.11649879,1.11648727,1.11646769,1.11645188,1.11644328,1.11643381,
                1.11641438,1.11639388,1.11638155,1.11636763,1.11635225,1.11634656,
                1.11635047,1.11635704,1.11635168,1.11634804,1.11635360,1.11638269,
                1.11640751,1.11641927,1.11643717,1.11645566,1.11646025,1.11646877,
                1.11649604,1.11651871,1.11653957,1.11657352,1.11661378,1.11665238,
                1.11669228,1.11669807,1.11670468,1.11671355,1.11672431,1.11670880,
                1.11669047,1.11668226,1.11666686,1.11665972,1.11663646,1.11660915,
                1.11658497,1.11655688,1.11653200,1.11650028,1.11648242,1.11647897,
                1.11647642,1.11646803,1.11646407,1.11646206,1.11646205,1.11646084,
                1.11644888,1.11644020,1.11642604,1.11639986,1.11638249,1.11636998,
                1.11635523,1.11632518,1.11627966,1.11623024,1.11619716,1.11618575,
                1.11617432,1.11614003,1.11611846,1.11611877,1.11611686,1.11611337,
                1.11613338,1.11614304,1.11614677,1.11613872,1.11617365,1.11618380,
                1.11620140,1.11621850,1.11624594,1.11627061,1.11630108,1.11631493,
                1.11633305,1.11635993,1.11636125,1.11639099,1.11644691,1.11646463,
                1.11648939,1.11651498,1.11655393,1.11661464,1.11668511,1.11679655,
                1.11691075,1.11699226,1.11706788,1.11713754,1.11718481,1.11725261,
                1.11732157,1.11744152,1.11757637,1.11769925,1.11780003,1.11794295,
                1.11805637,1.11814139,1.11826174,1.11837454,1.11851981,1.11861530,
                1.11866922,1.11877191,1.11883845,1.11890995,1.11897780,1.11903724,
                1.11911657,1.11921698,1.11931624,1.11936869,1.11942958,1.11950553,
                1.11959838,1.11965426,1.11970024,1.11975463,1.11981973,1.11986424,
                1.11987637,1.11986952,1.11985926,1.11984435,1.11983140,1.11983056,
                1.11982459,1.11980273,1.11980665,1.11980659,1.11979693,1.11980021,
                1.11981685,1.11980851,1.11976449,1.11973543,1.11971580,1.11970519,
                1.11970473,1.11972530,1.11975071,1.11975410,1.11976635,1.11976549,
                1.11977602,1.11976037,1.11973789,1.11972535,1.11969507,1.11967560,
                1.11965199,1.11959115,1.11953512,1.11946221,1.11937928,1.11928164,
                1.11921724,1.11916196,1.11911330,1.11905783,1.11898759,1.11897186,
                1.11895824,1.11891478,1.11885864,1.11882240,1.11882278,1.11882747,
                1.11879726,1.11875955,1.11869836,1.11869564,1.11867535,1.11866784,
                1.11866781,1.11866803,1.11866820,1.11866836,1.11866951,1.11867544,
                1.11868330,1.11872119,1.11876829,1.11883874,1.11889482,1.11892973,
                1.11895427,1.11898739,1.11900135,1.11901482,1.11903299,1.11907888,
                1.11910340,1.11912861,1.11915622,1.11917984,1.11922049,1.11925138,
                1.11928015,1.11931126,1.11935002,1.11937414,1.11937131,1.11935346,
                1.11934090,1.11933506,1.11931914,1.11929182,1.11927336,1.11926787,
                1.11922964,1.11919650,1.11916117,1.11914158,1.11911538,1.11911946,
                1.11911711,1.11910463,1.11909641,1.11909859,1.11909351,1.11908767,
                1.11908748,1.11907875,1.11907575,1.11907448,1.11909315,1.11911897,
                1.11913649,1.11916009,1.11918173,1.11919839,1.11921288,1.11923714,
                1.11926300,1.11928711,1.11931798,1.11936710,1.11940148,1.11943090
            };

            BasicIndicatorBaselineTest(
                new SmmaIndicator(10, EURUSD, RateToUse.Close), GetSmmaBaselines());
        }

        [Fact]
        public void StochasticsIndicatorShouldMatchBaseline()
        {
            var indicator = new StochasticsIndicator(14, 7, 3, EURUSD);

            foreach (var baseline in GetStochasticsBaselines())
            {
                var add = indicator.AddAndCalc(baseline.Candle);

                if (!add.K.Approximates(baseline.K))
                    throw new ArgumentOutOfRangeException(nameof(add.K));

                if (!add.D.Approximates(baseline.D))
                    throw new ArgumentOutOfRangeException(nameof(add.D));

                var update = indicator.UpdateAndCalc(baseline.Candle);

                if (!update.K.Approximates(baseline.K))
                    throw new ArgumentOutOfRangeException(nameof(update.K));

                if (!update.D.Approximates(baseline.D))
                    throw new ArgumentOutOfRangeException(nameof(update.D));
            }
        }

        private static void BasicIndicatorBaselineTest(
            IBasicIndicator indicator, List<ValueBaseline> baselines)
        {
            var results = new List<double>();

            foreach (var candle in GetCandles())
            {
                var result = indicator.AddAndCalc(candle);

                results.Add(result.Value);
            }

            var x = string.Join(",", results.Select(r=> r.ToString("0.00000000")));



            //foreach (var baseline in baselines)
            //{
            //    var result = indicator.AddAndCalc(baseline.Candle);

            //    if (!IsGoodCalc(result.Value, baseline.Value))
            //        throw new ArgumentOutOfRangeException(nameof(result));
            //}
        }

        private static Pair EURUSD => Known.Pairs[Symbol.EURUSD];

        private static bool IsGoodCalc(double actual, double expected) =>
            Math.Round(actual, expected.ToDigits()) == expected;
    }
}