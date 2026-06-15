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
        var mergeResult = await itemMergeController.TryMergeItems(movingItemData, restingItemData);

        // TODO: update the model down below
        switch (mergeResult.MergeResultType)
        {
            case ItemMergeController.MergeResultType.SUCCESS:
                bagSpaceView.MergeItems(
                    movingItemData,
                    restingItemData,
                    mergeResult.MergeResultItem
                );
                break;
            case ItemMergeController.MergeResultType.SAME_SLOT:
                bagSpaceView.SnapItems(movingItemData, movingItemData);
                break;
            case ItemMergeController.MergeResultType.SINGLE_ITEM: // Moving item
                bagSpaceView.SnapItems(movingItemData, restingItemData);
                break;
            case ItemMergeController.MergeResultType.NO_RECIPE_FOUND:
                // TODO: show some visuals here
                break;
        }
    }
}
