using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class DebugMergeRecipeProvider : IMergeRecipeProvider
{
    public MergeRecipe GetRecipeById(int id)
    {
        return new();
    }

    public async UniTask<IList<MergeRecipe>> GetRecipesAsync()
    {
        await UniTask.WaitForSeconds(1.2f);
        return new List<MergeRecipe>()
        {
            new()
            {
                Id = 1,
                Item1 = new() { Id = 0 },
                Item2 = new() { Id = 1 },
                ResultingItem = new() { Id = 2 },
                Metadata = string.Empty,
            },
        };
    }
}
