// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

//using SquidEyes.Basics;
//using SquidEyes.Trading.Context;

//namespace SquidEyes.Trading.FxData;

//public class Bundle : ListBase<TickSet>
//{
//    public Bundle(Source source, Pair pair, int year, int month)
//    {
//        Source = source.Validated(nameof(source), v => v.IsEnumValue());

//        Pair = pair.Validated(nameof(pair),
//            v => Known.Pairs.ContainsKey(v.Symbol));

//        Year = year = year.Validated(nameof(year), v => v >= 2000);

//        Month = month.Validated(nameof(month), v => v.Between(1, 12));
//    }

//    public Source Source { get; }
//    public Pair Pair { get; }
//    public int Year { get; }
//    public int Month { get; }

//    public void Add(TickSet tickSet)
//    {
//        if (tickSet.Source != Source)
//            throw new ArgumentOutOfRangeException(nameof(tickSet));

//        if (tickSet.Pair != Pair)
//            throw new ArgumentOutOfRangeException(nameof(tickSet));

//        if (tickSet.Session.TradeDate.Year != Year)
//            throw new ArgumentOutOfRangeException(nameof(tickSet));

//        if (tickSet.Session.TradeDate.Month != Month)
//            throw new ArgumentOutOfRangeException(nameof(tickSet));

//        if (Count > 0 && tickSet.Session.TradeDate <= Last().Session.TradeDate)
//            throw new ArgumentOutOfRangeException(nameof(tickSet));

//        Items.Add(tickSet);
//    }

//    public Dictionary<string, string> GetMetadata()
//    {
//        return new Dictionary<string, string>
//        {
//            { "CreatedOn", DateTime.UtcNow.ToTickOnText() },
//            { "Source", Source.ToString() },
//            { "Pair", Pair.ToString() },
//            { "MonthYear", $"{Month:00}/{Year}" },
//            { "TickSets", Count.ToString() }
//        };
//    }

//    public override string ToString() => FileName;

//    public string BlobName =>
//        $"{Source.ToCode()}/BUNDLES/{Pair}/{Year}/{FileName}";

//    public string FileName =>
//        $"{Source.ToCode()}_{Pair}_{Year}_{Month:00}_EST.bundle";
//}
