// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.Trading
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

//using SquidEyes.Trading.Context;
//namespace SquidEyes.Trading.FxData
//{
//    public class TdRenkoFeed : ICandleFeed
//    {
//        private readonly Rate brickSize;

//        private bool firstTick = true;

//        private Rate open;
//        private Rate max;
//        private Rate min;

//        public event EventHandler<CandleArgs>? OnCandle;

//        public TdRenkoFeed(Pair pair, Rate brickSize)
//        {
//            Pair = pair;

//            if (brickSize == default)
//                throw new ArgumentOutOfRangeException(nameof(brickSize));

//            this.brickSize = brickSize;
//        }

//        public Pair Pair { get; }

//        public void HandleTick(Tick tick)
//        {
//            if (firstTick)
//            {
//                firstTick = false;

//                open = tick.Mid;
//                min = new Rate(open.Value - brickSize.Value);
//                max = new Rate(open.Value + brickSize.Value);
//            }
//            else
//            {
//                //double _open = bars.GetOpen(bars.Count - 1);
//                //double _high = bars.GetHigh(bars.Count - 1);
//                //double _low = bars.GetLow(bars.Count - 1);
//                //double _close = bars.GetClose(bars.Count - 1);

//                //maxExceeded = bars.Instrument.MasterInstrument.Compare(close, barMax) > 0 ? true : false;
//                //minExceeded = bars.Instrument.MasterInstrument.Compare(close, barMin) < 0 ? true : false;

//                ////### Defined Range Exceeded?
//                //if (maxExceeded || minExceeded)
//                //{
//                //    //### Close Current Bar
//                //    double h = maxExceeded ? barMax : _high;
//                //    double l = minExceeded ? barMin : _low;
//                //    double c = maxExceeded ? barMax : barMin;
//                //    UpdateBar(bars, h, l, c, time, volume);

//                //    //### Add New Bar
//                //    double bodyH = Math.Max(_open, c);
//                //    double bodyL = Math.Min(_open, c);

//                //    barOpen = Rnd((bodyH + bodyL) / 2, tickSize);
//                //    barMax = barOpen + brickSize;
//                //    barMin = barOpen - brickSize;

//                //    AddBar(bars, barOpen, Math.Max(barOpen, close), Math.Min(barOpen, close), c, time, volume, bid, ask);
//                //}
//                ////### Current Bar Still Developing
//                //else
//                //    UpdateBar(bars, (close > _high ? close : _high), (close < _low ? close : _low), close, time, volume);
//            }
//        }


//        //double barOpen;
//        //double barMax;
//        //double barMin;
//        //double fakeOpen = 0;

//        //int barDirection = 0;
//        //double brickSize = 0;

//        //bool maxExceeded = false;
//        //bool minExceeded = false;

//        //double tickSize = 0.01;

//        //protected override void OnStateChange()
//        //{
//        //    if (State == State.SetDefaults)
//        //    {
//        //        Description = @"";
//        //        Name = "tdRenkoBar";
//        //        BarsPeriod = new BarsPeriod { BarsPeriodType = (BarsPeriodType)876154, Value = 1 };
//        //        BuiltFrom = BarsPeriodType.Tick;
//        //        DaysToLoad = 2;
//        //        IsIntraday = true;
//        //    }
//        //    else if (State == State.Configure)
//        //    {
//        //        Properties.Remove(Properties.Find("BaseBarsPeriodType", true));
//        //        Properties.Remove(Properties.Find("BaseBarsPeriodValue", true));
//        //        Properties.Remove(Properties.Find("PointAndFigurePriceType", true));
//        //        Properties.Remove(Properties.Find("ReversalType", true));
//        //        Properties.Remove(Properties.Find("Value2", true));

//        //        SetPropertyName("Value", "Brick Size");
//        //    }
//        //}

//        //public override int GetInitialLookBackDays(BarsPeriod barsPeriod, TradingHours tradingHours, int barsBack)
//        //{
//        //    return 3;
//        //}

//        //protected override void OnDataPoint(Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isBar, double bid, double ask)
//        //{
//        //    if (SessionIterator == null)
//        //        SessionIterator = new SessionIterator(bars);

//        //    bool isNewSession = SessionIterator.IsNewSession(time, isBar);
//        //    if (isNewSession)
//        //        SessionIterator.GetNextSession(time, isBar);

//        //    tickSize = bars.Instrument.MasterInstrument.TickSize;
//        //    brickSize = bars.BarsPeriod.Value * tickSize;

//        //    //### First Bar
//        //    if ((bars.Count == 0) || (bars.IsResetOnNewTradingDay && isNewSession))
//        //    {
//        //        barOpen = close;
//        //        barMax = barOpen + brickSize;
//        //        barMin = barOpen - brickSize;

//        //        AddBar(bars, barOpen, barOpen, barOpen, barOpen, time, volume, bid, ask);
//        //    }
//        //    //### Subsequent Bars
//        //    else
//        //    {
//        //        double _open = bars.GetOpen(bars.Count - 1);
//        //        double _high = bars.GetHigh(bars.Count - 1);
//        //        double _low = bars.GetLow(bars.Count - 1);
//        //        double _close = bars.GetClose(bars.Count - 1);

//        //        maxExceeded = bars.Instrument.MasterInstrument.Compare(close, barMax) > 0 ? true : false;
//        //        minExceeded = bars.Instrument.MasterInstrument.Compare(close, barMin) < 0 ? true : false;

//        //        //### Defined Range Exceeded?
//        //        if (maxExceeded || minExceeded)
//        //        {
//        //            //### Close Current Bar
//        //            double h = maxExceeded ? barMax : _high;
//        //            double l = minExceeded ? barMin : _low;
//        //            double c = maxExceeded ? barMax : barMin;
//        //            UpdateBar(bars, h, l, c, time, volume);

//        //            //### Add New Bar
//        //            double bodyH = Math.Max(_open, c);
//        //            double bodyL = Math.Min(_open, c);

//        //            barOpen = Rnd((bodyH + bodyL) / 2, tickSize);
//        //            barMax = barOpen + brickSize;
//        //            barMin = barOpen - brickSize;

//        //            AddBar(bars, barOpen, Math.Max(barOpen, close), Math.Min(barOpen, close), c, time, volume, bid, ask);
//        //        }
//        //        //### Current Bar Still Developing
//        //        else
//        //            UpdateBar(bars, (close > _high ? close : _high), (close < _low ? close : _low), close, time, volume);
//        //    }

//        //    bars.LastPrice = close;
//        //}

//        //public override void ApplyDefaultBasePeriodValue(BarsPeriod period)
//        //{

//        //}

//        //public override void ApplyDefaultValue(BarsPeriod period)
//        //{
//        //    period.Value = 4;
//        //}

//        //public override string ChartLabel(DateTime time) { return time.ToString("T", Core.Globals.GeneralOptions.CurrentCulture); }

//        //public override double GetPercentComplete(Bars bars, DateTime now)
//        //{
//        //    return 0;
//        //}

//        //public override bool IsRemoveLastBarSupported { get { return false; } }

//        //private double Rnd(double price, double tick)
//        //{
//        //    double mod = price % tick;
//        //    return mod > 0.01 ? Math.Ceiling(price / tick) * tick : Math.Round(price / tick) * tick;
//        //}
//    }
//}