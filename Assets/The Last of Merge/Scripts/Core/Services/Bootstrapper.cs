using Cysharp.Threading.Tasks;
using Zenject;

/// <summary>
/// Initializes game logic & services
/// </summary>
public class Bootstrapper : IInitializable
{
    [Inject]
    private BagSpacePresenter bagSpacePresenter;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            await bagSpacePresenter.InitializeBagSpaceAsync();
        });
    }
}
