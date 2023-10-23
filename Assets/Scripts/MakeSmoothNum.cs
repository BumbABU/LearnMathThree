using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSmoothNum : MonoBehaviour
{
    public static void EaseOut(float t)
    {
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
    }

    public static void EaseIn(float t)
    {
        t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
    }

    public static void SmoothStep(float t)
    {
        t = t * t * (3 - 2 * t);
    }

    public static void SmootherStep(float t)
    {
        t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
    }
}
