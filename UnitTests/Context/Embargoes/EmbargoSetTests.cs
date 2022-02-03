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
using System;
using Xunit;
using static SquidEyes.Trading.Context.EmbargoKind;

namespace SquidEyes.UnitTests.Context;

public class EmbargoSetTests
{
    private class MultipleEmbargoesHonoredData 
        : Testing.TheoryData<DateTime, EmbargoKind?>
    {
        public MultipleEmbargoesHonoredData()
        {
            Add(DT("10/10/2021 17:00:00.000"), Open);
            Add(DT("10/10/2021 17:14:59.599"), Open);
            Add(DT("10/10/2021 17:15:00.000"), null);
            Add(DT("10/10/2021 17:59:59.000"), null);
            Add(DT("10/10/2021 18:00:00.000"), Offset);
            Add(DT("10/10/2021 18:59:59.999"), Offset);
            Add(DT("10/10/2021 19:00:00.000"), null);
            Add(DT("10/10/2021 19:59:59.999"), null);
            Add(DT("10/10/2021 20:00:00.000"), AdHoc);
            Add(DT("10/10/2021 20:59:59.999"), AdHoc);
            Add(DT("10/10/2021 21:00:00.000"), null);
            Add(DT("10/10/2021 21:59:59.999"), null);
            Add(DT("10/10/2021 22:00:00.000"), OneTime);
            Add(DT("10/10/2021 22:59:59.999"), OneTime);
        }
    }

    [Theory]
    [ClassData(typeof(MultipleEmbargoesHonoredData))]
    public void MultipleEmbargoesHonored(TickOn tickOn, EmbargoKind? kind)
    {
        //var embargoes = new EmbargoSet
        //    {
        //        new PeggedEmbargo(PegTo.Open, 15, DayOfWeek.Sunday),
        //        new PeggedEmbargo(PegTo.Close, 30, DayOfWeek.Sunday)
        //    };

        //var minOffset = TimeSpan.FromHours(1);

        //var maxOffset = TimeSpan.FromHours(2)
        //    .Add(TimeSpan.FromMilliseconds(-1));

        //embargoes.Add(new OffsetEmbargo(
        //    minOffset, maxOffset, DayOfWeek.Sunday));

        //embargoes.Add(new OneTimeEmbargo(
        //    new DateTime(2021, 10, 10, 20, 0, 0),
        //    new DateTime(2021, 10, 10, 20, 59, 59, 999), true));

        //embargoes.Add(new OneTimeEmbargo(
        //    new DateTime(2021, 10, 10, 22, 0, 0),
        //    new DateTime(2021, 10, 10, 22, 59, 59, 999), false));

        //var (isEmbargoed, embargo) = embargoes.IsEmbargoed(tickOn);

        //isEmbargoed.Should().Be(kind.HasValue);

        //if (isEmbargoed)
        //    embargo!.Kind.Should().Be(kind);
    }
}