using UnityEngine;
using UnityEngine.UI;

public class RecipeVariantEntryView : MonoBehaviour
{
    [SerializeField]
    private Image item1Icon;

    [SerializeField]
    private Image item2Icon;

    [SerializeField]
    private Image resultingItemIcon;

    public void SetRecipe(MergeRecipe recipe)
    {
        item1Icon.sprite = recipe.Item1.Sprite;
        item2Icon.sprite = recipe.Item2.Sprite;
        resultingItemIcon.sprite = recipe.ResultingItem.Sprite;
    }
}
