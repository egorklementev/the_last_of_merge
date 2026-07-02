using System.Collections.Generic;
using System.Linq;
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
    private TextMeshProUGUI itemTitle;

    [SerializeField]
    private Transform recipeVariantsHolder;

    [SerializeField]
    private RecipeVariantEntryView recipeVariantPrefab;

    [Inject]
    private IInstantiator instantiator;

    public void FillListWithItems(IList<MergeRecipe> recipes)
    {
        foreach (var child in recipeListContent)
        {
            // KISS: if many recipes, change to pooling + canvas group
            Destroy((child as Transform).gameObject);
        }

        // Convert recipes list to unique item => multiple recipes
        var itemToRecipes = new Dictionary<BagItemData, List<MergeRecipe>>();
        var uniqueItems = recipes.Select(r => r.ResultingItem).Distinct();
        foreach (var item in uniqueItems)
        {
            itemToRecipes.Add(item, recipes.Where(r => r.ResultingItem.Id == item.Id).ToList());
        }

        foreach (var kvp in itemToRecipes)
        {
            var item = kvp.Key;
            var recipeList = kvp.Value;

            var view = instantiator.InstantiatePrefabForComponent<RecipeListEntryView>(
                entryViewPrefab,
                recipeListContent
            );

            view.Init(item);
            view.Clicked += () =>
            {
                foreach (var child in recipeVariantsHolder)
                {
                    // KISS: if many recipes, change to pooling + canvas group
                    Destroy((child as Transform).gameObject);
                }

                foreach (var recipe in recipeList)
                {
                    var recipeVariant =
                        instantiator.InstantiatePrefabForComponent<RecipeVariantEntryView>(
                            recipeVariantPrefab,
                            recipeVariantsHolder
                        );

                    recipeVariant.SetRecipe(recipe);
                }

                itemTitle.text = $"item_{item.Id}";
            };
        }
    }
}
