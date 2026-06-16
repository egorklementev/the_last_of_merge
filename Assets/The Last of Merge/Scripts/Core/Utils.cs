using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        var count = 0;
        T result = default;
        foreach (var entry in enumerable)
        {
            count++;
            if (Random.Range(0, count) == 0)
                result = entry;
        }

        return result;
    }
}
