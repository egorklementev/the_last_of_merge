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

    [Inject]
    private ItemMergeController itemMergeController;

    public async UniTask InitializeBagSpaceAsync()
    {
        var itemDatas = await bagItemsProvider.GetBagItemsAsync();
        await bagSpaceView.InitItemsAsync(itemDatas);

        bagSpaceView.ItemMoved += (i1, i2) => OnItemMoved(i1, i2).Forget();
    }

    private async UniTaskVoid OnItemMoved(BagItemData movingItemData, BagItemData restingItemData)
    {
        var (canBeMerged, resultingItem) = await itemMergeController.CanBeMerged(
            movingItemData,
            restingItemData
        );

        if (canBeMerged)
        {
            bagSpaceView.MergeItems(movingItemData, restingItemData, resultingItem);
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
