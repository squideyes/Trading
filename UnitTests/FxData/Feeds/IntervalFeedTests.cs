// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using SquidEyes.UnitTests.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.FxData;

public class IntervalFeedTests
{
    [Fact]
    public void ConstructWithGoodArgs()
    {
        var pair = Known.Pairs[Symbol.EURUSD];
        var tradeDate = new DateOnly(2020, 1, 6);
        var session = new Session(Extent.Day, tradeDate);

        var feed = new IntervalFeed(pair, session, 15);

        feed.Pair.Should().Be(pair);
        feed.Session.Should().Be(session);
        feed.Interval.Should().Be(TimeSpan.FromSeconds(15));
    }

    //////////////////////////

    //[Theory]
    //[InlineData(Extent.Day)]
    //[InlineData(Extent.Week)]
    //public void RaiseOnCandleAndResetDoes(Extent extent)
    //{
    //    static (MetaTickSet MetaTicks, IntervalFeed Feed) GetMetaTicksAndFeed(
    //        int seconds, Extent extent = Extent.Day)
    //    {
    //        var pair = Known.Pairs[Symbol.EURUSD];

    //        var tradeDate = new DateOnly(2020, 1, 6);

    //        var session = new Session(extent, tradeDate);

    //        var metaTicks = new MetaTickSet(Source.Dukascopy, session);

    //        TickSet GetTickSet(byte[] bytes, int days)
    //        {
    //            var tickSet = new TickSet(
    //                Source.Dukascopy, pair!, tradeDate.AddDays(days));

    //            tickSet.LoadFromStream(bytes.ToStream(), DataKind.STS);

    //            return tickSet;
    //        }

    //        var tickSets = new List<TickSet>
    //        {
    //            GetTickSet(DC_EURUSD_20200106_EST_STS, 0)
    //        };

    //        if (extent == Extent.Week)
    //        {
    //            tickSets.Add(GetTickSet(DC_EURUSD_20200107_EST_STS, 1));
    //            tickSets.Add(GetTickSet(DC_EURUSD_20200108_EST_STS, 2));
    //            tickSets.Add(GetTickSet(DC_EURUSD_20200109_EST_STS, 3));
    //            tickSets.Add(GetTickSet(DC_EURUSD_20200110_EST_STS, 4));
    //        }

    //        metaTicks.Add(tickSets);

    //        return (metaTicks, new IntervalFeed(pair, session, seconds));
    //    }

    //    var (metaTicks, feed) = GetMetaTicksAndFeed(60 * 7, extent);

    //    var candles = new List<ICandle>();

    //    feed.OnCandle += (s, e) =>
    //    {
    //        candles.Add(e.Candle);

    //        System.Diagnostics.Debug.WriteLine(e.Candle.ToString());
    //    };

    //    var list = metaTicks.ToList();

    //    foreach (var metaTick in list.Take(list.Count - 1))
    //        feed.HandleTick(metaTick.Tick);

    //    candles.Count.Should().Be(extent == Extent.Day ? 205 : 1028);

    //    var day = extent == Extent.Day ? 6 : 10;

    //    var offset = extent == Extent.Day ? 1 : 0;

    //    candles.Last().OpenOn.Should().Be(
    //        new DateTime(2020, 1, day, 16, 49 - offset, 0));

    //    candles.Last().CloseOn.Should().Be(
    //        new DateTime(2020, 1, day, 16, 55 - offset, 59, 999));

    //    feed.RaiseOnCandleAndReset(list.Last().Tick);

    //    candles.Last().OpenOn.Should().Be(
    //        new DateTime(2020, 1, day, 16, 56 - offset, 0));

    //    candles.Last().CloseOn.Should().Be(
    //        new DateTime(2020, 1, day, 16, 59, 59, 999));

    //    candles.Count.Should().Be(extent == Extent.Day ? 206 : 1029);
    //}

    //////////////////////////

    [Theory]
    [InlineData(false, true, 15, ErrorType.ArgumentNullException)]
    [InlineData(true, false, 15, ErrorType.ArgumentNullException)]
    [InlineData(true, true, 0, ErrorType.ArgumentOutOfRangeException)]
    public void ConstructWithBadArgs(
        bool goodPair, bool goodSession, int seconds, ErrorType errorType)
    {
        var pair = goodPair ? Known.Pairs[Symbol.EURUSD] : null!;

        Session session;

        if (goodSession)
            session = new Session(Extent.Day, new DateOnly(2020, 1, 6));
        else
            session = null!;

        FluentActions.Invoking(() => new IntervalFeed(pair, session, seconds))
            .Should().Throw<Exception>().Where(e => e.GetType() == errorType.ToType());
    }

    //////////////////////////

    [Fact]
    public void IntervalFeedShouldCreateExpectedCandles()
    {
        var pair = Known.Pairs[Symbol.EURUSD];

        var tradeDate = new DateOnly(2020, 1, 6);

        var session = new Session(Extent.Day, tradeDate);

        var feed = new IntervalFeed(pair, session, 60 * 7);

        var actual = new List<ICandle>();

        var expected = GetExpectedCandles();

        feed.OnCandle += (s, e) => actual.Add(e.Candle);

        var tickSet = new TickSet(
            Source.Dukascopy, pair, session.TradeDate);

        tickSet.LoadFromStream(
            DC_EURUSD_20200106_EST_STS.ToStream(), DataKind.STS);

        var ticks = tickSet.ToList();

        foreach (var tick in ticks.Take(ticks.Count - 1))
            feed.HandleTick(tick);

        feed.RaiseOnCandleAndReset(ticks.Last());

        var sb = new StringBuilder();

        for (var i = 0; i < actual.Count; i++)
            actual[i].Should().Be(expected[i]);
    }

    //////////////////////////

    private static List<Candle> GetExpectedCandles()
    {
        var candles = new List<Candle>();

        var tradeDate = new DateOnly(2020, 1, 6);

        var session = new Session(Extent.Day, tradeDate);

        void Add(int offset, string openOnString, string closeOnString,
            int open, int high, int low, int close)
        {
            var openOn = tradeDate.AddDays(offset)
                .ToDateTime(TimeOnly.Parse(openOnString));

            var closeOn = tradeDate.AddDays(offset)
                .ToDateTime(TimeOnly.Parse(closeOnString));

            candles.Add(new Candle(session, openOn, closeOn, open, high, low, close));
        }

        Add(-1, "17:00:00.000", "17:06:59.999", 111671, 111679, 111650, 111652);
        Add(-1, "17:07:00.000", "17:13:59.999", 111652, 111653, 111606, 111634);
        Add(-1, "17:14:00.000", "17:20:59.999", 111634, 111637, 111593, 111608);
        Add(-1, "17:21:00.000", "17:27:59.999", 111607, 111634, 111603, 111609);
        Add(-1, "17:28:00.000", "17:34:59.999", 111608, 111610, 111606, 111607);
        Add(-1, "17:35:00.000", "17:41:59.999", 111606, 111619, 111606, 111613);
        Add(-1, "17:42:00.000", "17:48:59.999", 111614, 111615, 111610, 111611);
        Add(-1, "17:49:00.000", "17:55:59.999", 111614, 111614, 111609, 111609);
        Add(-1, "17:56:00.000", "18:02:59.999", 111610, 111618, 111593, 111605);
        Add(-1, "18:03:00.000", "18:09:59.999", 111604, 111643, 111595, 111639);
        Add(-1, "18:10:00.000", "18:16:59.999", 111640, 111647, 111630, 111647);
        Add(-1, "18:17:00.000", "18:23:59.999", 111646, 111677, 111639, 111639);
        Add(-1, "18:24:00.000", "18:30:59.999", 111640, 111678, 111640, 111675);
        Add(-1, "18:31:00.000", "18:37:59.999", 111674, 111675, 111639, 111640);
        Add(-1, "18:38:00.000", "18:44:59.999", 111640, 111666, 111640, 111660);
        Add(-1, "18:45:00.000", "18:51:59.999", 111660, 111660, 111608, 111616);
        Add(-1, "18:52:00.000", "18:58:59.999", 111615, 111625, 111605, 111605);
        Add(-1, "18:59:00.000", "19:05:59.999", 111606, 111648, 111601, 111648);
        Add(-1, "19:06:00.000", "19:12:59.999", 111648, 111672, 111640, 111640);
        Add(-1, "19:13:00.000", "19:19:59.999", 111640, 111657, 111638, 111656);
        Add(-1, "19:20:00.000", "19:26:59.999", 111655, 111682, 111648, 111677);
        Add(-1, "19:27:00.000", "19:33:59.999", 111677, 111680, 111665, 111668);
        Add(-1, "19:34:00.000", "19:40:59.999", 111667, 111686, 111655, 111671);
        Add(-1, "19:41:00.000", "19:47:59.999", 111670, 111681, 111668, 111675);
        Add(-1, "19:48:00.000", "19:54:59.999", 111675, 111679, 111651, 111654);
        Add(-1, "19:55:00.000", "20:01:59.999", 111652, 111652, 111630, 111645);
        Add(-1, "20:02:00.000", "20:08:59.999", 111645, 111655, 111635, 111654);
        Add(-1, "20:09:00.000", "20:15:59.999", 111654, 111654, 111618, 111633);
        Add(-1, "20:16:00.000", "20:22:59.999", 111634, 111640, 111623, 111634);
        Add(-1, "20:23:00.000", "20:29:59.999", 111636, 111640, 111629, 111634);
        Add(-1, "20:30:00.000", "20:36:59.999", 111636, 111640, 111617, 111619);
        Add(-1, "20:37:00.000", "20:43:59.999", 111618, 111628, 111613, 111619);
        Add(-1, "20:44:00.000", "20:50:59.999", 111619, 111639, 111617, 111633);
        Add(-1, "20:51:00.000", "20:57:59.999", 111632, 111639, 111618, 111634);
        Add(-1, "20:58:00.000", "21:04:59.999", 111634, 111645, 111628, 111638);
        Add(-1, "21:05:00.000", "21:11:59.999", 111637, 111644, 111627, 111639);
        Add(-1, "21:12:00.000", "21:18:59.999", 111639, 111642, 111631, 111636);
        Add(-1, "21:19:00.000", "21:25:59.999", 111635, 111643, 111625, 111640);
        Add(-1, "21:26:00.000", "21:32:59.999", 111640, 111667, 111640, 111663);
        Add(-1, "21:33:00.000", "21:39:59.999", 111662, 111672, 111654, 111655);
        Add(-1, "21:40:00.000", "21:46:59.999", 111655, 111666, 111653, 111666);
        Add(-1, "21:47:00.000", "21:53:59.999", 111667, 111667, 111661, 111665);
        Add(-1, "21:54:00.000", "22:00:59.999", 111664, 111665, 111645, 111654);
        Add(-1, "22:01:00.000", "22:07:59.999", 111655, 111679, 111654, 111675);
        Add(-1, "22:08:00.000", "22:14:59.999", 111675, 111682, 111670, 111675);
        Add(-1, "22:15:00.000", "22:21:59.999", 111675, 111696, 111674, 111695);
        Add(-1, "22:22:00.000", "22:28:59.999", 111696, 111707, 111695, 111705);
        Add(-1, "22:29:00.000", "22:35:59.999", 111704, 111710, 111698, 111699);
        Add(-1, "22:36:00.000", "22:42:59.999", 111701, 111703, 111668, 111677);
        Add(-1, "22:43:00.000", "22:49:59.999", 111677, 111691, 111672, 111680);
        Add(-1, "22:50:00.000", "22:56:59.999", 111680, 111686, 111668, 111673);
        Add(-1, "22:57:00.000", "23:03:59.999", 111673, 111676, 111648, 111649);
        Add(-1, "23:04:00.000", "23:10:59.999", 111648, 111663, 111648, 111658);
        Add(-1, "23:11:00.000", "23:17:59.999", 111658, 111662, 111647, 111654);
        Add(-1, "23:18:00.000", "23:24:59.999", 111654, 111658, 111640, 111642);
        Add(-1, "23:25:00.000", "23:31:59.999", 111640, 111643, 111625, 111629);
        Add(-1, "23:32:00.000", "23:38:59.999", 111628, 111638, 111627, 111627);
        Add(-1, "23:39:00.000", "23:45:59.999", 111628, 111631, 111623, 111629);
        Add(-1, "23:46:00.000", "23:52:59.999", 111628, 111631, 111617, 111628);
        Add(-1, "23:53:00.000", "23:59:59.999", 111628, 111645, 111627, 111643);
        Add(0, "00:00:00.000", "00:06:59.999", 111644, 111652, 111642, 111645);
        Add(0, "00:07:00.000", "00:13:59.999", 111645, 111646, 111637, 111645);
        Add(0, "00:14:00.000", "00:20:59.999", 111645, 111647, 111641, 111645);
        Add(0, "00:21:00.000", "00:27:59.999", 111645, 111650, 111644, 111645);
        Add(0, "00:28:00.000", "00:34:59.999", 111646, 111648, 111629, 111634);
        Add(0, "00:35:00.000", "00:41:59.999", 111634, 111639, 111633, 111635);
        Add(0, "00:42:00.000", "00:48:59.999", 111636, 111636, 111623, 111624);
        Add(0, "00:49:00.000", "00:55:59.999", 111625, 111625, 111606, 111618);
        Add(0, "00:56:00.000", "01:02:59.999", 111619, 111625, 111617, 111624);
        Add(0, "01:03:00.000", "01:09:59.999", 111623, 111623, 111604, 111604);
        Add(0, "01:10:00.000", "01:16:59.999", 111604, 111605, 111577, 111579);
        Add(0, "01:17:00.000", "01:23:59.999", 111580, 111588, 111572, 111588);
        Add(0, "01:24:00.000", "01:30:59.999", 111588, 111605, 111583, 111604);
        Add(0, "01:31:00.000", "01:37:59.999", 111603, 111606, 111597, 111598);
        Add(0, "01:38:00.000", "01:44:59.999", 111598, 111599, 111575, 111589);
        Add(0, "01:45:00.000", "01:51:59.999", 111591, 111615, 111589, 111612);
        Add(0, "01:52:00.000", "01:58:59.999", 111613, 111615, 111603, 111604);
        Add(0, "01:59:00.000", "02:05:59.999", 111605, 111636, 111604, 111633);
        Add(0, "02:06:00.000", "02:12:59.999", 111634, 111635, 111610, 111621);
        Add(0, "02:13:00.000", "02:19:59.999", 111622, 111635, 111598, 111607);
        Add(0, "02:20:00.000", "02:26:59.999", 111608, 111650, 111603, 111623);
        Add(0, "02:27:00.000", "02:33:59.999", 111623, 111641, 111615, 111637);
        Add(0, "02:34:00.000", "02:40:59.999", 111636, 111646, 111624, 111644);
        Add(0, "02:41:00.000", "02:47:59.999", 111644, 111661, 111639, 111658);
        Add(0, "02:48:00.000", "02:54:59.999", 111657, 111662, 111651, 111660);
        Add(0, "02:55:00.000", "03:01:59.999", 111660, 111670, 111644, 111659);
        Add(0, "03:02:00.000", "03:08:59.999", 111659, 111670, 111650, 111656);
        Add(0, "03:09:00.000", "03:15:59.999", 111656, 111665, 111633, 111649);
        Add(0, "03:16:00.000", "03:22:59.999", 111648, 111683, 111646, 111683);
        Add(0, "03:23:00.000", "03:29:59.999", 111684, 111699, 111666, 111668);
        Add(0, "03:30:00.000", "03:36:59.999", 111667, 111682, 111653, 111682);
        Add(0, "03:37:00.000", "03:43:59.999", 111682, 111698, 111669, 111694);
        Add(0, "03:44:00.000", "03:50:59.999", 111694, 111723, 111677, 111718);
        Add(0, "03:51:00.000", "03:57:59.999", 111717, 111784, 111706, 111783);
        Add(0, "03:58:00.000", "04:04:59.999", 111782, 111814, 111779, 111805);
        Add(0, "04:05:00.000", "04:11:59.999", 111805, 111809, 111767, 111772);
        Add(0, "04:12:00.000", "04:18:59.999", 111770, 111798, 111768, 111798);
        Add(0, "04:19:00.000", "04:25:59.999", 111798, 111798, 111752, 111764);
        Add(0, "04:26:00.000", "04:32:59.999", 111763, 111819, 111761, 111806);
        Add(0, "04:33:00.000", "04:39:59.999", 111805, 111864, 111794, 111859);
        Add(0, "04:40:00.000", "04:46:59.999", 111858, 111908, 111857, 111903);
        Add(0, "04:47:00.000", "04:53:59.999", 111904, 111910, 111875, 111883);
        Add(0, "04:54:00.000", "05:00:59.999", 111882, 111938, 111877, 111937);
        Add(0, "05:01:00.000", "05:07:59.999", 111936, 111944, 111902, 111902);
        Add(0, "05:08:00.000", "05:14:59.999", 111902, 111954, 111900, 111943);
        Add(0, "05:15:00.000", "05:21:59.999", 111944, 111974, 111932, 111942);
        Add(0, "05:22:00.000", "05:28:59.999", 111942, 111996, 111942, 111964);
        Add(0, "05:29:00.000", "05:35:59.999", 111965, 111969, 111925, 111950);
        Add(0, "05:36:00.000", "05:42:59.999", 111950, 111983, 111947, 111982);
        Add(0, "05:43:00.000", "05:49:59.999", 111981, 111982, 111953, 111962);
        Add(0, "05:50:00.000", "05:56:59.999", 111962, 111978, 111957, 111969);
        Add(0, "05:57:00.000", "06:03:59.999", 111968, 111989, 111958, 111988);
        Add(0, "06:04:00.000", "06:10:59.999", 111989, 112046, 111987, 112032);
        Add(0, "06:11:00.000", "06:17:59.999", 112033, 112054, 112017, 112035);
        Add(0, "06:18:00.000", "06:24:59.999", 112035, 112036, 111990, 112003);
        Add(0, "06:25:00.000", "06:31:59.999", 112002, 112046, 111999, 112046);
        Add(0, "06:32:00.000", "06:38:59.999", 112047, 112056, 112021, 112021);
        Add(0, "06:39:00.000", "06:45:59.999", 112022, 112037, 112014, 112015);
        Add(0, "06:46:00.000", "06:52:59.999", 112016, 112054, 112011, 112048);
        Add(0, "06:53:00.000", "06:59:59.999", 112048, 112053, 112029, 112033);
        Add(0, "07:00:00.000", "07:06:59.999", 112033, 112044, 111995, 112008);
        Add(0, "07:07:00.000", "07:13:59.999", 112009, 112011, 111972, 111974);
        Add(0, "07:14:00.000", "07:20:59.999", 111973, 111979, 111959, 111975);
        Add(0, "07:21:00.000", "07:27:59.999", 111974, 111985, 111969, 111975);
        Add(0, "07:28:00.000", "07:34:59.999", 111973, 111984, 111970, 111977);
        Add(0, "07:35:00.000", "07:41:59.999", 111976, 111983, 111952, 111961);
        Add(0, "07:42:00.000", "07:48:59.999", 111961, 111985, 111961, 111981);
        Add(0, "07:49:00.000", "07:55:59.999", 111981, 111999, 111967, 111983);
        Add(0, "07:56:00.000", "08:02:59.999", 111982, 111998, 111976, 111998);
        Add(0, "08:03:00.000", "08:09:59.999", 111997, 112003, 111972, 111975);
        Add(0, "08:10:00.000", "08:16:59.999", 111977, 111977, 111927, 111928);
        Add(0, "08:17:00.000", "08:23:59.999", 111929, 111961, 111912, 111950);
        Add(0, "08:24:00.000", "08:30:59.999", 111951, 111972, 111946, 111957);
        Add(0, "08:31:00.000", "08:37:59.999", 111956, 111999, 111956, 111988);
        Add(0, "08:38:00.000", "08:44:59.999", 111990, 112033, 111990, 112000);
        Add(0, "08:45:00.000", "08:51:59.999", 111999, 112000, 111964, 111977);
        Add(0, "08:52:00.000", "08:58:59.999", 111977, 111993, 111973, 111977);
        Add(0, "08:59:00.000", "09:05:59.999", 111977, 112003, 111972, 111974);
        Add(0, "09:06:00.000", "09:12:59.999", 111973, 111977, 111952, 111952);
        Add(0, "09:13:00.000", "09:19:59.999", 111951, 111973, 111945, 111959);
        Add(0, "09:20:00.000", "09:26:59.999", 111958, 111961, 111930, 111931);
        Add(0, "09:27:00.000", "09:33:59.999", 111931, 111962, 111930, 111957);
        Add(0, "09:34:00.000", "09:40:59.999", 111956, 111956, 111892, 111895);
        Add(0, "09:41:00.000", "09:47:59.999", 111896, 111900, 111866, 111885);
        Add(0, "09:48:00.000", "09:54:59.999", 111885, 111887, 111839, 111856);
        Add(0, "09:55:00.000", "10:01:59.999", 111856, 111875, 111826, 111845);
        Add(0, "10:02:00.000", "10:08:59.999", 111846, 111882, 111845, 111880);
        Add(0, "10:09:00.000", "10:15:59.999", 111879, 111879, 111838, 111855);
        Add(0, "10:16:00.000", "10:22:59.999", 111854, 111858, 111833, 111838);
        Add(0, "10:23:00.000", "10:29:59.999", 111837, 111881, 111827, 111876);
        Add(0, "10:30:00.000", "10:36:59.999", 111876, 111902, 111865, 111878);
        Add(0, "10:37:00.000", "10:43:59.999", 111878, 111879, 111827, 111848);
        Add(0, "10:44:00.000", "10:50:59.999", 111850, 111857, 111822, 111843);
        Add(0, "10:51:00.000", "10:57:59.999", 111844, 111901, 111837, 111900);
        Add(0, "10:58:00.000", "11:04:59.999", 111901, 111901, 111852, 111853);
        Add(0, "11:05:00.000", "11:11:59.999", 111853, 111861, 111826, 111830);
        Add(0, "11:12:00.000", "11:18:59.999", 111829, 111844, 111810, 111831);
        Add(0, "11:19:00.000", "11:25:59.999", 111832, 111875, 111832, 111838);
        Add(0, "11:26:00.000", "11:32:59.999", 111838, 111879, 111832, 111863);
        Add(0, "11:33:00.000", "11:39:59.999", 111862, 111876, 111857, 111867);
        Add(0, "11:40:00.000", "11:46:59.999", 111866, 111869, 111856, 111862);
        Add(0, "11:47:00.000", "11:53:59.999", 111862, 111873, 111847, 111859);
        Add(0, "11:54:00.000", "12:00:59.999", 111859, 111874, 111851, 111856);
        Add(0, "12:01:00.000", "12:07:59.999", 111855, 111902, 111843, 111897);
        Add(0, "12:08:00.000", "12:14:59.999", 111898, 111937, 111898, 111923);
        Add(0, "12:15:00.000", "12:21:59.999", 111924, 111961, 111921, 111948);
        Add(0, "12:22:00.000", "12:28:59.999", 111950, 111953, 111929, 111930);
        Add(0, "12:29:00.000", "12:35:59.999", 111930, 111931, 111915, 111925);
        Add(0, "12:36:00.000", "12:42:59.999", 111925, 111933, 111916, 111921);
        Add(0, "12:43:00.000", "12:49:59.999", 111922, 111929, 111911, 111915);
        Add(0, "12:50:00.000", "12:56:59.999", 111915, 111930, 111907, 111928);
        Add(0, "12:57:00.000", "13:03:59.999", 111927, 111964, 111925, 111939);
        Add(0, "13:04:00.000", "13:10:59.999", 111939, 111947, 111935, 111946);
        Add(0, "13:11:00.000", "13:17:59.999", 111946, 111949, 111933, 111933);
        Add(0, "13:18:00.000", "13:24:59.999", 111933, 111964, 111933, 111961);
        Add(0, "13:25:00.000", "13:31:59.999", 111961, 111967, 111951, 111956);
        Add(0, "13:32:00.000", "13:38:59.999", 111956, 111964, 111952, 111964);
        Add(0, "13:39:00.000", "13:45:59.999", 111964, 111980, 111962, 111977);
        Add(0, "13:46:00.000", "13:52:59.999", 111978, 111978, 111950, 111952);
        Add(0, "13:53:00.000", "13:59:59.999", 111951, 111953, 111918, 111919);
        Add(0, "14:00:00.000", "14:06:59.999", 111919, 111929, 111914, 111917);
        Add(0, "14:07:00.000", "14:13:59.999", 111917, 111929, 111907, 111912);
        Add(0, "14:14:00.000", "14:20:59.999", 111912, 111921, 111901, 111902);
        Add(0, "14:21:00.000", "14:27:59.999", 111901, 111911, 111901, 111903);
        Add(0, "14:28:00.000", "14:34:59.999", 111902, 111926, 111886, 111888);
        Add(0, "14:35:00.000", "14:41:59.999", 111887, 111889, 111880, 111882);
        Add(0, "14:42:00.000", "14:48:59.999", 111883, 111897, 111880, 111891);
        Add(0, "14:49:00.000", "14:55:59.999", 111892, 111900, 111881, 111892);
        Add(0, "14:56:00.000", "15:02:59.999", 111893, 111922, 111892, 111901);
        Add(0, "15:03:00.000", "15:09:59.999", 111901, 111915, 111899, 111899);
        Add(0, "15:10:00.000", "15:16:59.999", 111899, 111908, 111890, 111907);
        Add(0, "15:17:00.000", "15:23:59.999", 111906, 111913, 111900, 111901);
        Add(0, "15:24:00.000", "15:30:59.999", 111901, 111913, 111899, 111900);
        Add(0, "15:31:00.000", "15:37:59.999", 111898, 111909, 111895, 111895);
        Add(0, "15:38:00.000", "15:44:59.999", 111896, 111908, 111894, 111904);
        Add(0, "15:45:00.000", "15:51:59.999", 111905, 111918, 111894, 111918);
        Add(0, "15:52:00.000", "15:58:59.999", 111918, 111935, 111916, 111922);
        Add(0, "15:59:00.000", "16:05:59.999", 111922, 111939, 111922, 111932);
        Add(0, "16:06:00.000", "16:12:59.999", 111931, 111943, 111930, 111938);
        Add(0, "16:13:00.000", "16:19:59.999", 111937, 111941, 111931, 111937);
        Add(0, "16:20:00.000", "16:26:59.999", 111937, 111956, 111934, 111956);
        Add(0, "16:27:00.000", "16:33:59.999", 111953, 111953, 111946, 111952);
        Add(0, "16:34:00.000", "16:40:59.999", 111952, 111954, 111949, 111954);
        Add(0, "16:41:00.000", "16:47:59.999", 111953, 111991, 111953, 111975);
        Add(0, "16:48:00.000", "16:54:59.999", 111976, 111985, 111973, 111976);
        Add(0, "16:55:00.000", "16:59:59.999", 111976, 111980, 111970, 111973);

        return candles;
    }
}