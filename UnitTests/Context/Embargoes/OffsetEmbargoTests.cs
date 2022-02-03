// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

//using FluentAssertions;
//using SquidEyes.Quark.Shared.Context;
//using System;
//using Xunit;
//using SquidEyes.Basics;
//using static System.DayOfWeek;
//using static SquidEyes.Quark.Shared.Context.Extent;
//using SquidEyes.UnitTests.Context.Testing;

//namespace SquidEyes.UnitTests.Context;

//public class OffsetEmbargoTests
//{
//    private class GoodConstructorArgsData
//        : Testing.TheoryData<TimeSpan, TimeSpan, DayOfWeek?, bool>
//    {
//        public GoodConstructorArgsData()
//        {
//            Add(TS("00:00:00.000"), TS("23:59:59.999"), Monday, true);
//            Add(TS("00:00:00.001"), TS("23:59:59.998"), Monday, false);
//            Add(TS("00:00:00.000"), TS("23:59:59.999"), null, true);
//            Add(TS("00:00:00.001"), TS("23:59:59.998"), null, false);
//            Add(TS("00:00:00.000"), TS("23:59:59.999"), Monday, true);
//            Add(TS("00:00:00.001"), TS("23:59:59.998"), Monday, false);
//            Add(TS("00:00:00.000"), TS("23:59:59.999"), null, true);
//            Add(TS("00:00:00.001"), TS("23:59:59.998"), null, false);
//        }
//    }

//    [Theory]
//    [ClassData(typeof(GoodConstructorArgsData))]
//    public void GoodConstructorArgs(TimeSpan minOffset,
//        TimeSpan maxOffset, bool fullSpan)
//    {
//        void Validate(Session session, DayOfWeek dayOfWeek)
//        {
//            var (minTickOn, maxTickOn) = session.MinTickOn.AddDays((int)dayOfWeek - 1)
//                .AsFunc(d => (d.Add(minOffset), d.Add(maxOffset)));

//            var embargo = new OffsetEmbargo(minOffset, maxOffset, dayOfWeek);

//            embargo.Kind.Should().Be(EmbargoKind.Offset);

//            _ = embargo.IsEmbargoed(minTickOn).Should().Be(true);
//            _ = embargo.IsEmbargoed(maxTickOn).Should().Be(true);

//            if (!fullSpan)
//            {
//                _ = embargo.IsEmbargoed(minTickOn.AddMilliseconds(-1)).Should().Be(false);
//                _ = embargo.IsEmbargoed(minTickOn.AddMilliseconds(1)).Should().Be(true);
//                _ = embargo.IsEmbargoed(maxTickOn.AddMilliseconds(-1)).Should().Be(true);
//                _ = embargo.IsEmbargoed(maxTickOn.AddMilliseconds(1)).Should().Be(false);
//            }
//        }

//        var tradeDate = new DateOnly(2020, 1, 6);

//        //if (dayOfWeek != null)
//        //{
//        //    Validate(new Session(estOrUtc == EST ?
//        //        EstDay : UtcDay, tradeDate), dayOfWeek.Value);
//        //}
//        //else
//        //{
//        //    var session = new Session(estOrUtc == EST ?
//        //        EstWeek : UtcWeek, tradeDate);

//        //    for (var dow = Monday; dow <= Friday; dow += 1)
//        //        Validate(session, dow);
//        //}
//    }

//    ////////////////////////////

//    //private class BadConstructorArgsData
//    //    : Testing.TheoryData<TimeSpan, TimeSpan, DayOfWeek?>
//    //{
//    //    public BadConstructorArgsData()
//    //    {
//    //        Add(TS("-00:00:00.001"), TS("23:59:59.999"), Sunday);
//    //        Add(TS("1.00:00:00.000"), TS("1.00:00:00.001"), Sunday);
//    //        Add(TS("00:00:00.001"), TS("00:00:00.000"), Sunday);
//    //        Add(TS("00:00:00.000"), TS("1.00:00:00.000"), Sunday);
//    //        Add(TS("00:00:00.000"), TS("23:59:59.999"), (DayOfWeek)10);
//    //        Add(TS("00:00:00.000"), TS("23:59:59.999"), Friday);
//    //    }
//    //}

//    //[Theory]
//    //[ClassData(typeof(BadConstructorArgsData))]
//    //public void BadContructorArgs(
//    //    TimeSpan minOffset, TimeSpan maxOffset, DayOfWeek? dayOfWeek)
//    //{
//    //    FluentActions.Invoking(() =>
//    //        _ = new OffsetEmbargo(minOffset, maxOffset, dayOfWeek))
//    //            .Should().Throw<ArgumentOutOfRangeException>();
//    //}

//    ////////////////////////////

//    //[Theory]
//    //[InlineData(Friday)]
//    //[InlineData(Saturday)]
//    //public void BadDayOfWeek(DayOfWeek dayOfWeek)
//    //{
//    //    FluentActions.Invoking(() => _ = GetEmbargo(dayOfWeek))
//    //        .Should().Throw<ArgumentOutOfRangeException>();
//    //}

//    ////////////////////////////

//    //private class IsEmbargoedWithGoodAndBadArgsData
//    //    : Testing.TheoryData<DayOfWeek?, DateTime, bool>
//    //{
//    //    public IsEmbargoedWithGoodAndBadArgsData()
//    //    {
//    //        Add(Sunday, DT("12/25/2021 17:00:00.000"), false);
//    //        Add(Sunday, DT("10/10/2021 17:59:59.999"), false);
//    //        Add(Sunday, DT("10/10/2021 18:00:00.000"), true);
//    //        Add(Sunday, DT("10/10/2021 18:59:59.999"), true);
//    //        Add(Sunday, DT("10/10/2021 19:00:00.000"), false);
//    //        Add(null, DT("10/10/2021 17:59:59.999"), false);
//    //        Add(null, DT("10/10/2021 18:00:00.000"), true);
//    //        Add(null, DT("10/10/2021 18:59:59.999"), true);
//    //        Add(null, DT("10/10/2021 19:00:00.000"), false);
//    //        Add(null, DT("10/11/2021 17:59:59.999"), false);
//    //        Add(null, DT("10/11/2021 18:00:00.000"), true);
//    //        Add(null, DT("10/11/2021 18:59:59.999"), true);
//    //        Add(null, DT("10/11/2021 19:00:00.000"), false);
//    //        Add(null, DT("10/12/2021 17:59:59.999"), false);
//    //        Add(null, DT("10/12/2021 18:00:00.000"), true);
//    //        Add(null, DT("10/12/2021 18:59:59.999"), true);
//    //        Add(null, DT("10/12/2021 19:00:00.000"), false);
//    //        Add(null, DT("10/13/2021 17:59:59.999"), false);
//    //        Add(null, DT("10/13/2021 18:00:00.000"), true);
//    //        Add(null, DT("10/13/2021 18:59:59.999"), true);
//    //        Add(null, DT("10/13/2021 19:00:00.000"), false);
//    //        Add(null, DT("10/14/2021 17:59:59.999"), false);
//    //        Add(null, DT("10/14/2021 18:00:00.000"), true);
//    //        Add(null, DT("10/14/2021 18:59:59.999"), true);
//    //        Add(null, DT("10/14/2021 19:00:00.000"), false);
//    //    }
//    //}

//    //[Theory]
//    //[ClassData(typeof(IsEmbargoedWithGoodAndBadArgsData))]
//    //public void IsEmbargoedWithGoodAndBadArgs(
//    //    DayOfWeek? dayOfWeek, DateTime tickOn, bool result)
//    //{
//    //    GetEmbargo(dayOfWeek).IsEmbargoed(tickOn).Should().Be(result);
//    //}

//    ////////////////////////////

//    //[Fact]
//    //public void GoodToString()
//    //{
//    //    GetEmbargo(Sunday).ToString().Should().Be(
//    //        "Embargo (Offset; 01:00:00.000 to 01:59:59.999; Sunday)");
//    //}

//    ////////////////////////////

//    //private static OffsetEmbargo GetEmbargo(DayOfWeek? dayOfWeek)
//    //{
//    //    var minOffset = new TimeSpan(1, 0, 0);
//    //    var maxOffset = new TimeSpan(0, 1, 59, 59, 999);

//    //    return new OffsetEmbargo(minOffset, maxOffset, dayOfWeek);
//    //}
//}