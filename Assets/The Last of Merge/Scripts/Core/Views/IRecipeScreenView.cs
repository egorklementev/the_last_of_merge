using System.Collections.Generic;

public interface IRecipeScreenView
{
    void FillListWithItems(IList<MergeRecipe> recipes);
}
