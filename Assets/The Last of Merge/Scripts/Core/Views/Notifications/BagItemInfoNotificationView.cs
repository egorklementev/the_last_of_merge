using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BagItemInfoNotificationView : ANotificationView
{
    [SerializeField]
    private CanvasGroup group;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TextMeshProUGUI itemTitleLabel;

    [SerializeField]
    private TextMeshProUGUI itemDescrLabel;

    [SerializeField]
    private Button closeButton;

    [Inject]
    private NotificationManager manager;

    public override void Initialize(NotificationData data, params object[] args)
    {
        base.Initialize(data, args);

        group.ToggleAnimated(true);

        var titleText = args[0] as string;
        var sprite = args[1] as Sprite;

        itemIcon.sprite = sprite;
        itemTitleLabel.text = titleText;

        closeButton.onClick.AddListener(() =>
            manager.GetNotifyButtonAction(data.ButtonData[0].ActionId)?.Invoke(this)
        );
    }

    public override void Close()
    {
        base.Close();
        group.ToggleAnimated(false, onComplete: () => Destroy(gameObject));
    }
}
