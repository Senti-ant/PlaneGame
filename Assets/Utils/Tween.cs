using System;
using System.Collections;
using UnityEngine;

public static class Tween
{
    //Basic tweening function.
    //Example usage: StartCoroutine( Tween.Routine(Tween.InvLinear, x => color.alpha = x, 3f) );
    public static IEnumerator Routine<T>(Func<float, T> lerper, Action<T> setAction, float duration)
    {
        float startTime = Time.time;

        setAction( lerper(0f) );
        for (float progress = 0f; progress < 1f; progress = (Time.time - startTime) / duration)
        {
            setAction( lerper(progress) );
            yield return null;
        }
        setAction( lerper(1f) );
    }

    //Some lerpers that are ready to use (all return a value within 0..1).
    public static float Linear(float x) => x;
    public static float InvLinear(float x) => 1f - x;
    public static float Sigmoid(float x)
    {
        if (x <= 0.5f)
        {
            float x3 = x*x*x;
            return 4*x3;
        }
        else
        {
            float invx3 = (1f - x)*(1f - x)*(1f - x);
            return 1f - 4f*invx3;
        }
    }
    public static float InvSigmoid(float x) => 1f - Sigmoid(x);
    public static float Accelerating(float x) => x*x*x;
    public static float InvAccelerating(float x) => 1f - x*x*x;
    public static float Decelerating(float x) => 1f - ((1f-x) * (1f-x) * (1f-x));
    public static float InvDecelerating(float x) => (1f-x) * (1f-x) * (1f-x);

    //Helper function helps you convert from 0..1 to whatever range you actually want.
    //Example usage: 
    // StartCoroutine( Tween.Routine(Tween.WithRange(-10, 10, Tween.Sigmoid), SetSomething, 9f) );
    public static Func<float, float> WithRange(float min, float max, Func<float, float> lerper)
    {
        return x => Mathf.Lerp(min, max, lerper(x));
    }

    //Helper functions for using other data types than floats.
    public static Func<float, Vector2> With2DRange(Vector2 min, Vector2 max, Func<float, float> lerper)
    {
        return x => Vector2.Lerp(min, max, lerper(x));
    }
    public static Func<float, Vector3> With3DRange(Vector3 min, Vector3 max, Func<float, float> lerper)
    {
        return x => Vector3.Lerp(min, max, lerper(x));
    }
    public static Func<float, Color> WithColorRange(Color min, Color max, Func<float, float> lerper)
    {
        return x => Color.Lerp(min, max, lerper(x));
    }
    public static Func<float, Quaternion> WithRotationRange(Quaternion min, Quaternion max, Func<float, float> lerper)
    {
        return x => Quaternion.Lerp(min, max, lerper(x));
    }
}
