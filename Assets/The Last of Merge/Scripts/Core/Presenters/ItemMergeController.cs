using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Checks whether two given items can be merged or not.
/// </summary>
public class ItemMergeController : IInitializable
{
    [Inject]
    private IMergeRecipeProvider mergeRecipeProvider;

    private IList<MergeRecipe> recipes;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            recipes = await mergeRecipeProvider.GetRecipes();
        });
    }

    public async UniTask<(bool canBeMerged, BagItemData mergeResult)> CanBeMerged(
        BagItemData item1,
        BagItemData item2
    )
    {
        await UniTask.WaitWhile(() => recipes == null);

        var noItem = new BagItemData() { Id = -1 };

        // Putting an item in the same slot => no merging
        var isDifferentSlots = item1.SlotId != item2.SlotId;
        if (!isDifferentSlots)
            return (false, noItem);

        // Putting an item in empty slot => no merging
        if (item1.IsEmpty() || item2.IsEmpty())
            return (false, noItem);

        // No recipe found => no merging
        var recipe = GetRecipeForItems(item1, item2);
        if (recipe == null)
            return (false, noItem);

        // Now, we can merge
        return (
            true,
            new() // TODO: find a way of getting this from provider or something
            {
                Id = recipe.ResultingItemId,
                Color = Color.cyan,
                SlotId = item2.SlotId,
            }
        );
    }

    private MergeRecipe GetRecipeForItems(BagItemData item1, BagItemData item2)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            var recipe = recipes[i];

            // ATTENTION: order below is not important, so there are two checks, not one
            if (recipe.ItemId1 == item1.Id && recipe.ItemId2 == item2.Id)
                return recipe;

            if (recipe.ItemId1 == item2.Id && recipe.ItemId2 == item1.Id)
                return recipe;
        }

        return null;
    }
}
