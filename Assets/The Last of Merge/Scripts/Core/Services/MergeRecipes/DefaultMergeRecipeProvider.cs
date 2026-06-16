using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DefaultMergeRecipeProvider : IMergeRecipeProvider, IInitializable
{
    [Inject]
    private AddressablesManager addressablesManager;

    private bool initialized = false;
    private IList<MergeRecipe> allMergeRecipes;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            await UniTask.WaitUntil(() => addressablesManager.Initialized);
            allMergeRecipes = await addressablesManager.LoadResourcesAsync<MergeRecipe>(
                "merge_recipe"
            );

            initialized = true;
            Debug.Log("[DefaultMergeRecipeProvider]: Initialized.");
        });
    }

    public async UniTask<IList<MergeRecipe>> GetRecipesAsync()
    {
        await UniTask.WaitUntil(() => initialized && allMergeRecipes != null);

        return allMergeRecipes;
    }

    public MergeRecipe GetRecipeById(int id)
    {
        for (int i = 0; i < allMergeRecipes.Count; i++)
        {
            if (allMergeRecipes[i].Id == id)
                return allMergeRecipes[i];
        }

        Debug.LogWarning($"No recipe data found for id {id}!");
        return default;
    }
}
