using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class DebugMergeRecipeProvider : IMergeRecipeProvider
{
    public async UniTask<IList<MergeRecipe>> GetRecipes()
    {
        await UniTask.WaitForSeconds(1.2f);
        return new List<MergeRecipe>()
        {
            new()
            {
                Id = 1,
                ItemId1 = 0,
                ItemId2 = 1,
                ResultingItemId = 2,
                Metadata = string.Empty,
            },
        };
    }
}
