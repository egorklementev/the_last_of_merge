using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class NotificationManager : IInitializable
{
    [Inject]
    private NotificationContainerView containerView;

    private Dictionary<string, Action<INotificationView>> actionMap = new();

    private IList<NotificationData> templates;

    public void Initialize()
    {
        RegisterNotifyButtonAction("close", DefaultNotificationClose);

        UniTask.Void(async () =>
        {
            templates = await Addressables.LoadAssetsAsync<NotificationData>(
                "notification_template"
            );
        });
    }

    public async UniTask ShowNotification(string notificationId, params object[] args)
    {
        await UniTask.WaitUntil(() => templates != null);

        var data = templates.SingleOrDefault(t => t.Id == notificationId);
        if (data == null)
        {
            Debug.LogError($"No template with {notificationId} can be found!");
            return;
        }

        await ShowNotification(data, args);
    }

    public async UniTask ShowNotification(NotificationData data, params object[] args)
    {
        await containerView.ShowNotification(data, args);
    }

    public void RegisterNotifyButtonAction(string actionId, Action<INotificationView> action) =>
        actionMap.Add(actionId, action);

    public Action<INotificationView> GetNotifyButtonAction(string actionId)
    {
        if (!actionMap.ContainsKey(actionId))
            throw new UnityException($"No '{actionId}' button action found!");

        return actionMap[actionId];
    }

    private void DefaultNotificationClose(INotificationView view)
    {
        containerView.CloseNotification(view);
    }
}
