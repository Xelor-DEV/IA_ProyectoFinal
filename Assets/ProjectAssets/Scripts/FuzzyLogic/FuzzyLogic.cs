using UnityEngine;
using System;

[Serializable]
public class FuzzyFunction
{
    [SerializeField] private string name;
    [SerializeField] private AnimationCurve functionCurve;
    [SerializeField] private float y;
    [SerializeField] private float singletonValue;

    public AnimationCurve FunctionCurve
    {
        get
        {
            return functionCurve;
        }
        set
        {
            functionCurve = value;
        }
    }

    public float Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }

    public float SingletonValue
    {
        get
        {
            return singletonValue;
        }
        set
        {
            singletonValue = value;
        }
    }

    public float Evaluate(float x)
    {
        y = 0;
        if (x >= functionCurve.keys[0].time)
        {
            y += Mathf.Clamp01(functionCurve.Evaluate(x));
        }
        return y;
    }
}

[Serializable]
public class CalculateFuzzy
{
    [SerializeField] private FuzzyFunction[] membershipFunctions;

    public float CalculateFuzzyValue(float distance)
    {
        float sumW = 0;
        float multW = 0;

        for (int i = 0; i < membershipFunctions.Length; ++i)
        {
            float y = membershipFunctions[i].Evaluate(distance);
            sumW += y;
            multW += y * membershipFunctions[i].SingletonValue;
        }

        if (sumW != 0)
        {
            return multW / sumW;
        }
        else
        {
            return multW;
        }
    }
}