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

public class BollingerBandsIndictorTests
{
    [Fact]
    public void IsPrimedReturnsExpectedValue()
    {
        var indicator = new BollingerBandsIndictor(
            10, Known.Pairs[Symbol.EURUSD], RateToUse.Close, 2.0);

        TestingHelper<BollingerBandsIndictor>.IsPrimedReturnsExpectedValue(
            indicator, 11, (i, c) => i.AddAndCalc(c), i => i.IsPrimed);
    }

    [Fact]
    public void BollingerBandsIndicatorBaseline()
    {
        var results = new (double Upper, double Middle, double Lower)[]
        {
            (1.11663997, 1.11663997, 1.11663997),
            (1.11670494, 1.11657500, 1.11644506),
            (1.11674751, 1.11649334, 1.11623918),
            (1.11681021, 1.11639002, 1.11596983),
            (1.11677830, 1.11632802, 1.11587775),
            (1.11673604, 1.11628836, 1.11584069),
            (1.11669891, 1.11625717, 1.11581544),
            (1.11666387, 1.11624752, 1.11583117),
            (1.11663389, 1.11623446, 1.11583503),
            (1.11660824, 1.11622201, 1.11583579),
            (1.11643934, 1.11616901, 1.11589868),
            (1.11629032, 1.11611501, 1.11593969),
            (1.11619091, 1.11608701, 1.11598310),
            (1.11632694, 1.11611800, 1.11590907),
            (1.11641134, 1.11614900, 1.11588665),
            (1.11659355, 1.11620399, 1.11581442),
            (1.11668388, 1.11625298, 1.11582208),
            (1.11682898, 1.11630999, 1.11579099),
            (1.11690219, 1.11636199, 1.11582179),
            (1.11692829, 1.11640599, 1.11588368),
            (1.11694816, 1.11645499, 1.11596183),
            (1.11687705, 1.11650299, 1.11612893),
            (1.11683711, 1.11651399, 1.11619087),
            (1.11690092, 1.11648099, 1.11606107),
            (1.11690082, 1.11647899, 1.11605716),
            (1.11690339, 1.11647999, 1.11605659),
            (1.11688618, 1.11646399, 1.11604181),
            (1.11682939, 1.11644499, 1.11606059),
            (1.11683583, 1.11644700, 1.11605816),
            (1.11688304, 1.11646399, 1.11604495),
            (1.11691625, 1.11647599, 1.11603573),
            (1.11692959, 1.11648699, 1.11604439),
            (1.11695450, 1.11654599, 1.11613747),
            (1.11686597, 1.11660999, 1.11635401),
            (1.11683504, 1.11662699, 1.11641894),
            (1.11685920, 1.11659799, 1.11633677),
            (1.11685041, 1.11660298, 1.11635555),
            (1.11686619, 1.11658498, 1.11630377),
            (1.11687356, 1.11654598, 1.11621839),
            (1.11684065, 1.11650298, 1.11616530),
            (1.11678075, 1.11646599, 1.11615123),
            (1.11676014, 1.11644399, 1.11612784),
            (1.11665655, 1.11639199, 1.11612743),
            (1.11653579, 1.11634098, 1.11614617),
            (1.11646052, 1.11631199, 1.11616346),
            (1.11645116, 1.11629900, 1.11614683),
            (1.11639835, 1.11627400, 1.11614965),
            (1.11636687, 1.11626400, 1.11616112),
            (1.11639835, 1.11627400, 1.11614965),
            (1.11643936, 1.11628700, 1.11613465),
            (1.11643055, 1.11628300, 1.11613545),
            (1.11642396, 1.11628000, 1.11613603),
            (1.11645298, 1.11629699, 1.11614101),
            (1.11659023, 1.11634300, 1.11609577),
            (1.11668589, 1.11638399, 1.11608210),
            (1.11671361, 1.11641599, 1.11611837),
            (1.11673673, 1.11645700, 1.11617726),
            (1.11676488, 1.11649300, 1.11622111),
            (1.11676839, 1.11650701, 1.11624562),
            (1.11677565, 1.11652000, 1.11626435),
            (1.11681125, 1.11656400, 1.11631675),
            (1.11681152, 1.11660800, 1.11640448),
            (1.11680818, 1.11664300, 1.11647782),
            (1.11689422, 1.11666800, 1.11644177),
            (1.11700802, 1.11670300, 1.11639798),
            (1.11709769, 1.11675200, 1.11640631),
            (1.11718460, 1.11680000, 1.11641539),
            (1.11718489, 1.11681501, 1.11644512),
            (1.11715675, 1.11684000, 1.11652326),
            (1.11711960, 1.11686500, 1.11661041),
            (1.11711747, 1.11687300, 1.11662854),
            (1.11715078, 1.11685600, 1.11656123),
            (1.11718974, 1.11683201, 1.11647427),
            (1.11718272, 1.11680101, 1.11641930),
            (1.11713959, 1.11675202, 1.11636445),
            (1.11705300, 1.11670601, 1.11635903),
            (1.11691503, 1.11663902, 1.11636301),
            (1.11690177, 1.11659402, 1.11628626),
            (1.11686839, 1.11655102, 1.11623364),
            (1.11680645, 1.11649902, 1.11619159),
            (1.11668441, 1.11644402, 1.11620362),
            (1.11666982, 1.11640501, 1.11614020),
            (1.11664581, 1.11638300, 1.11612020),
            (1.11659457, 1.11636701, 1.11613944),
            (1.11657207, 1.11636000, 1.11614793),
            (1.11649760, 1.11634099, 1.11618438),
            (1.11649760, 1.11634099, 1.11618438),
            (1.11651848, 1.11635100, 1.11618351),
            (1.11654237, 1.11636299, 1.11618361),
            (1.11655688, 1.11637999, 1.11620310),
            (1.11655267, 1.11638600, 1.11621932),
            (1.11651109, 1.11640201, 1.11629292),
            (1.11651109, 1.11640201, 1.11629292),
            (1.11655798, 1.11637400, 1.11619003),
            (1.11655187, 1.11634901, 1.11614614),
            (1.11654457, 1.11633401, 1.11612345),
            (1.11652693, 1.11631300, 1.11609908),
            (1.11652343, 1.11627300, 1.11602256),
            (1.11654019, 1.11621100, 1.11588180),
            (1.11653258, 1.11614000, 1.11574741),
            (1.11649367, 1.11609099, 1.11568831),
            (1.11642482, 1.11606098, 1.11569715),
            (1.11636858, 1.11603799, 1.11570740),
            (1.11634983, 1.11600499, 1.11566016),
            (1.11629826, 1.11597400, 1.11564973),
            (1.11624705, 1.11595999, 1.11567293),
            (1.11620346, 1.11594899, 1.11569452),
            (1.11621424, 1.11595299, 1.11569175),
            (1.11632447, 1.11600000, 1.11567552),
            (1.11635569, 1.11605101, 1.11574632),
            (1.11636743, 1.11608500, 1.11580258),
            (1.11636870, 1.11608701, 1.11580532),
            (1.11649476, 1.11612900, 1.11576325),
            (1.11649280, 1.11617800, 1.11586320),
            (1.11649330, 1.11622599, 1.11595869),
            (1.11652424, 1.11625500, 1.11598576),
            (1.11658265, 1.11629601, 1.11600936),
            (1.11661540, 1.11634001, 1.11606461),
            (1.11668392, 1.11636901, 1.11605410),
            (1.11670027, 1.11639100, 1.11608173),
            (1.11670770, 1.11642301, 1.11613832),
            (1.11666409, 1.11647800, 1.11629191),
            (1.11666185, 1.11647000, 1.11627816),
            (1.11669501, 1.11650500, 1.11631499),
            (1.11688874, 1.11656600, 1.11624327),
            (1.11690095, 1.11659501, 1.11628906),
            (1.11692694, 1.11661700, 1.11630706),
            (1.11695692, 1.11664200, 1.11632708),
            (1.11703178, 1.11667500, 1.11631821),
            (1.11719457, 1.11674800, 1.11630143),
            (1.11738842, 1.11683500, 1.11628158),
            (1.11776855, 1.11696000, 1.11615146),
            (1.11807085, 1.11712500, 1.11617915),
            (1.11822125, 1.11724300, 1.11626476),
            (1.11834694, 1.11732801, 1.11630908),
            (1.11840386, 1.11744401, 1.11648416),
            (1.11837779, 1.11753901, 1.11670024),
            (1.11833864, 1.11765301, 1.11696739),
            (1.11827611, 1.11776102, 1.11724593),
            (1.11848056, 1.11790001, 1.11731947),
            (1.11879006, 1.11805301, 1.11731596),
            (1.11905373, 1.11816001, 1.11726629),
            (1.11921223, 1.11823801, 1.11726379),
            (1.11951571, 1.11838701, 1.11725830),
            (1.11968797, 1.11852601, 1.11736404),
            (1.11974121, 1.11864400, 1.11754679),
            (1.11979711, 1.11881900, 1.11784089),
            (1.11982448, 1.11897900, 1.11813352),
            (1.11992015, 1.11917200, 1.11842385),
            (1.11995475, 1.11927500, 1.11859524),
            (1.11994492, 1.11930901, 1.11867309),
            (1.12002363, 1.11939001, 1.11875639),
            (1.11997564, 1.11946101, 1.11894638),
            (1.12000452, 1.11949000, 1.11897549),
            (1.12002328, 1.11953400, 1.11904473),
            (1.11994646, 1.11959600, 1.11924555),
            (1.12001334, 1.11964201, 1.11927067),
            (1.12019726, 1.11971101, 1.11922475),
            (1.12034254, 1.11974800, 1.11915346),
            (1.12037798, 1.11978000, 1.11918203),
            (1.12035390, 1.11985800, 1.11936211),
            (1.12044903, 1.11990800, 1.11936697),
            (1.12059309, 1.12000500, 1.11941691),
            (1.12061085, 1.12006800, 1.11952516),
            (1.12059002, 1.12011900, 1.11964798),
            (1.12053739, 1.12018400, 1.11983062),
            (1.12056931, 1.12024101, 1.11991272),
            (1.12058506, 1.12025400, 1.11992295),
            (1.12057995, 1.12022601, 1.11987207),
            (1.12061141, 1.12021401, 1.11981661),
            (1.12066020, 1.12018701, 1.11971382),
            (1.12068437, 1.12013201, 1.11957964),
            (1.12059541, 1.12005100, 1.11950659),
            (1.12055110, 1.12000700, 1.11946291),
            (1.12051607, 1.11996701, 1.11941794),
            (1.12044073, 1.11989801, 1.11935529),
            (1.12022675, 1.11983401, 1.11944126),
            (1.11999477, 1.11978201, 1.11956924),
            (1.11988652, 1.11975000, 1.11961348),
            (1.11988652, 1.11975000, 1.11961348),
            (1.11996122, 1.11977099, 1.11958076),
            (1.11996105, 1.11977600, 1.11959094),
            (1.12005271, 1.11974200, 1.11943129),
            (1.12006158, 1.11970400, 1.11934641),
            (1.12005010, 1.11967800, 1.11930589),
            (1.12004999, 1.11967700, 1.11930400),
            (1.12002501, 1.11966399, 1.11930297),
            (1.12005559, 1.11967399, 1.11929240),
            (1.12013223, 1.11970299, 1.11927376),
            (1.12013018, 1.11970199, 1.11927380),
            (1.12010161, 1.11969299, 1.11928438),
            (1.12010490, 1.11969500, 1.11928509),
            (1.12009950, 1.11974599, 1.11939248),
            (1.12006399, 1.11976600, 1.11946801),
            (1.12006162, 1.11976699, 1.11947236),
            (1.12006162, 1.11976699, 1.11947236),
            (1.12010268, 1.11973900, 1.11937532),
            (1.12007151, 1.11969500, 1.11931848),
            (1.11998534, 1.11963700, 1.11928867),
            (1.12004451, 1.11955800, 1.11907150),
            (1.12001508, 1.11946701, 1.11891893),
            (1.12001898, 1.11936500, 1.11871103),
            (1.11995254, 1.11923400, 1.11851547),
            (1.11995100, 1.11910300, 1.11825501),
            (1.11986403, 1.11900501, 1.11814599),
            (1.11969811, 1.11890601, 1.11811391),
            (1.11955745, 1.11882701, 1.11809657),
            (1.11934052, 1.11873101, 1.11812150),
            (1.11907258, 1.11861900, 1.11816543),
            (1.11897597, 1.11859300, 1.11821004),
            (1.11890890, 1.11857799, 1.11824708),
            (1.11886579, 1.11855400, 1.11824220),
            (1.11887327, 1.11852900, 1.11818473),
            (1.11886293, 1.11854100, 1.11821908),
            (1.11892088, 1.11856600, 1.11821112),
            (1.11899242, 1.11859300, 1.11819358),
            (1.11898463, 1.11858400, 1.11818337),
            (1.11898769, 1.11857200, 1.11815631),
            (1.11902960, 1.11855299, 1.11807639),
            (1.11899671, 1.11853800, 1.11807929),
            (1.11892355, 1.11850500, 1.11808646),
            (1.11893298, 1.11851200, 1.11809102),
            (1.11895290, 1.11854700, 1.11814110),
            (1.11897510, 1.11856999, 1.11816488),
            (1.11894292, 1.11855799, 1.11817306),
            (1.11887361, 1.11853799, 1.11820236),
            (1.11889908, 1.11855298, 1.11820688),
            (1.11892912, 1.11858698, 1.11824484),
            (1.11879725, 1.11865199, 1.11850672),
            (1.11898407, 1.11869799, 1.11841190),
            (1.11916678, 1.11877199, 1.11837721),
            (1.11944005, 1.11886599, 1.11829193),
            (1.11960445, 1.11894699, 1.11828953),
            (1.11966994, 1.11901000, 1.11835005),
            (1.11969139, 1.11906400, 1.11843660),
            (1.11971054, 1.11912800, 1.11854546),
            (1.11967627, 1.11917601, 1.11867574),
            (1.11962284, 1.11921802, 1.11881319),
            (1.11953115, 1.11926302, 1.11899488),
            (1.11957844, 1.11930702, 1.11903560),
            (1.11958953, 1.11932101, 1.11905250),
            (1.11954551, 1.11930702, 1.11906852),
            (1.11953156, 1.11930301, 1.11907446),
            (1.11955404, 1.11931502, 1.11907599),
            (1.11963983, 1.11935501, 1.11907020),
            (1.11969101, 1.11938101, 1.11907102),
            (1.11971207, 1.11942201, 1.11913195),
            (1.11971677, 1.11946900, 1.11922123),
            (1.11974681, 1.11952100, 1.11929518),
            (1.11976777, 1.11953299, 1.11929821),
            (1.11976777, 1.11953299, 1.11929821),
            (1.11981627, 1.11951399, 1.11921170),
            (1.11984353, 1.11949199, 1.11914046),
            (1.11985159, 1.11947699, 1.11910239),
            (1.11983699, 1.11943299, 1.11902898),
            (1.11983615, 1.11937898, 1.11892182),
            (1.11979962, 1.11932998, 1.11886033),
            (1.11971999, 1.11928798, 1.11885597),
            (1.11958528, 1.11920298, 1.11882069),
            (1.11943673, 1.11912599, 1.11881525),
            (1.11938643, 1.11906999, 1.11875356),
            (1.11935944, 1.11904399, 1.11872854),
            (1.11932064, 1.11900899, 1.11869734),
            (1.11926878, 1.11899500, 1.11872122),
            (1.11924670, 1.11898800, 1.11872930),
            (1.11924121, 1.11898400, 1.11872678),
            (1.11922710, 1.11897700, 1.11872690),
            (1.11919028, 1.11896800, 1.11874571),
            (1.11920374, 1.11898500, 1.11876625),
            (1.11920511, 1.11900200, 1.11879888),
            (1.11919034, 1.11902899, 1.11886763),
            (1.11918517, 1.11903598, 1.11888680),
            (1.11914661, 1.11905398, 1.11896136),
            (1.11912500, 1.11904699, 1.11896897),
            (1.11921158, 1.11906298, 1.11891439),
            (1.11932874, 1.11910099, 1.11887324),
            (1.11938479, 1.11913199, 1.11887920),
            (1.11945530, 1.11916000, 1.11886470),
            (1.11951203, 1.11919500, 1.11887796),
            (1.11954084, 1.11922899, 1.11891715),
            (1.11956045, 1.11925700, 1.11895354),
            (1.11957780, 1.11930400, 1.11903020),
            (1.11958976, 1.11935199, 1.11911423),
            (1.11956107, 1.11939899, 1.11923690),
            (1.11961634, 1.11943499, 1.11925364),
            (1.11977861, 1.11948199, 1.11918537),
            (1.11984328, 1.11952599, 1.11920871),
            (1.11988448, 1.11955999, 1.11923550)
        };

        var indicator = new BollingerBandsIndictor(
            10, Known.Pairs[Symbol.EURUSD], RateToUse.Close, 2.0);

        var candles = IndicatorData.GetCandles();

        candles.Count.Should().Be(results.Length);

        for (var i = 0; i < candles.Count; i++)
        {
            var result = indicator.AddAndCalc(candles[i]);

            result.Upper.Should().BeApproximately(results[i].Upper, 8);
            result.Middle.Should().BeApproximately(results[i].Middle, 8);
            result.Lower.Should().BeApproximately(results[i].Lower, 8);
        }
    }
}