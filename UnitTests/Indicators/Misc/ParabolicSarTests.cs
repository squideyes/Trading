﻿using FluentAssertions;
using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using SquidEyes.Trading.Indicators;
using SquidEyes.UnitTests.Testing;
using Xunit;

namespace SquidEyes.UnitTests
{
    public class ParabolicSarTests
    {
        private class Baseline
        {
            public ICandle? Candle { get; init; }
            public double Value { get; init; }
        }

        [Fact]
        public void ParabolicSarIndicatorBaseline()
        {
            var results = new double[]
            {
                0.00000000,
                0.00000000,
                0.00000000,
                1.11594720,
                1.11652000,
                1.11650820,
                1.11649664,
                1.11648530,
                1.11647420,
                1.11646331,
                1.11645265,
                1.11644219,
                1.11643195,
                1.11593000,
                1.11594000,
                1.11594980,
                1.11598261,
                1.11601410,
                1.11604434,
                1.11608848,
                1.11612997,
                1.11616897,
                1.11678000,
                1.11676600,
                1.11673736,
                1.11601000,
                1.11601000,
                1.11602420,
                1.11603812,
                1.11605175,
                1.11608248,
                1.11611198,
                1.11615687,
                1.11619905,
                1.11623871,
                1.11627599,
                1.11630000,
                1.11630000,
                1.11686000,
                1.11684640,
                1.11683307,
                1.11682001,
                1.11680721,
                1.11679467,
                1.11676808,
                1.11674256,
                1.11671805,
                1.11669453,
                1.11667195,
                1.11665027,
                1.11662946,
                1.11660948,
                1.11659030,
                1.11613000,
                1.11614080,
                1.11615138,
                1.11617413,
                1.11619596,
                1.11621692,
                1.11623705,
                1.11625637,
                1.11628658,
                1.11632686,
                1.11637617,
                1.11644623,
                1.11652656,
                1.11661351,
                1.11670108,
                1.11710000,
                1.11709160,
                1.11708337,
                1.11707530,
                1.11705549,
                1.11702096,
                1.11698850,
                1.11695799,
                1.11691895,
                1.11686706,
                1.11679301,
                1.11672785,
                1.11667051,
                1.11660884,
                1.11653862,
                1.11647964,
                1.11617000,
                1.11617700,
                1.11618386,
                1.11619058,
                1.11619717,
                1.11620363,
                1.11620996,
                1.11621616,
                1.11622223,
                1.11652000,
                1.11651260,
                1.11649450,
                1.11647712,
                1.11646043,
                1.11643521,
                1.11638679,
                1.11632011,
                1.11626010,
                1.11620609,
                1.11615748,
                1.11611373,
                1.11572000,
                1.11572760,
                1.11574450,
                1.11576072,
                1.11579427,
                1.11583953,
                1.11588117,
                1.11591948,
                1.11597753,
                1.11602978,
                1.11607680,
                1.11611912,
                1.11617322,
                1.11623437,
                1.11629607,
                1.11636338,
                1.11643070,
                1.11670000,
                1.11633000,
                1.11633000,
                1.11633000,
                1.11636960,
                1.11640682,
                1.11644181,
                1.11647471,
                1.11653433,
                1.11661990,
                1.11678431,
                1.11697411,
                1.11713733,
                1.11727770,
                1.11739843,
                1.11750225,
                1.11752000,
                1.11761000,
                1.11779540,
                1.11797000,
                1.11819600,
                1.11837680,
                1.11857544,
                1.11874835,
                1.11888668,
                1.11900000,
                1.11901000,
                1.11920000,
                1.11996000,
                1.11994580,
                1.11993188,
                1.11991825,
                1.11990488,
                1.11989178,
                1.11925000,
                1.11926300,
                1.11931088,
                1.11938283,
                1.11947540,
                1.11956057,
                1.11963892,
                1.11973103,
                1.11981393,
                1.11988854,
                1.11995568,
                1.12001611,
                1.12056000,
                1.12054920,
                1.12051923,
                1.12047008,
                1.12039967,
                1.12033490,
                1.12027531,
                1.12022048,
                1.12017004,
                1.12010504,
                1.12004653,
                1.11999388,
                1.11952000,
                1.11953020,
                1.12003000,
                1.12001640,
                1.11998054,
                1.11994612,
                1.11991308,
                1.11912000,
                1.11914420,
                1.11916792,
                1.11919116,
                1.11921393,
                1.11923626,
                1.11925813,
                1.11927957,
                1.11930058,
                1.11932117,
                1.12033000,
                1.12030940,
                1.12028921,
                1.12023764,
                1.12015438,
                1.12003483,
                1.11987035,
                1.11967711,
                1.11950706,
                1.11935741,
                1.11922572,
                1.11910983,
                1.11900785,
                1.11826000,
                1.11827000,
                1.11902000,
                1.11900500,
                1.11897360,
                1.11822000,
                1.11823580,
                1.11825128,
                1.11901000,
                1.11899200,
                1.11895632,
                1.11892207,
                1.11888918,
                1.11885762,
                1.11882731,
                1.11879822,
                1.11877029,
                1.11874348,
                1.11810000,
                1.11811340,
                1.11815206,
                1.11822514,
                1.11833593,
                1.11843785,
                1.11853163,
                1.11861790,
                1.11869726,
                1.11877028,
                1.11883746,
                1.11889926,
                1.11895612,
                1.11902451,
                1.11908606,
                1.11914145,
                1.11919131,
                1.11923618,
                1.11928824,
                1.11933405,
                1.11937436,
                1.11942415,
                1.11980000,
                1.11980000,
                1.11977520,
                1.11973709,
                1.11970126,
                1.11965076,
                1.11958669,
                1.11952902,
                1.11947712,
                1.11940306,
                1.11931863,
                1.11924602,
                1.11918358,
                1.11880000,
                1.11880840,
                1.11881663,
                1.11882470,
                1.11883261,
                1.11884035,
                1.11884795,
                1.11885539,
                1.11886268,
                1.11886983,
                1.11887683,
                1.11888369,
                1.11890235,
                1.11893160,
                1.11895911,
                1.11899438,
                1.11903794,
                1.11907715,
                1.11911243,
                1.11916614,
                1.11921340,
                1.11925500,
                1.11930750,
                1.11940390,
                1.11948487
            };

            var indicator = new ParabolicSarIndicator(Known.Pairs[Symbol.EURUSD],
                RateToUse.Close, new MinMaxStep(0.02, 0.2, 0.02, 0.02));

            var candles = IndicatorData.GetCandles();

            candles.Count.Should().Be(results.Length);

            for (var i = 0; i < candles.Count; i++)
                indicator.AddAndCalc(candles[i]).Value.Should().BeApproximately(results[i], 8);   
        }
    }
}