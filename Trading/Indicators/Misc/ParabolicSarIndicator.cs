using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using SquidEyes.Trading.Shared.Helpers;

namespace SquidEyes.Trading.Indicators;

public class ParabolicSarIndicator : BasicIndicatorBase, IBasicIndicator
{
    private const int PERIOD = 4;

    private readonly SlidingBuffer<ICandle> candles = new(PERIOD, true);
    private readonly SlidingBuffer<double> values = new(PERIOD, true);

    private readonly MinMaxStep acceleration;
    private double af = 0.0;
    private bool afIncreased = false;
    private bool longPosition = false;
    private int prevBar = 0;
    private double prevSar = 0.0;
    private int reverseBar = 0;
    private double reverseValue = 0.0;
    private double todaySar = 0.0;
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

            longPosition = candle.High > candles[1].High;
            xp = longPosition ? max : min;
            af = acceleration.Value;

            var value = xp + (longPosition ? -1 : 1) * ((max - min) * af);

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
            todaySar = GetTodaysSar(values[1] + af * (xp - values[1]));

            for (var x = 1; x <= 2; x++)
            {
                if (longPosition)
                {
                    if (todaySar > AsDouble(candles[x].Low))
                        todaySar = AsDouble(candles[x].Low);
                }
                else
                {
                    if (todaySar < AsDouble(candles[x].High))
                        todaySar = AsDouble(candles[x].High);
                }
            }

            if (longPosition)
            {
                if (prevBar != currentBar || low0 < prevSar)
                {
                    values.Update(todaySar);

                    prevSar = todaySar;
                }
                else
                {
                    values.Update(prevSar);
                }

                if (high0 > xp)
                {
                    xp = high0;

                    AfIncrease();
                }
            }
            else if (!longPosition)
            {
                if (prevBar != currentBar || high0 > prevSar)
                {
                    values.Update(todaySar);

                    prevSar = todaySar;
                }
                else
                {
                    values.Update(prevSar);
                }

                if (low0 < xp)
                {
                    xp = low0;

                    AfIncrease();
                }
            }
        }
        else
        {
            if (longPosition && high0 > xp)
                xp = high0;
            else if (!longPosition && low0 < xp)
                xp = low0;

            values.Update(prevSar);

            todaySar = GetTodaysSar(longPosition
                ? Math.Min(reverseValue, low0)
                : Math.Max(reverseValue, high0));
        }

        prevBar = currentBar;

        if ((longPosition && (low0 < todaySar || low1 < todaySar))
            || (!longPosition && (high0 > todaySar || high1 > todaySar)))
        {
            values.Update(Reverse());
        }

        return GetBasicResult(candle.OpenOn, values[0]);
    }

    private double Reverse()
    {
        double todaySar = xp;

        if ((longPosition && prevSar > AsDouble(candles[0].Low))
            || (!longPosition && prevSar < AsDouble(candles[0].High))
            || prevBar != currentBar)
        {
            longPosition = !longPosition;
            reverseBar = currentBar;
            reverseValue = xp;
            af = acceleration.Value;
            xp = longPosition ? AsDouble(candles[0].High) : AsDouble(candles[0].Low);
            prevSar = todaySar;
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

    private double GetTodaysSar(double todaySar)
    {
        if (longPosition)
        {
            var lowestSar = Math.Min(Math.Min(todaySar,
                AsDouble(candles[0].Low)), AsDouble(candles[1].Low));

            if (AsDouble(candles[0].Low) > lowestSar)
                todaySar = lowestSar;
        }
        else
        {
            var highestSar = Math.Max(Math.Max(todaySar,
                AsDouble(candles[0].High)), AsDouble(candles[1].High));

            if (AsDouble(candles[0].High) < highestSar)
                todaySar = highestSar;
        }

        return todaySar;
    }

    private double AsDouble(Rate rate) => Math.Round(rate.AsFloat(Pair.Digits), 5);
}