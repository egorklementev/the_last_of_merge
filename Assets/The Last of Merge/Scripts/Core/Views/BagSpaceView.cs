using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BagSpaceView : MonoBehaviour, IBagSpaceView
{
    public event Action<BagItemData, BagItemData> ItemMoved;

    [Inject]
    private BagSpaceInitializer bagSpaceInitializer;

    private IList<IItemSlotView> itemSlots;

    public void ClearItems()
    {
        if (itemSlots == null)
            return;

        foreach (var slot in itemSlots)
        {
            slot.SetEmpty();
        }
    }

    public async UniTask InitItemsAsync(IList<BagItemData> items)
    {
        if (itemSlots != null) // No secondary initializations allowed
            return;

        await UniTask.WaitUntil(() => bagSpaceInitializer.Initialized);
        itemSlots = bagSpaceInitializer.GetInitializedItemSlots();

        for (int dataI = 0; dataI < items.Count; dataI++)
        {
            SetItem(items[dataI]);
        }

        for (int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            slot.Moved += () =>
            {
                var targetSlot = itemSlots.SingleOrDefault(s => s.IsHovered() && s.Id != slot.Id);
                if (targetSlot == null)
                {
                    ItemMoved?.Invoke(slot.GetItem(), slot.GetItem());
                }
                else
                {
                    ItemMoved?.Invoke(slot.GetItem(), targetSlot.GetItem());
                }
            };
        }
    }

    public void SetItem(BagItemData item)
    {
        if (itemSlots == null)
            return;

        for (int slotI = 0; slotI < itemSlots.Count; slotI++)
        {
            if (item.SlotId != itemSlots[slotI].Id)
                continue;

            itemSlots[slotI].SetItem(item);
        }
    }

    public void MergeItems(
        BagItemData movingItem,
        BagItemData restingItem,
        BagItemData resultingItem
    )
    {
        if (movingItem.SlotId == restingItem.SlotId)
            return;

        var movingSlot = itemSlots.SingleOrDefault(s => s.Id == movingItem.SlotId);
        var restingSlot = itemSlots.SingleOrDefault(s => s.Id == restingItem.SlotId);
        if (movingSlot == null || restingSlot == null)
        {
            // TODO: error
            return;
        }

        movingSlot.MergeWithItem(restingItem);
    }

    public void SnapItems(BagItemData movingItem, BagItemData restingItem)
    {
        var movingSlot = itemSlots.SingleOrDefault(s => s.Id == movingItem.SlotId);
        var restingSlot = itemSlots.SingleOrDefault(s => s.Id == restingItem.SlotId);
        if (movingSlot == null || restingSlot == null)
        {
            // TODO: error
            return;
        }

        movingSlot.SnapToSlot(restingItem);
        movingItem.SlotId = restingItem.SlotId; // ATTENTION: just changing ID to a new slot
        restingSlot.SetItem(movingItem);
    }
}
