using UnityEngine;
using System.Collections;

public static class FloatExtensions
{
    public static float WrapFloatToRange(this float f, float min, float max)
    {
        float result = f;
        float difference = max - min;

        while (result < min || result > max)
        {
            if (result < min)
                result += difference;
            else if (result > max)
                result -= difference;
        }

        return result;
    }

    public static float NormInverse(this float f)
    {
        return 1 - f;
    }

    public static float WrapFloatTo01(this float f)
    {
        return f.WrapFloatToRange(0, 1);
    }

    // alternative to Mathf.Lerp() - if value lies within epsilon of targ, then targ is returned
    public static float Lerp(this float src, float targ, float t, float epsilon)
    {
        if (t <= 0.0f)
            return src;

        if (t >= 1.0f)
            return targ;

        float result = src + ((targ - src) * t);

        if (Mathf.Abs(targ - result) <= epsilon)
            return targ;

        return result;
    }



    public static float Scale(this float val, float fromMin, float fromMax, float toMin, float toMax)
    {
        float fromRange = fromMax - fromMin;
        float toRange = toMax - toMin;

        if (fromRange == 0)
            return 0;

        float fromNormalizedValue = Mathf.Clamp01((val - fromMin) / fromRange);
        float scaledVal = toMin + ((toRange) * fromNormalizedValue);
        return Mathf.Clamp(scaledVal, toMin, toMax);    // Clamp value to toRange
    }

    public static float ScaleTo01(this float val, float min, float max)
    {
        float range = max - min;
        if (range == 0)
            return 0;
        else
            return Mathf.Clamp01((val - min) / range);
    }

    public static float ScaleTo01Abs(this float val, float min, float max)
    {
        float range = max - min;
        if (range == 0)
            return 0;
        else
            return Mathf.Clamp01(Mathf.Abs((val - min) / range));
    }

    public static float ScaleFrom01(this float normaliedVal, float min, float max)
    {
        UnityEngine.Profiling.Profiler.BeginSample("scaling");

        float val;
        
        if (min > max)
        {
            float temp = max;
            max = min;
            min = temp;
            normaliedVal = 1 - normaliedVal;
        }        

        val = min + ((max - min) * normaliedVal);

        UnityEngine.Profiling.Profiler.EndSample();
        return val;
    }

    public static float ScaleFrom01Clamped(this float normaliedVal, float min, float max)
    {
        float val;
        if (min > max)
        {
            float temp = max;
            max = min;
            min = temp;
            normaliedVal = 1 - normaliedVal;
        }


        val = min + ((max - min) * normaliedVal);
        val = Mathf.Clamp(val, min, max);
        return val;
    }

    public static float MirrorNormalized(this float normaliedVal)
    {
        if (normaliedVal > .5f)
        {
            normaliedVal = 1 - normaliedVal;
        }
        else
            normaliedVal *= 2;


        return normaliedVal;
    }


    public static string ToDoubleDecimalString(this float val)
    {
        string decimalString;
        decimalString = val.ToString("#0.00");
        return decimalString;
    }

    public static float RoundToOneDecimalPlace(this float val)
    {
        return Mathf.Round(val * 10) / 10;
    }

    public static float RoundToTwoDecimalPlaces(this float val)
    {
        return Mathf.Round(val * 100) / 100;
    }

    public static float GetValueFromNormPosInArray( float[] array, float norm )
    {
        norm *= array.Length;

        int lowerIndex = (int)Mathf.Floor(norm);
        int upperIndex = Mathf.Clamp( lowerIndex + 1, lowerIndex, array.Length - 1);
        float lerp = norm % 1;

        return Mathf.Lerp(array[lowerIndex], array[upperIndex], lerp);
    }

}