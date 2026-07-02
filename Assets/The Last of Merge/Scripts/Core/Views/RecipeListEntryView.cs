using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeListEntryView : MonoBehaviour, IPointerClickHandler
{
    public event Action Clicked;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TextMeshProUGUI itemTitle;

    private Tween iconColorTween;
    private Tween iconSizeTween;

    public void Init(BagItemData data)
    {
        itemIcon.color = Color.white;
        itemIcon.sprite = data.Sprite;
        itemTitle.text = $"item_{data.Id}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        iconSizeTween.Kill();
        iconColorTween.Kill();

        itemIcon.rectTransform.localScale = 1.25f * Vector3.one;
        iconSizeTween = itemIcon.rectTransform.DOScale(Vector3.one, .666f);

        itemIcon.color = Color.goldenRod;
        iconColorTween = itemIcon.DOColor(Color.white, .333f);

        Clicked?.Invoke();
    }
}
