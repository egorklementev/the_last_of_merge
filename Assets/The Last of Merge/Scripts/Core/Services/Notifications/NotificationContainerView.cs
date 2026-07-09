using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class NotificationContainerView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup group;

    [SerializeField]
    private NotificationView viewPrefab;

    [Inject]
    private IInstantiator instantiator;

    private List<NotificationView> viewPool = new();

    public void ShowNotification(
        Sprite sprite,
        string text,
        IList<string> buttonTexts,
        IList<Action<object>> buttonActions
    )
    {
        if (ShownNotificationsCount() == 0)
            group.ToggleAnimated(true);

        var view = GetOrCreateNotifyView();
        view.Initialize(sprite, text, buttonTexts, buttonActions);
    }

    public void CloseNotification(NotificationView view)
    {
        view.Close(() =>
        {
            if (ShownNotificationsCount() == 0)
                group.ToggleAnimated(false);
        });
    }

    private NotificationView GetOrCreateNotifyView()
    {
        var freeView = viewPool.FirstOrDefault(view => !view.IsShown);
        if (freeView != null)
            return freeView;

        var newView = instantiator.InstantiatePrefabForComponent<NotificationView>(
            viewPrefab,
            transform
        );

        viewPool.Add(newView);

        return newView;
    }

    private int ShownNotificationsCount() => viewPool.Count(view => view.IsShown);
}
