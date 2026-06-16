using System;
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

    public void Init(BagItemData data)
    {
        itemIcon.color = Color.white;
        itemIcon.sprite = data.Sprite;
        itemTitle.text = $"item_{data.Id}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke();
    }
}
