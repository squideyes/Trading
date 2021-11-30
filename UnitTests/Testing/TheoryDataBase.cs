// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com) 
// 
// This file is part of SquidEyes.Trading
// 
// The use of this source code is licensed under the terms 
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using static System.Globalization.DateTimeStyles;

namespace SquidEyes.UnitTests.Testing;

public abstract class TheoryDataBase : IEnumerable<object[]>
{
    readonly List<object[]> data = new();

    protected void AddRow(params object[] values) => data.Add(values);

    protected static DateTime DT(
        string value, DateTimeKind kind = DateTimeKind.Unspecified)
    {
        if (kind == DateTimeKind.Utc)
            return DateTime.Parse(value, null, AssumeUniversal | AdjustToUniversal);
        else
            return DateTime.Parse(value);
    }

    protected static DateOnly DO(string value) => DateOnly.Parse(value);

    protected static TimeOnly TO(string value) => TimeOnly.Parse(value);

    protected static TimeSpan TS(string value) => TimeSpan.Parse(value);

    public IEnumerator<object[]> GetEnumerator() =>
        data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract class TheoryData<T1> : TheoryDataBase
{
    public void Add(T1 p1) => AddRow(p1!);
}

public abstract class TheoryData<T1, T2> : TheoryDataBase
{
    public void Add(T1 p1, T2 p2) => AddRow(p1!, p2!);
}

public abstract class TheoryData<T1, T2, T3> : TheoryDataBase
{
    public void Add(T1 p1, T2 p2, T3 p3) =>
        AddRow(p1!, p2!, p3!);
}

public abstract class TheoryData<T1, T2, T3, T4> : TheoryDataBase
{
    public void Add(T1 p1, T2 p2, T3 p3, T4 p4) =>
        AddRow(p1!, p2!, p3!, p4!);
}

public abstract class TheoryData<T1, T2, T3, T4, T5> : TheoryDataBase
{
    public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) =>
        AddRow(p1!, p2!, p3!, p4!, p5!);
}

public abstract class TheoryData<T1, T2, T3, T4, T5, T6> : TheoryDataBase
{
    public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) =>
        AddRow(p1!, p2!, p3!, p4!, p5!, p6!);
}