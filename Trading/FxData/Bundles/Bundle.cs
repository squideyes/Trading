// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using System.IO.Compression;
using System.Text;

namespace SquidEyes.Trading.FxData;

public class Bundle : ListBase<TickSet>
{
    public static readonly MajorMinor Version = new(1, 0);

    public Bundle(Source source, Pair pair, int year, int month)
    {
        Source = source.Validated(nameof(source), v => v.IsEnumValue());

        Pair = pair.Validated(
            nameof(pair), v => Known.Pairs.ContainsKey(v.Symbol));

        Year = year = year.Validated(nameof(year), v => v >= Known.MinYear);

        Month = month.Validated(nameof(month), v => v.Between(1, 12));
    }

    public Source Source { get; }
    public Pair Pair { get; }
    public int Year { get; }
    public int Month { get; }

    public TickSet this[int index] => Items[index];

    public void Add(TickSet tickSet)
    {
        ArgumentNullException.ThrowIfNull(tickSet);

        if (tickSet.Count == 0)
            throw new InvalidOperationException("An empty tick-set may not be bundled!");

        if (tickSet.Source != Source)
            throw new ArgumentOutOfRangeException(nameof(tickSet));

        if (tickSet.Pair != Pair)
            throw new ArgumentOutOfRangeException(nameof(tickSet));

        if (tickSet.Session.TradeDate.Year != Year)
            throw new ArgumentOutOfRangeException(nameof(tickSet));

        if (tickSet.Session.TradeDate.Month != Month)
            throw new ArgumentOutOfRangeException(nameof(tickSet));

        if (Count > 0 && tickSet.Session.TradeDate <= Last().Session.TradeDate)
            throw new ArgumentOutOfRangeException(nameof(tickSet));

        Items.Add(tickSet);
    }

    public bool IsComplete()
    {
        var tradeDates = Items.Select(i => i.Session.TradeDate).ToHashSet();

        foreach (var tradeDate in Known.GetTradeDates(Year, Month))
        {
            if (!tradeDates.Contains(tradeDate))
                return false;
        }

        return true;
    }

    public Dictionary<string, string> GetMetadata()
    {
        return new Dictionary<string, string>
        {
            { "CreatedOn", DateTime.UtcNow.ToTickOnText() },
            { "Source", Source.ToString() },
            { "Pair", Pair.ToString() },
            { "Year", Year.ToString() },
            { "Month", Month.ToString() },
            { "Days", GetDays() }
        };
    }

    private string GetDays() => string.Join(",",
        Items.Select(i => i.Session.TradeDate.Day.ToString()));

    public override string ToString() => FileName;

    public string BlobName =>
        $"{Source.ToCode()}/BUNDLES/{Pair}/{Year}/{FileName}";

    public string FileName =>
        $"{Source.ToCode()}_{Pair}_{Year}_{Month:00}_EST.stsb";

    public string GetFullPath(string basePath)
    {
        if (!basePath.IsFolderName())
            throw new ArgumentOutOfRangeException(nameof(basePath));

        var sb = new StringBuilder();

        sb.Append(Source.ToCode());
        sb.AppendDelimited("BUNDLES", Path.DirectorySeparatorChar);
        sb.AppendDelimited(Pair, Path.DirectorySeparatorChar);
        sb.AppendDelimited(Year, Path.DirectorySeparatorChar);
        sb.AppendDelimited(FileName, Path.DirectorySeparatorChar);

        return Path.Combine(basePath, sb.ToString());
    }

    public void SaveToStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);

        foreach (var tickSet in Items)
        {
            var fileName = tickSet.GetFileName(DataKind.STS);

            var entry = archive.CreateEntry(fileName);

            using var entryStream = entry.Open();

            tickSet.SaveToStream(entryStream, DataKind.STS);
        }
    }

    public void LoadFromStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var entries = archive.Entries;

        foreach (var entry in entries)
        {
            var entryStream = entry.Open();

            var tickSet = TickSet.Create(entry.Name);

            tickSet.LoadFromStream(entryStream, DataKind.STS);

            Add(tickSet);
        }
    }
}