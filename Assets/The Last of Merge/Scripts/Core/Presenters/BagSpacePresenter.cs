using Cysharp.Threading.Tasks;
using Zenject;

/// <summary>
/// Controls all the logic of bag space
/// </summary>
public class BagSpacePresenter
{
    [Inject]
    private IBagItemsProvider bagItemsProvider;

    [Inject]
    private IBagSpaceView bagSpaceView;

    public async UniTask InitializeBagSpaceAsync()
    {
        var itemDatas = await bagItemsProvider.GetBagItemsAsync();
        await bagSpaceView.InitItemsAsync(itemDatas);
    }
}
