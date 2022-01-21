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

public class AtrIndicatorTests
{
    [Fact]
    public void AtrIndicatorBaseline()
    {
        var results = new double[]
        {
            19.00000000, 16.50000000, 26.33333333, 30.75000000, 30.80000000,
            26.50000000, 23.28571429, 22.00000000, 20.33333333, 18.80000000,
            17.32000000, 17.78800000, 18.40920000, 20.36828000, 19.43145200,
            21.58830680, 22.72947612, 22.55652851, 21.80087566, 22.22078809,
            22.29870928, 21.56883835, 23.41195452, 22.87075907, 24.28368316,
            25.35531484, 25.51978336, 24.66780502, 24.10102452, 23.29092207,
            22.26182986, 23.13564688, 23.32208219, 21.88987397, 22.10088657,
            22.29079792, 21.46171812, 21.01554631, 21.61399168, 21.15259251,
            20.13733326, 19.22359993, 19.00123994, 18.50111595, 17.75100435,
            17.27590392, 17.64831353, 17.48348217, 17.43513396, 17.39162056,
            16.75245850, 15.77721265, 15.89949139, 17.00954225, 16.40858802,
            16.56772922, 15.81095630, 14.82986067, 15.04687460, 14.54218714,
            15.28796843, 14.65917159, 13.89325443, 14.70392898, 14.43353609,
            13.89018248, 13.70116423, 15.43104781, 15.38794303, 15.74914872,
            15.97423385, 16.87681047, 16.38912942, 16.05021648, 15.54519483,
            15.09067535, 15.38160781, 15.64344703, 14.77910233, 14.40119209,
            13.66107289, 13.69496560, 14.02546904, 14.32292213, 13.79062992,
            13.31156693, 12.78041024, 12.10236921, 11.29213229, 10.76291906,
            11.58662716, 10.62796444, 10.56516800, 11.00865120, 11.60778608,
            11.14700747, 10.53230672, 11.37907605, 12.44116844, 12.59705160,
            12.73734644, 13.66361180, 12.79725062, 13.91752556, 14.02577300,
            14.72319570, 14.55087613, 13.89578852, 15.00620966, 15.80558870,
            16.22502983, 18.30252685, 21.17227416, 22.45504675, 22.80954207,
            22.72858786, 22.25572908, 21.43015617, 20.38714055, 20.54842650,
            21.09358385, 20.88422546, 21.79580292, 23.41622262, 25.17460036,
            25.95714033, 25.36142629, 24.52528366, 24.97275530, 26.97547977,
            27.57793179, 31.02013861, 30.81812475, 31.73631228, 30.66268105,
            29.99641294, 30.39677165, 30.45709448, 30.71138504, 34.34024653,
            34.30622188, 34.07559969, 32.66803972, 35.20123575, 33.88111217,
            32.79300096, 34.81370086, 35.53233078, 37.87909770, 37.89118793,
            38.50206914, 39.65186222, 38.68667600, 36.41800840, 34.47620756,
            33.12858680, 32.61572812, 35.05415531, 34.94873978, 37.55386580,
            35.79847922, 35.31863130, 34.88676817, 34.99809135, 33.79828222,
            32.41845400, 32.07660860, 30.86894774, 31.98205296, 31.88384767,
            30.89546290, 29.80591661, 28.72532495, 27.05279245, 25.74751321,
            25.47276189, 26.02548570, 25.02293713, 25.72064342, 25.84857907,
            25.96372117, 25.96734905, 27.57061415, 27.91355273, 27.62219746,
            27.35997771, 27.42397994, 31.98158195, 30.58342375, 31.12508138,
            29.91257324, 28.72131592, 28.94918432, 28.95426589, 27.25883930,
            27.33295537, 27.09965983, 26.28969385, 26.86072447, 28.37465202,
            27.23718682, 27.61346814, 28.65212132, 30.68690919, 30.61821827,
            30.75639644, 30.48075680, 29.43268112, 29.38941301, 31.85047171,
            31.46542454, 31.51888208, 31.96699387, 32.27029449, 33.24326504,
            34.21893853, 35.29704468, 34.06734021, 33.76060619, 35.48454557,
            34.73609102, 33.86248191, 32.87623372, 31.48861035, 29.63974932,
            28.37577438, 27.63819695, 26.57437725, 27.31693953, 27.78524557,
            28.00672102, 29.20604891, 28.08544402, 27.47689962, 26.32920966,
            25.19628869, 24.37665982, 23.73899384, 23.66509446, 24.49858501,
            24.74872651, 23.37385386, 21.93646847, 21.34282163, 21.50853946,
            20.45768552, 19.51191697, 18.76072527, 17.98465274, 17.98618747,
            19.18756872, 19.56881185, 18.31193066, 17.68073760, 17.91266384,
            18.12139745, 17.30925771, 17.97833194, 19.88049874, 18.79244887,
            17.61320398, 17.55188358, 17.69669523, 19.52702570, 19.37432313,
            19.03689082, 18.23320174, 17.70988156, 17.13889341, 16.52500407,
            16.07250366, 15.96525329, 15.26872796, 15.04185517, 16.43766965,
            17.09390269, 16.18451242, 15.56606118, 14.60945506, 14.04850955,
            13.04365860, 13.73929274, 12.96536346, 12.16882712, 12.05194441,
            13.74674997, 13.37207497, 13.03486747
            };

        var indicator = new AtrIndicator(
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