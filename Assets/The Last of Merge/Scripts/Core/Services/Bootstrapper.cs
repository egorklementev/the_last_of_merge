using Cysharp.Threading.Tasks;
using Zenject;

/// <summary>
/// Initializes game logic & services
/// </summary>
public class Bootstrapper : IInitializable
{
    [Inject]
    private BagSpacePresenter bagSpacePresenter;

    [Inject]
    private RecipeScreenPresenter recipeScreenPresenter;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            await bagSpacePresenter.InitializeAsync();
            await recipeScreenPresenter.InitializeAsync();
        });
    }
}
