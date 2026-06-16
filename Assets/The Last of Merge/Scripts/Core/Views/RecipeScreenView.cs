using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RecipeScreenView : MonoBehaviour, IRecipeScreenView
{
    [SerializeField]
    private RectTransform recipeListContent;

    [SerializeField]
    private RecipeListEntryView entryViewPrefab;

    [SerializeField]
    private Image item1Icon;

    [SerializeField]
    private Image item2Icon;

    [SerializeField]
    private Image itemResultIcon;

    [SerializeField]
    private TextMeshProUGUI itemTitle;

    [Inject]
    private IInstantiator instantiator;

    public void FillListWithItems(IList<MergeRecipe> recipes)
    {
        foreach (var child in recipeListContent)
        {
            // KISS: if many recipes, change to pooling + canvas group
            Destroy((child as Transform).gameObject);
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            var view = instantiator.InstantiatePrefabForComponent<RecipeListEntryView>(
                entryViewPrefab,
                recipeListContent
            );

            var iCopy = i;
            view.Init(recipes[i].ResultingItem);
            view.Clicked += () =>
            {
                item1Icon.color = Color.white;
                item2Icon.color = Color.white;
                itemResultIcon.color = Color.white;

                var recipe = recipes[iCopy];
                item1Icon.sprite = recipe.Item1.Sprite;
                item2Icon.sprite = recipe.Item2.Sprite;
                itemResultIcon.sprite = recipe.ResultingItem.Sprite;
                itemTitle.text = $"item_{recipe.ResultingItem.Id}";
            };
        }
    }
}
