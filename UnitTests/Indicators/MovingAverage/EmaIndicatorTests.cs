// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.Indicators;
using SquidEyes.UnitTests.Testing;
using Xunit;

namespace SquidEyes.UnitTests.Indicators;

public class EmaIndicatorTests
{
    [Fact]
    public void IsPrimedReturnsExpectedValue()
    {
        var indicator = new EmaIndicator(
            10, Known.Pairs[Symbol.EURUSD], RateToUse.Close);

        TestingHelper<EmaIndicator>.IsPrimedReturnsExpectedValue(
            indicator, 10, (i, c) => i.AddAndCalc(c), i => i.IsPrimed);
    }

    [Fact]
    public void EmaIndicatorBaseline()
    {
        var results = new double[]
        {
            1.11663997, 1.11661635, 1.11656429, 1.11647624, 1.11640421, 1.11634709,
            1.11629672, 1.11627548, 1.11624903, 1.11622375, 1.11620306, 1.11616069,
            1.11614056, 1.11618591, 1.11622302, 1.11629883, 1.11634631, 1.11641971,
            1.11646158, 1.11647765, 1.11649990, 1.11649082, 1.11643068, 1.11636328,
            1.11636450, 1.11641641, 1.11641342, 1.11644007, 1.11648188, 1.11652517,
            1.11656058, 1.11656047, 1.11659493, 1.11661405, 1.11660058, 1.11655682,
            1.11653739, 1.11650877, 1.11646717, 1.11643495, 1.11641952, 1.11640507,
            1.11637324, 1.11633991, 1.11632357, 1.11630656, 1.11628718, 1.11628587,
            1.11630298, 1.11632426, 1.11632167, 1.11631955, 1.11633418, 1.11639160,
            1.11644040, 1.11646032, 1.11648754, 1.11651526, 1.11651613, 1.11652228,
            1.11656368, 1.11659756, 1.11662528, 1.11667522, 1.11673609, 1.11679135,
            1.11684565, 1.11683554, 1.11682363, 1.11681932, 1.11682126, 1.11677740,
            1.11672879, 1.11670356, 1.11667019, 1.11665380, 1.11661129, 1.11656197,
            1.11652162, 1.11647768, 1.11644173, 1.11639596, 1.11637669, 1.11638639,
            1.11639795, 1.11639650, 1.11640078, 1.11640792, 1.11641738, 1.11642330,
            1.11640817, 1.11639760, 1.11637803, 1.11633657, 1.11631174, 1.11629870,
            1.11628257, 1.11623846, 1.11616601, 1.11608854, 1.11604517, 1.11604605,
            1.11604859, 1.11600703, 1.11598576, 1.11600652, 1.11602351, 1.11603379,
            1.11608401, 1.11611420, 1.11612797, 1.11611744, 1.11618336, 1.11620638,
            1.11623612, 1.11626410, 1.11630881, 1.11634722, 1.11639319, 1.11640715,
            1.11642586, 1.11646114, 1.11645003, 1.11648820, 1.11657761, 1.11659623,
            1.11662055, 1.11664773, 1.11669905, 1.11679014, 1.11689739, 1.11707422,
            1.11725163, 1.11735862, 1.11744433, 1.11751628, 1.11754604, 1.11761222,
            1.11768455, 1.11784918, 1.11804205, 1.11820531, 1.11831888, 1.11850273,
            1.11863315, 1.11870348, 1.11883557, 1.11895820, 1.11913672, 1.11922458,
            1.11922921, 1.11932391, 1.11936319, 1.11940988, 1.11945536, 1.11948893,
            1.11956185, 1.11967788, 1.11979280, 1.11981957, 1.11985784, 1.11992914,
            1.12003475, 1.12007388, 1.12009136, 1.12012748, 1.12018795, 1.12021377,
            1.12018037, 1.12011484, 1.12005033, 1.11998663, 1.11993451, 1.11991187,
            1.11988608, 1.11983408, 1.11983152, 1.11982760, 1.11980621, 1.11980872,
            1.11983804, 1.11982203, 1.11973802, 1.11968201, 1.11965074, 1.11963971,
            1.11964884, 1.11969633, 1.11975154, 1.11976217, 1.11978358, 1.11978112,
            1.11979727, 1.11976686, 1.11972196, 1.11969798, 1.11964563, 1.11961369,
            1.11957849, 1.11947694, 1.11938478, 1.11926936, 1.11914038, 1.11899121,
            1.11890918, 1.11885297, 1.11881062, 1.11875596, 1.11867305, 1.11868886,
            1.11871269, 1.11867584, 1.11860932, 1.11857854, 1.11861699, 1.11866300,
            1.11863882, 1.11859358, 1.11850564, 1.11852462, 1.11851832, 1.11852953,
            1.11855325, 1.11857448, 1.11859184, 1.11860605, 1.11861949, 1.11863958,
            1.11866147, 1.11873576, 1.11882563, 1.11895187, 1.11904607, 1.11909224,
            1.11911365, 1.11914936, 1.11915130, 1.11915107, 1.11916179, 1.11922511,
            1.11925145, 1.11927482, 1.11930303, 1.11932430, 1.11937624, 1.11941147,
            1.11944030, 1.11947297, 1.11951970, 1.11953975, 1.11950888, 1.11945090,
            1.11940710, 1.11938217, 1.11934358, 1.11928657, 1.11924900, 1.11924009,
            1.11917462, 1.11911742, 1.11906154, 1.11903762, 1.11900532, 1.11902800,
            1.11904108, 1.11903179, 1.11902782, 1.11904276, 1.11904408, 1.11904152,
            1.11904851, 1.11903968, 1.11903974, 1.11904343, 1.11908280, 1.11913501,
            1.11916865, 1.11920890, 1.11924365, 1.11926662, 1.11928359, 1.11931749,
            1.11935430, 1.11938624, 1.11942873, 1.11950351, 1.11955015, 1.11958285
        };

        var indicator = new EmaIndicator(
            10, Known.Pairs[Symbol.EURUSD], RateToUse.Close);

        var candles = IndicatorData.GetCandles();

        candles.Count.Should().Be(results.Length);

        for (var i = 0; i < candles.Count; i++)
        {
            indicator.AddAndCalc(candles[i]).Value
                .Should().BeApproximately(results[i], 8);
        }
    }
}