using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IMergeRecipeProvider
{
    public UniTask<IList<MergeRecipe>> GetRecipes();
}
