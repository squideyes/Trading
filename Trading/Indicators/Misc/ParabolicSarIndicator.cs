using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;

namespace SquidEyes.Trading.Indicators;

public class ParabolicSarIndicator : BasicIndicatorBase, IBasicIndicator
{
    private const int PERIOD = 4;

    private readonly SlidingBuffer<ICandle> candles = new(PERIOD, true);
    private readonly SlidingBuffer<double> values = new(PERIOD, true);

    private readonly MinMaxStep acceleration;
    private double af = 0.0;
    private bool afIncreased = false;
    private bool isLong = false;
    private int prevBar = 0;
    private double prevSar = 0.0;
    private int reverseBar = 0;
    private double reverseValue = 0.0;
    private double todaysSar = 0.0;
    private double xp = 0.0;
    private int currentBar = -1;

    public ParabolicSarIndicator(
        Pair pair, RateToUse rateToUse, MinMaxStep acceleration)
        : base(PERIOD, pair, rateToUse)
    {
        this.acceleration = acceleration;
    }

    public bool IsPrimed => candles.IsPrimed;

    public BasicResult AddAndCalc(ICandle candle)
    {
        ArgumentNullException.ThrowIfNull(candle);

        candles.Add(candle);

        currentBar++;

        values.Add(0.0);

        if (currentBar < 3)
            return GetBasicResult(candle.OpenOn, values[0]);

        if (currentBar == 3)
        {
            var max = AsDouble(candles.Select(c => c.High).Max());
            var min = AsDouble(candles.Select(c => c.Low).Min());

            isLong = candle.High > candles[1].High;
            xp = isLong ? max : min;
            af = acceleration.Value;

            var value = xp + (isLong ? -1 : 1) * ((max - min) * af);

            values.Update(value);

            return GetBasicResult(candle.OpenOn, value);
        }

        if (afIncreased && prevBar != currentBar)
            afIncreased = false;

        var low0 = AsDouble(candles[0].Low);
        var low1 = AsDouble(candles[1].Low);
        var high0 = AsDouble(candles[0].High);
        var high1 = AsDouble(candles[1].High);

        if (reverseBar != currentBar)
        {
            todaysSar = GetTodaysSar(
                values[1] + af * (xp - values[1]), low0, low1, high0, high1);

            for (var i = 1; i <= 2; i++)
            {
                if (isLong)
                {
                    AsDouble(candles[i].Low).DoIfCanDo(
                        r => todaysSar > r, r => todaysSar = r);
                }
                else
                {
                    AsDouble(candles[i].High).DoIfCanDo(
                        r => todaysSar < r, r => todaysSar = r);
                }
            }

            if (isLong)
            {
                if (prevBar != currentBar || low0 < prevSar)
                    values.Update(prevSar = todaysSar);
                else
                    values.Update(prevSar);

                if (high0 > xp)
                {
                    xp = high0;

                    AfIncrease();
                }
            }
            else
            {
                if (prevBar != currentBar || high0 > prevSar)
                    values.Update(prevSar = todaysSar);
                else
                    values.Update(prevSar);

                if (low0 < xp)
                {
                    xp = low0;

                    AfIncrease();
                }
            }
        }
        else
        {
            if (isLong && high0 > xp)
                xp = high0;
            else if (!isLong && low0 < xp)
                xp = low0;

            values.Update(prevSar);

            todaysSar = GetTodaysSar(isLong ? Math.Min(reverseValue, low0)
                : Math.Max(reverseValue, high0), low0, low1, high0, high1);
        }

        prevBar = currentBar;

        if ((isLong && (low0 < todaysSar || low1 < todaysSar))
            || (!isLong && (high0 > todaysSar || high1 > todaysSar)))
        {
            values.Update(Reverse(low0, high0));
        }

        return GetBasicResult(candle.OpenOn, values[0]);
    }

    private double Reverse(double low0, double high0)
    {
        var todaySar = xp;

        if ((isLong && prevSar > low0) || (!isLong && prevSar < high0) || prevBar != currentBar)
        {
            isLong = !isLong;
            reverseBar = currentBar;
            reverseValue = xp;
            af = acceleration.Value;
            prevSar = todaySar;
            xp = isLong ? high0 : low0;
        }
        else
        {
            todaySar = prevSar;
        }

        return todaySar;
    }

    private void AfIncrease()
    {
        if (!afIncreased)
        {
            af = Math.Min(acceleration.Maximum, af + acceleration.Step);

            afIncreased = true;
        }
    }

    private double GetTodaysSar(
        double todaySar, double low0, double low1, double high0, double high1)
    {
        if (isLong)
        {
            var lowestSar = Math.Min(Math.Min(todaySar, low0), low1);

            if (low0 > lowestSar)
                todaySar = lowestSar;
        }
        else
        {
            var highestSar = Math.Max(Math.Max(todaySar, high0), high1);

            if (high0 < highestSar)
                todaySar = highestSar;
        }

        return todaySar;
    }

    private double AsDouble(Rate rate) => Math.Round(rate.AsFloat(Pair.Digits), 5);
}