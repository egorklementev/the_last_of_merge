using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class NotificationContainerView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup group;

    [Inject]
    private NotificationViewProvider viewProvider;

    [Inject]
    private IInstantiator instantiator;

    private Queue<INotificationView> openedViews = new();

    public async UniTask ShowNotification(NotificationData data, params object[] args)
    {
        var viewTemplate = viewProvider.GetView(data);
        var view = instantiator.InstantiatePrefabForComponent<ANotificationView>(
            viewTemplate,
            transform
        );

        view.Initialize(data, args);
        openedViews.Enqueue(view);
        group.ToggleAnimated(true);
    }

    public void CloseNotification(INotificationView view)
    {
        view.Close();
        openedViews.Dequeue();

        if (openedViews.Count == 0)
        {
            group.ToggleAnimated(false);
        }
    }
}
