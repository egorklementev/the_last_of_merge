using Cysharp.Threading.Tasks;
using Zenject;

/// <summary>
/// Manages recipe screen logic
/// </summary>
public class RecipeScreenPresenter
{
    [Inject]
    private IRecipeScreenView recipeScreenView;

    [Inject]
    private IMergeRecipeProvider mergeRecipeProvider;

    public async UniTask InitializeAsync()
    {
        var recipes = await mergeRecipeProvider.GetRecipesAsync();
        recipeScreenView.FillListWithItems(recipes);
    }
}
