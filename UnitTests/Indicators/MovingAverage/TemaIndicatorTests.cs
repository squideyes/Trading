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

namespace SquidEyes.UnitTests.Indicators
{
    public class TemaIndicatorTests
    {
        [Fact]
        public void TemaIndicatorBaseline()
        {
            var results = new double[]
            {
                1.11663997, 1.11658120, 1.11646097, 1.11626942, 1.11614565, 1.11607393,
                1.11602434, 1.11604850, 1.11604901, 1.11604619, 1.11604949, 1.11599292,
                1.11599562, 1.11615545, 1.11626468, 1.11645049, 1.11653574, 1.11667377,
                1.11671452, 1.11668933, 1.11668855, 1.11661452, 1.11642949, 1.11625881,
                1.11628637, 1.11643408, 1.11642072, 1.11648381, 1.11657508, 1.11665683,
                1.11670837, 1.11666658, 1.11672104, 1.11673167, 1.11666301, 1.11653320,
                1.11648661, 1.11642492, 1.11634039, 1.11629139, 1.11628965, 1.11628782,
                1.11624032, 1.11619394, 1.11619399, 1.11618881, 1.11617518, 1.11620542,
                1.11627352, 1.11633866, 1.11633260, 1.11632811, 1.11636548, 1.11650291,
                1.11659732, 1.11660688, 1.11663615, 1.11666510, 1.11662604, 1.11660981,
                1.11668670, 1.11673522, 1.11676406, 1.11684736, 1.11694855, 1.11702450,
                1.11709172, 1.11699440, 1.11691466, 1.11687149, 1.11685433, 1.11672875,
                1.11661345, 1.11657494, 1.11652108, 1.11651617, 1.11644464, 1.11636493,
                1.11631676, 1.11626301, 1.11623294, 1.11617804, 1.11619248, 1.11627107,
                1.11633676, 1.11635595, 1.11638369, 1.11641199, 1.11644000, 1.11645373,
                1.11641236, 1.11638865, 1.11634562, 1.11625405, 1.11621695, 1.11621270,
                1.11619875, 1.11611486, 1.11597113, 1.11583415, 1.11579854, 1.11587215,
                1.11593173, 1.11586701, 1.11585884, 1.11595194, 1.11601663, 1.11605161,
                1.11617873, 1.11623493, 1.11624220, 1.11618943, 1.11633686, 1.11635537,
                1.11639058, 1.11641891, 1.11648781, 1.11653375, 1.11659555, 1.11657266,
                1.11657035, 1.11661425, 1.11654005, 1.11660470, 1.11678987, 1.11677358,
                1.11677990, 1.11679764, 1.11687765, 1.11704882, 1.11723873, 1.11757908,
                1.11787642, 1.11796467, 1.11800207, 1.11801552, 1.11793755, 1.11797728,
                1.11803884, 1.11833260, 1.11866243, 1.11888152, 1.11896048, 1.11922139,
                1.11932749, 1.11928862, 1.11943058, 1.11954575, 1.11980122, 1.11981033,
                1.11963213, 1.11972578, 1.11968274, 1.11968061, 1.11968961, 1.11968007,
                1.11978133, 1.11998407, 1.12016252, 1.12010556, 1.12009941, 1.12018781,
                1.12035797, 1.12034645, 1.12029461, 1.12030795, 1.12038878, 1.12037907,
                1.12023214, 1.12003616, 1.11987868, 1.11975002, 1.11966998, 1.11967351,
                1.11966507, 1.11958915, 1.11964449, 1.11968312, 1.11966842, 1.11971232,
                1.11981279, 1.11978219, 1.11958618, 1.11948936, 1.11946563, 1.11949120,
                1.11955811, 1.11970568, 1.11984615, 1.11985260, 1.11988587, 1.11985563,
                1.11987804, 1.11978227, 1.11966706, 1.11962224, 1.11951268, 1.11946943,
                1.11942180, 1.11921319, 1.11905693, 1.11886113, 1.11865450, 1.11842020,
                1.11837774, 1.11839178, 1.11842397, 1.11840738, 1.11831132, 1.11846480,
                1.11860250, 1.11855878, 1.11844191, 1.11842623, 1.11857824, 1.11871850,
                1.11865803, 1.11855291, 1.11835601, 1.11845295, 1.11846643, 1.11851771,
                1.11858891, 1.11864056, 1.11867353, 1.11869337, 1.11870868, 1.11873893,
                1.11877003, 1.11892809, 1.11910191, 1.11934269, 1.11947178, 1.11946978,
                1.11941650, 1.11941664, 1.11934104, 1.11927968, 1.11926147, 1.11938198,
                1.11939583, 1.11940556, 1.11943099, 1.11944006, 1.11952877, 1.11956646,
                1.11958752, 1.11962012, 1.11968773, 1.11968346, 1.11955880, 1.11939145,
                1.11928888, 1.11925006, 1.11918253, 1.11907830, 1.11903646, 1.11906876,
                1.11895109, 1.11886888, 1.11879793, 1.11881187, 1.11879637, 1.11891415,
                1.11898379, 1.11898289, 1.11899270, 1.11904557, 1.11905415, 1.11905053,
                1.11907034, 1.11904689, 1.11904883, 1.11905888, 1.11915563, 1.11926881,
                1.11931897, 1.11938015, 1.11942105, 1.11942992, 1.11942656, 1.11946980,
                1.11951722, 1.11954911, 1.11960664, 1.11973995, 1.11978722, 1.11979870
            };

            var indicator = new TemaIndicator(
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
}