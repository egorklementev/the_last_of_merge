using System.Collections.Generic;
using System.Linq;
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
    private BagSpaceInitializer bagSpaceInitializer;

    [Inject]
    private ItemMergeController itemMergeController;

    [Inject]
    private BagSpaceModel bagSpaceModel;

    public async UniTask InitializeAsync()
    {
        var itemDatas = await bagItemsProvider.GetBagItemsAsync();
        await InitItemsAsync(itemDatas);
    }

    private IList<ItemSlot> itemSlots;

    public async UniTask InitItemsAsync(IList<BagItemData> items)
    {
        if (itemSlots != null) // No secondary initializations allowed
            return;

        await UniTask.WaitUntil(() => bagSpaceInitializer.Initialized);
        itemSlots = bagSpaceInitializer.GetInitializedItemSlots();

        for (int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            slot.Moved += () =>
            {
                // If two slots are moved at the same time - no action
                if (itemSlots.Count(s => s.JustReleased()) > 1)
                {
                    OnItemMoved(slot, slot).Forget();
                }

                var targetSlot = itemSlots.SingleOrDefault(s =>
                    s.IsHovered() && s.SlotId != slot.SlotId
                );
                if (targetSlot == null)
                {
                    OnItemMoved(slot, slot).Forget();
                }
                else
                {
                    OnItemMoved(slot, targetSlot).Forget();
                }
            };

            await UniTask.WaitUntil(() => bagSpaceModel.Loaded);
            var data = bagSpaceModel.GetDataForSlot(slot.SlotId);
            if (data == null || data.Id < 0)
                slot.SetEmpty();
            else
                slot.SetItem(data);
        }
    }

    public ItemSlot GetRandomFreeSlot()
    {
        if (itemSlots.Count(slot => slot.IsEmpty()) == 0)
            return null;

        return itemSlots.Where(slot => slot.IsEmpty()).GetRandom();
    }

    private async UniTaskVoid OnItemMoved(ItemSlot movingSlot, ItemSlot restingSlot)
    {
        var mergeResult = await itemMergeController.TryMergeSlots(movingSlot, restingSlot);

        switch (mergeResult.MergeResultType)
        {
            case ItemMergeController.MergeResultType.SUCCESS:
                bagSpaceView.MergeItems(movingSlot, restingSlot, mergeResult.MergeResultItem);
                bagSpaceModel.SetEmpty(movingSlot);
                bagSpaceModel.UpdateSlot(restingSlot);
                break;
            case ItemMergeController.MergeResultType.SAME_SLOT:
                bagSpaceView.SnapItems(movingSlot, movingSlot);
                break;
            case ItemMergeController.MergeResultType.SINGLE_ITEM: // Moving item
                bagSpaceView.SnapItems(movingSlot, restingSlot);
                bagSpaceModel.SetEmpty(movingSlot);
                bagSpaceModel.UpdateSlot(restingSlot);
                break;
            case ItemMergeController.MergeResultType.NO_RECIPE_FOUND:
                // TODO: show some visuals here
                bagSpaceView.SnapItems(movingSlot, movingSlot);
                break;
        }
    }
}
