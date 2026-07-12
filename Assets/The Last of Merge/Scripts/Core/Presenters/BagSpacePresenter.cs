using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Zenject;

/// <summary>
/// Controls all the logic of bag space
/// </summary>
public class BagSpacePresenter
{
    public bool Loaded { get; set; } = false;

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

    [Inject]
    private NotificationManager notificationManager;

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

            slot.LongPress += () =>
            {
                notificationManager
                    .ShowNotification(
                        "bagspace_bagitem_info",
                        $"item_{slot.ItemData.Id}",
                        slot.ItemData.Sprite
                    )
                    .Forget();
            };

            await UniTask.WaitUntil(() => bagSpaceModel.Loaded);
            var data = bagSpaceModel.GetDataForSlot(slot.SlotId);
            if (data == null || data.Id < 0)
                slot.SetEmpty();
            else
                slot.SetItem(data);

            Loaded = true;
        }
    }

    public ItemSlot GetRandomFreeSlot(bool inEquip = false)
    {
        bool Selector(ItemSlot slot) =>
            slot.IsEmpty() && (inEquip && slot.IsEquipSlot || !inEquip && !slot.IsEquipSlot);

        if (itemSlots.Count(Selector) == 0)
            return null;

        return itemSlots.Where(Selector).GetRandom();
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

    public void ClearEquippedItems()
    {
        foreach (var slot in itemSlots.Where(s => s.IsEquipSlot))
        {
            slot.SetEmpty();
            bagSpaceModel.UpdateSlot(slot);
        }

        bagSpaceModel.SaveDataToServer().Forget();
    }

    public void OnDeploymentStart(bool isInstant = false)
    {
        bagSpaceView.SetInDeployment(isInstant);
    }

    public void OnDeploymentFinish(bool isInstant = false)
    {
        bagSpaceView.FinishDeployment(isInstant);
    }
}
