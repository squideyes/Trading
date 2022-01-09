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
    public class LinRegIndicatorTests
    {
        [Fact]
        public void LinRegIndicatorBaseline()
        {
            var results = new double[]
            {
                1.11663997, 1.11651003, 1.11633837, 1.11611105, 1.11601806, 1.11598197,
                1.11595720, 1.11599168, 1.11599845, 1.11599835, 1.11604324, 1.11604270,
                1.11606325, 1.11616652, 1.11625289, 1.11641071, 1.11650690, 1.11666399,
                1.11674545, 1.11675998, 1.11676781, 1.11666907, 1.11646000, 1.11627838,
                1.11622182, 1.11631308, 1.11630145, 1.11639181, 1.11651054, 1.11662543,
                1.11674107, 1.11676488, 1.11679034, 1.11674637, 1.11667417, 1.11658652,
                1.11649579, 1.11641015, 1.11632178, 1.11626814, 1.11625871, 1.11623401,
                1.11622129, 1.11621310, 1.11622148, 1.11620275, 1.11619873, 1.11622199,
                1.11626525, 1.11631889, 1.11633944, 1.11636018, 1.11639164, 1.11649217,
                1.11658309, 1.11660963, 1.11663347, 1.11666127, 1.11664801, 1.11664546,
                1.11668291, 1.11669637, 1.11670382, 1.11679399, 1.11691054, 1.11699636,
                1.11708473, 1.11704929, 1.11697476, 1.11690291, 1.11686782, 1.11674200,
                1.11660129, 1.11653129, 1.11647602, 1.11647421, 1.11645221, 1.11637913,
                1.11632660, 1.11628274, 1.11627326, 1.11621270, 1.11619396, 1.11625162,
                1.11631088, 1.11636524, 1.11640835, 1.11644238, 1.11647784, 1.11649127,
                1.11645747, 1.11640584, 1.11634473, 1.11626329, 1.11621836, 1.11618891,
                1.11616327, 1.11609437, 1.11598216, 1.11586723, 1.11580705, 1.11584470,
                1.11589016, 1.11583728, 1.11583656, 1.11593328, 1.11603163, 1.11609511,
                1.11619583, 1.11623020, 1.11623799, 1.11622582, 1.11635618, 1.11635691,
                1.11636563, 1.11639708, 1.11645417, 1.11648838, 1.11657221, 1.11658329,
                1.11658420, 1.11658164, 1.11655509, 1.11658873, 1.11672579, 1.11672998,
                1.11675961, 1.11679309, 1.11688419, 1.11702456, 1.11719530, 1.11750982,
                1.11777436, 1.11792838, 1.11807857, 1.11812585, 1.11806455, 1.11803893,
                1.11801766, 1.11819184, 1.11843673, 1.11870654, 1.11891597, 1.11921363,
                1.11939000, 1.11942346, 1.11950216, 1.11955909, 1.11969730, 1.11973565,
                1.11965402, 1.11973257, 1.11967237, 1.11970109, 1.11970582, 1.11963964,
                1.11970798, 1.11987436, 1.12012708, 1.12016781, 1.12014820, 1.12026202,
                1.12039637, 1.12040400, 1.12035763, 1.12032092, 1.12035747, 1.12038001,
                1.12031383, 1.12011638, 1.11992276, 1.11976710, 1.11969344, 1.11964998,
                1.11960072, 1.11953856, 1.11962402, 1.11971491, 1.11973963, 1.11977782,
                1.11985581, 1.11983436, 1.11967543, 1.11958123, 1.11952744, 1.11948147,
                1.11951454, 1.11962873, 1.11974855, 1.11980891, 1.11992398, 1.11996201,
                1.11995544, 1.11985215, 1.11971597, 1.11961945, 1.11948073, 1.11942201,
                1.11938803, 1.11920948, 1.11907075, 1.11888366, 1.11870654, 1.11846999,
                1.11835890, 1.11833600, 1.11833801, 1.11835710, 1.11834600, 1.11847491,
                1.11862871, 1.11863963, 1.11855652, 1.11848398, 1.11856982, 1.11868165,
                1.11866530, 1.11858347, 1.11836945, 1.11843055, 1.11847035, 1.11849725,
                1.11850797, 1.11852906, 1.11860761, 1.11870871, 1.11875618, 1.11878471,
                1.11874907, 1.11888510, 1.11902729, 1.11924727, 1.11941473, 1.11948128,
                1.11948237, 1.11948856, 1.11941003, 1.11931185, 1.11921748, 1.11926423,
                1.11927058, 1.11933074, 1.11940581, 1.11944564, 1.11952652, 1.11959180,
                1.11961290, 1.11962689, 1.11966526, 1.11970072, 1.11961181, 1.11946271,
                1.11934089, 1.11924980, 1.11917033, 1.11905852, 1.11899342, 1.11900652,
                1.11895399, 1.11892091, 1.11886057, 1.11883620, 1.11880584, 1.11889984,
                1.11897110, 1.11898017, 1.11900833, 1.11909887, 1.11910962, 1.11910017,
                1.11908871, 1.11905888, 1.11902507, 1.11904237, 1.11912654, 1.11921799,
                1.11927546, 1.11936019, 1.11942111, 1.11944856, 1.11946400, 1.11948617,
                1.11950798, 1.11951106, 1.11955960, 1.11968652, 1.11975018, 1.11979345
            };

            var indicator = new LinRegIndicator(
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