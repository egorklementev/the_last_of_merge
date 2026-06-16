using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;

/// <summary>
/// Checks whether two given items can be merged or not.
/// </summary>
public class ItemMergeController : IInitializable
{
    [Inject]
    private IMergeRecipeProvider mergeRecipeProvider;

    [Inject]
    private IBagItemsProvider bagItemsProvider;

    private IList<MergeRecipe> recipes;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            recipes = await mergeRecipeProvider.GetRecipes();
        });
    }

    public async UniTask<MergeRequestResult> TryMergeSlots(ItemSlot slot1, ItemSlot slot2)
    {
        await UniTask.WaitWhile(() => recipes == null);

        // Putting an item in the same slot => no merging
        var isDifferentSlots = slot1.SlotId != slot2.SlotId;
        if (!isDifferentSlots)
            return new MergeRequestResult()
            {
                MergeResultType = MergeResultType.SAME_SLOT,
                MergeResultItem = default,
            };

        // Putting an item in empty slot => no merging
        if (slot1.IsEmpty() || slot2.IsEmpty())
            return new MergeRequestResult()
            {
                MergeResultType = MergeResultType.SINGLE_ITEM,
                MergeResultItem = default,
            };

        // No recipe found => no merging
        var recipe = GetRecipeForItems(slot1.ItemData, slot2.ItemData);
        if (recipe == null)
            return new MergeRequestResult()
            {
                MergeResultType = MergeResultType.NO_RECIPE_FOUND,
                MergeResultItem = default,
            };

        // Now, we can merge
        return new MergeRequestResult()
        {
            MergeResultType = MergeResultType.SUCCESS,
            MergeResultItem = bagItemsProvider.GetBagItemById(recipe.ResultingItem.Id),
        };
    }

    private MergeRecipe GetRecipeForItems(BagItemData item1, BagItemData item2)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            var recipe = recipes[i];

            // ATTENTION: order below is not important, so there are two checks, not one
            if (recipe.Item1.Id == item1.Id && recipe.Item2.Id == item2.Id)
                return recipe;

            if (recipe.Item1.Id == item2.Id && recipe.Item2.Id == item1.Id)
                return recipe;
        }

        return null;
    }

    public struct MergeRequestResult
    {
        public MergeResultType MergeResultType;
        public BagItemData MergeResultItem;
    }

    public enum MergeResultType
    {
        SUCCESS,
        SAME_SLOT,
        SINGLE_ITEM,
        NO_RECIPE_FOUND,
    }
}
