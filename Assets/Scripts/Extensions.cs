using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = newLayer;
        }
    }
    
    public static float StandardDeviation(this IEnumerable<double> values)
    {
        double avg = values.Average();
        return (float)Math.Sqrt(values.Average(v=>Math.Pow(v-avg,2)));
    }
    public static Vector3 NormalizeVector(this Vector3 vec, float maxValue)
    {
        return Vector3.Max(Vector3.one * -1, Vector3.Min(Vector3.one, vec / maxValue));
    }

    public static float SafeTarget(this ArticulationDrive drive)
    {
        var driveTarget = drive.target;
        if (driveTarget >= drive.upperLimit) driveTarget -= 0.1f;
        if (driveTarget <= drive.lowerLimit) driveTarget += 0.1f;
        return driveTarget;
    }
        
    public static TSource MinBy<TSource, TValue>(
        this IEnumerable<TSource> source, Func<TSource, TValue> selector) {
        using (var iter = source.GetEnumerator())
        {
            if (!iter.MoveNext()) throw new InvalidOperationException("no data");
            var comparer = Comparer<TValue>.Default;
            var minItem = iter.Current;
            var minValue = selector(minItem);
            while (iter.MoveNext())
            {
                var item = iter.Current;
                var value = selector(item);
                if (comparer.Compare(minValue, value) > 0)
                {
                    minItem = item;
                    minValue = value;
                }
            }
            return minItem;
        }
    }   
}