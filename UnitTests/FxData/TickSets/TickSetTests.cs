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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;
using Xunit.Categories;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.FxData;

public class TickSetTests
{
    [Fact]
    public void RoundtripThroughStream()
    {
        TickSet.Version.Should().Be(new MajorMinor(3, 0));

        var (pair, baseDate) = GetPairAndBaseDate();

        var csv = GetTickSet(DataKind.CSV);

        var stream = new MemoryStream();

        csv.SaveToStream(stream, DataKind.STS);

        stream.Position = 0;

        var sts = new TickSet(Source.Dukascopy, pair, baseDate);

        sts.LoadFromStream(stream, DataKind.STS);

        CsvEqualsSts(csv, sts);
    }

    ////////////////////////////

    [Fact]
    public void ClearDoesIndeedClear()
    {
        var tickSet = GetTickSet(DataKind.CSV);

        tickSet.Count.Should().Be(77046);

        tickSet.Clear();

        tickSet.Count.Should().Be(0);

        tickSet.Add(GetTick(tickSet, 0));

        tickSet.Count.Should().Be(1);

        tickSet.Clear();

        tickSet.Count.Should().Be(0);
    }

    ////////////////////////////

    [Fact]
    public void AddRangeWithGoodArgs()
    {
        var tickSet = new TickSet(Source.Dukascopy,
            Known.Pairs[Symbol.EURUSD], Known.MinTradeDate);

        var tick1 = GetTick(tickSet, 0);
        var tick2 = GetTick(tickSet, 1);
        var tick3 = GetTick(tickSet, 2);

        tickSet.AddRange(new List<Tick> { tick1, tick2, tick3 });

        tickSet.Count.Should().Be(3);

        tickSet[0].Should().Be(tick1);
        tickSet[1].Should().Be(tick2);
        tickSet[2].Should().Be(tick3);
    }

    ////////////////////////////

    [Fact]
    public void AddRangeWithEmptyList()
    {
        var tickSet = new TickSet(Source.Dukascopy,
            Known.Pairs[Symbol.EURUSD], Known.MinTradeDate);

        tickSet.AddRange(new List<Tick> { });

        tickSet.Count.Should().Be(0);
    }

    ////////////////////////////

    [Fact]
    [IntegrationTest]
    public void CsvAndStsTickSetsEqualThroughFile()
    {
        var csv = GetTickSet(DataKind.CSV);

        var sts = new TickSet(Source.Dukascopy, csv.Pair, csv.Session.TradeDate);

        var fileName = Path.GetTempFileName();

        using (var stream = File.OpenWrite(fileName))
            csv.SaveToStream(stream, DataKind.STS);

        using (var stream = File.OpenRead(fileName))
            sts.LoadFromStream(stream, DataKind.STS);

        CsvEqualsSts(csv, sts);
    }

    [Fact]
    public void CsvAndStsTickSetsEqual()
    {
        var csv = GetTickSet(DataKind.CSV);
        var sts = GetTickSet(DataKind.STS);

        CsvEqualsSts(csv, sts);
    }

    ////////////////////////////

    [Fact]
    public void ToStringReturnsGetFileNameResult()
    {
        var tickSet = GetTickSet(DataKind.STS);

        tickSet.ToString().Should().Be(tickSet.GetFileName(DataKind.STS));
    }

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV)]
    [InlineData(DataKind.STS)]
    public void GetMetadataReturnsExpectedValues(DataKind dataKind)
    {
        static DateTime ParseCreatedOn(string value)
        {
            if (value == null)
                return default;
            else
                return DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
        }

        var tickSet = GetTickSet(dataKind);

        var metaData = tickSet.GetMetadata(dataKind);

        var createdOn = ParseCreatedOn(metaData["CreatedOn"]);

        createdOn.Should().NotBe(default);
        createdOn.Kind.Should().Be(DateTimeKind.Utc);

        metaData["Count"].Should().Be(tickSet.Count.ToString());
        metaData["Extent"].Should().Be(tickSet.Session.Extent.ToString());
        metaData["Pair"].Should().Be(tickSet.Pair.ToString());
        metaData["SaveAs"].Should().Be(dataKind.ToString());
        metaData["Source"].Should().Be(tickSet.Source.ToString());
        metaData["TradeDate"].Should().Be(tickSet.Session.TradeDate.ToString());
        metaData["Version"].Should().Be(TickSet.Version.ToString());
    }

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV, "DC_EURUSD_20200106_EST.csv")]
    [InlineData(DataKind.STS, "DC_EURUSD_20200106_EST.sts")]
    public void GoodFileNameGenerated(DataKind dataKind, string fileName) =>
        GetTickSet(dataKind).GetFileName(dataKind).Should().Be(fileName);

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV, "DC/TICKSETS/EURUSD/2020/DC_EURUSD_20200106_EST.csv")]
    [InlineData(DataKind.STS, "DC/TICKSETS/EURUSD/2020/DC_EURUSD_20200106_EST.sts")]
    public void GoodBlobNameGenerated(DataKind dataKind, string blobName) =>
        GetTickSet(dataKind).GetBlobName(dataKind).Should().Be(blobName);

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV, @"C:\DC\TICKSETS\EURUSD\2020\DC_EURUSD_20200106_EST.csv")]
    [InlineData(DataKind.STS, @"C:\DC\TICKSETS\EURUSD\2020\DC_EURUSD_20200106_EST.sts")]
    public void GoodFullPathGenerated(DataKind dataKind, string fullPath) =>
        GetTickSet(dataKind).GetFullPath("C:\\", dataKind).Should().Be(fullPath);

    ////////////////////////////

    [Theory]
    [InlineData("DC_EURUSD_20200106_EST.csv")]
    [InlineData("DC_EURUSD_20200106_EST.sts")]
    public void GoodFileNamesParsedWithoutError(string fileName)
    {
        var tickSet = TickSet.Create(fileName);

        tickSet.Count.Should().Be(0);
        tickSet.Pair.Should().Be(Known.Pairs[Symbol.EURUSD]);
        tickSet.Session.TradeDate.Should().Be(new DateOnly(2020, 1, 6));
        tickSet.Source.Should().Be(Source.Dukascopy);
    }

    ////////////////////////////

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("DC_EURUSD_20200106_EST.xxx")]
    [InlineData("EURUSD_20200106_EST.csv")]
    [InlineData("DC_20200106_EST.csv")]
    [InlineData("DC_EURUSD_EST.csv")]
    [InlineData("DC_EURUSD_20200106.csv")]
    [InlineData("XX_EURUSD_20200106_EST..sts")]
    [InlineData("DC_XXXXXX_20200106_EST..sts")]
    [InlineData("DC_EURUSD_20200105_EST..sts")]
    [InlineData("DC_EURUSD_XXXXXXXX_EST..sts")]
    [InlineData("DC_EURUSD_20200106_XXX..sts")]
    public void CreateWithBadArgsThrowsError(string fileName)
    {
        FluentActions.Invoking(() => TickSet.Create(fileName))
            .Should().Throw<Exception>();
    }

    ////////////////////////////

    [Fact]
    public void AddOutOfTickOnOrder()
    {
        var (pair, tradeDate) = GetPairAndBaseDate();

        var session = new Session(Extent.Day, tradeDate);

        Tick GetTick(int milliseconds, int bid, int ask)
        {
            return new Tick(((DateTime)session!.MinTickOn)
                .AddSeconds(milliseconds), new Rate(bid), new Rate(ask));
        }

        var tickSet = new TickSet(Source.Dukascopy, pair, tradeDate)
        {
            GetTick(1, 1, 2)
        };

        FluentActions.Invoking(() => tickSet.Add(GetTick(0, 1, 2)))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    private static Tick GetTick(TickSet tickSet, int msOffset)
    {
        var tickOn = new TickOn(
            tickSet.Session.MinTickOn.Value.AddMilliseconds(msOffset));

        var bid = new Rate(tickSet.Pair.MinValue, 5);

        var ask = new Rate(tickSet.Pair.MaxValue, 5);

        return new Tick(tickOn, bid, ask);
    }

    private static void CsvEqualsSts(TickSet csv, TickSet sts)
    {
        csv.Count.Should().Be(sts.Count);
        csv.Pair.Should().Be(sts.Pair);
        csv.Session.Should().Be(sts.Session);
        csv.Source.Should().Be(sts.Source);

        for (var i = 0; i < csv.Count; i++)
            csv[i].Should().Be(sts[i]);
    }

    private static (Pair, DateOnly) GetPairAndBaseDate() =>
        (Known.Pairs[Symbol.EURUSD], new DateOnly(2020, 1, 6));

    private static TickSet GetTickSet(DataKind dataKind)
    {
        var (pair, baseDate) = GetPairAndBaseDate();

        var tickSet = new TickSet(Source.Dukascopy, pair, baseDate);

        if (dataKind == DataKind.CSV)
        {
            foreach (var fields in new CsvEnumerator(
                DC_EURUSD_20200106_EST_CSV.ToStream(), 3))
            {
                var tickOn = DateTime.Parse(fields[0]);
                var bid = new Rate(float.Parse(fields[1]), pair.Digits);
                var ask = new Rate(float.Parse(fields[2]), pair.Digits);

                tickSet.Add(new Tick(tickOn, bid, ask));
            }
        }
        else
        {
            tickSet.LoadFromStream(
                DC_EURUSD_20200106_EST_STS.ToStream(), DataKind.STS);
        }

        return tickSet;
    }
}