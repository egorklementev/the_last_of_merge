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

    private ItemMergeController itemMergeController = new();

    public async UniTask InitializeBagSpaceAsync()
    {
        var itemDatas = await bagItemsProvider.GetBagItemsAsync();
        await bagSpaceView.InitItemsAsync(itemDatas);

        bagSpaceView.ItemMoved += OnItemMoved;
    }

    private void OnItemMoved(BagItemData movingItemData, BagItemData restingItemData)
    {
        if (itemMergeController.CanBeMerged(movingItemData, restingItemData, out var result))
        {
            bagSpaceView.MergeItems(movingItemData, restingItemData, result);
            // TODO: update model here
        }
        else if (restingItemData.IsEmpty())
        {
            bagSpaceView.SnapItems(movingItemData, restingItemData);
            // TODO: update model here
        }
        else
        {
            bagSpaceView.SnapItems(movingItemData, movingItemData);
        }
    }
}
