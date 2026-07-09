using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Zenject;

public class NotificationManager : IInitializable
{
    [Inject]
    private NotificationContainerView containerView;

    private Dictionary<string, Action<object>> actionMap = new();
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
            return;

        await ShowNotification(data, args);
    }

    public async UniTask ShowNotification(NotificationData data, params object[] args)
    {
        // TODO: make sure text is localized
        var langCode = "en";
        var localizedText =
            data.TextKey == "none"
                ? data.TranslationData.Single(d => d.LanguageCode == langCode).Translation
                : data.TextKey;
        // String.Format(...);

        var btnTexts = new List<string>();
        var btnActs = new List<Action<object>>();
        foreach (var btnData in data.ButtonData)
        {
            btnTexts.Add(btnData.ButtonKey); // TODO: translate this
            btnActs.Add(actionMap[btnData.ActionId]); // TODO: validate this
        }

        containerView.ShowNotification(data.Sprite, localizedText, btnTexts, btnActs);
    }

    public void RegisterNotifyButtonAction(string actionId, Action<object> action) =>
        actionMap.Add(actionId, action);

    private void DefaultNotificationClose(object param)
    {
        if (param is not NotificationView view)
            return;

        containerView.CloseNotification(view);
    }
}
