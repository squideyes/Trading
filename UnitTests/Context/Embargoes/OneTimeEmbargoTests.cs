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

//namespace SquidEyes.UnitTests.Context;

//public class OneTimeEmbargoTests
//{
//    private class GoodConstructorArgsData 
//        : Testing.TheoryData<DateTime, DateTime, bool, bool>
//    {
//        public GoodConstructorArgsData()
//        {
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/11/2021 16:59:59.999"), true, true);
//            Add(DT("10/10/2021 17:00:00.001"), DT("10/11/2021 16:59:59.998"), true, true);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/11/2021 16:59:59.999"), true, false);
//            Add(DT("10/10/2021 17:00:00.001"), DT("10/11/2021 16:59:59.998"), true, false);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/11/2021 16:59:59.999"), false, true);
//            Add(DT("10/10/2021 17:00:00.001"), DT("10/11/2021 16:59:59.998"), false, true);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/11/2021 16:59:59.999"), false, false);
//            Add(DT("10/10/2021 17:00:00.001"), DT("10/11/2021 16:59:59.998"), false, false);
//        }
//    }

//    [Theory]
//    [ClassData(typeof(GoodConstructorArgsData))]
//    public void GoodConstructorArgs(
//        string minTickOnString, string maxTickOnString, bool isAdHoc, bool fullSpan)
//    {
//        var minTickOn = DateTime.Parse(minTickOnString);
//        var maxTickOn = DateTime.Parse(maxTickOnString);

//        var embargo = new OneTimeEmbargo(minTickOn, maxTickOn, isAdHoc);

//        embargo.Kind.Should().Be(isAdHoc ? EmbargoKind.AdHoc : EmbargoKind.OneTime);

//        _ = embargo.IsEmbargoed(minTickOn).Should().Be(true);
//        _ = embargo.IsEmbargoed(maxTickOn).Should().Be(true);

//        if (!fullSpan)
//        {
//            _ = embargo.IsEmbargoed(minTickOn.AddMilliseconds(-1)).Should().Be(false);
//            _ = embargo.IsEmbargoed(minTickOn.AddMilliseconds(1)).Should().Be(true);
//            _ = embargo.IsEmbargoed(maxTickOn.AddMilliseconds(-1)).Should().Be(true);
//            _ = embargo.IsEmbargoed(maxTickOn.AddMilliseconds(1)).Should().Be(false);
//        }
//    }

//    //////////////////////////

//    private class BadConstructorArgsData 
//        : Testing.TheoryData<DateTime, DateTime, bool>
//    {
//        public BadConstructorArgsData()
//        {
//            Add(DT("10/10/2021 16:59:59.999"), DT("10/10/2021 17:00:00.000"), true);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/11/2021 17:00:00.000"), true);
//            Add(DT("10/10/2021 17:00:00.001"), DT("10/10/2021 17:00:00.000"), true);
//            Add(DT("10/10/2021 16:59:59.999"), DT("10/10/2021 16:59:59.998"), true);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/12/2021 16:59:59.999"), true);
//            Add(DT("10/10/2021 16:59:59.999"), DT("10/10/2021 17:00:00.000"), false);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/11/2021 17:00:00.000"), false);
//            Add(DT("10/10/2021 17:00:00.001"), DT("10/10/2021 17:00:00.000"), false);
//            Add(DT("10/10/2021 16:59:59.999"), DT("10/10/2021 16:59:59.998"), false);
//            Add(DT("10/10/2021 17:00:00.000"), DT("10/12/2021 16:59:59.999"), false);
//        }
//    }

//    [Theory]
//    [ClassData(typeof(BadConstructorArgsData))]
//    public void BadConstructorArgs(
//        string minTickOnString, string maxTickOnString, bool isAdHoc)
//    {
//        var minTickOn = DateTime.Parse(minTickOnString);
//        var maxTickOn = DateTime.Parse(maxTickOnString);

//        FluentActions.Invoking(
//            () => _ = new OneTimeEmbargo(minTickOn, maxTickOn, isAdHoc))
//                .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    //////////////////////////

//    private class IsEmbargoedWithGoodAndBadArgsData 
//        : Testing.TheoryData<DateTime, bool>
//    {
//        public IsEmbargoedWithGoodAndBadArgsData()
//        {
//            Add(DT("12/25/2021 17:00:00.000"), false);
//            Add(DT("10/10/2021 17:59:59.999"), false);
//            Add(DT("10/10/2021 18:00:00.000"), true);
//            Add(DT("10/10/2021 18:59:59.999"), true);
//            Add(DT("10/10/2021 19:00:00.000"), false);
//        }
//    }

//    [Theory]
//    [ClassData(typeof(IsEmbargoedWithGoodAndBadArgsData))]
//    public void IsEmbargoedWithGoodAndBadArgs(DateTime tickOn, bool result) =>
//       GetEmbargo().IsEmbargoed(tickOn).Should().Be(result);

//    //////////////////////////

//    [Fact]
//    public void ToStringReturnsExpectValue()
//    {
//        GetEmbargo().ToString().Should().Be(
//            "Embargo (OneTime; 10/10/2021 18:00:00.000 to 10/10/2021 18:59:59.999)");
//    }

//    //////////////////////////

//    private static OneTimeEmbargo GetEmbargo()
//    {
//        var minTickOn = new DateTime(2021, 10, 10, 18, 0, 0);
//        var maxTickOn = new DateTime(2021, 10, 10, 18, 59, 59, 999);

//        return new OneTimeEmbargo(minTickOn, maxTickOn);
//    }
//}