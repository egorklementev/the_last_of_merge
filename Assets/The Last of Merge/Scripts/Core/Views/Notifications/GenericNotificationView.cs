using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GenericNotificationView : ANotificationView
{
    [SerializeField]
    private CanvasGroup group;

    [SerializeField]
    private Image image;

    [SerializeField]
    private TextMeshProUGUI textLabel;

    [SerializeField]
    private List<Button> buttons;

    [SerializeField]
    private List<TextMeshProUGUI> buttonLabels;

    [Inject]
    private NotificationManager notificationManager;

    public override void Initialize(NotificationData data, params object[] args)
    {
        base.Initialize(data, args);

        var sprite = data.Sprite;
        var text = data.TextKey;
        var buttonTexts = data.ButtonData.Select(d => d.ButtonKey).ToList();
        var buttonActions = data
            .ButtonData.Select(d => notificationManager.GetNotifyButtonAction(d.ActionId))
            .ToList();

        Reset();

        var imageAndText = sprite != null && text != string.Empty;
        var justImage = sprite != null && text == string.Empty;
        var justText = sprite == null && text != string.Empty;

        if (imageAndText)
        {
            image.gameObject.SetActive(true);
            image.rectTransform.anchorMin = new Vector2(.075f, .5f);
            image.rectTransform.anchorMax = new Vector2(.925f, .95f);

            textLabel.gameObject.SetActive(true);
            textLabel.rectTransform.anchorMin = new Vector2(.075f, .15f);
            textLabel.rectTransform.anchorMax = new Vector2(.925f, .45f);
        }
        else if (justImage)
        {
            image.gameObject.SetActive(true);
            image.rectTransform.anchorMin = new Vector2(.075f, .15f);
            image.rectTransform.anchorMax = new Vector2(.925f, .95f);
        }
        else if (justText)
        {
            textLabel.gameObject.SetActive(true);
            textLabel.rectTransform.anchorMin = new Vector2(.075f, .15f);
            textLabel.rectTransform.anchorMax = new Vector2(.925f, .95f);
        }

        image.sprite = sprite;
        textLabel.text = text;

        if (buttonTexts.Count != buttonActions.Count)
        {
            Debug.LogError("Wow, you messed up badly with notification creation!");
            return;
        }

        var count = buttonTexts.Count;
        for (int i = 0; i < count; i++)
        {
            int iCopy = i;
            buttons[i].gameObject.SetActive(true);
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => buttonActions[iCopy](this));
            buttonLabels[i].text = buttonTexts[i];
        }

        group.ToggleAnimated(true);
    }

    public override void Close()
    {
        base.Close();

        group.ToggleAnimated(false, onComplete: () => Destroy(gameObject));
    }

    private void Reset()
    {
        image.sprite = null;
        image.gameObject.SetActive(false);

        textLabel.text = string.Empty;
        textLabel.gameObject.SetActive(false);

        foreach (var btn in buttons)
        {
            btn.gameObject.SetActive(false);
        }

        foreach (var btnLbl in buttonLabels)
        {
            btnLbl.text = string.Empty;
        }
    }
}
