using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoundItemEntryView : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TextMeshProUGUI itemNameLabel;

    public void Init(BagItemData data)
    {
        itemIcon.sprite = data.Sprite;
        itemNameLabel.text = $"item_{data.Id}";
    }
}
