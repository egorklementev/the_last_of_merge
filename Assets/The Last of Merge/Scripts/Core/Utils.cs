using System;
using System.Collections.Generic;
using DG.Tweening;
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
            if (UnityEngine.Random.Range(0, count) == 0)
                result = entry;
        }

        return result;
    }

    public static void Toggle(this CanvasGroup group, bool isEnabled)
    {
        group.alpha = isEnabled ? 1f : 0f;
        group.blocksRaycasts = isEnabled;
        group.interactable = isEnabled;
    }

    public static void ToggleAnimated(
        this CanvasGroup group,
        bool isEnabled,
        float duration = .444f,
        Action onComplete = null
    )
    {
        if (isEnabled)
        {
            group.blocksRaycasts = isEnabled;
            group.interactable = isEnabled;
        }

        group
            .DOFade(isEnabled ? 1f : 0f, duration)
            .OnComplete(() =>
            {
                group.blocksRaycasts = isEnabled;
                group.interactable = isEnabled;
                onComplete?.Invoke();
            });
    }
}
