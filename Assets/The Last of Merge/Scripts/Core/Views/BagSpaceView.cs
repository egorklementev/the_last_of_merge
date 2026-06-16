using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BagSpaceView : MonoBehaviour, IBagSpaceView
{
    public event Action<ItemSlot, ItemSlot> ItemMoved;

    [Inject]
    private BagSpaceInitializer bagSpaceInitializer;

    private IList<ItemSlot> itemSlots;

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

        // TODO: set actual item data to corresponding slot
        // TODO: place this code to the right location, SOLID is bad here
        itemSlots[2].SetItem(items[0]);
        itemSlots[4].SetItem(items[1]);
        itemSlots[6].SetItem(items[2]);
        itemSlots[8].SetItem(items[3]);
        itemSlots[10].SetItem(items[4]);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            slot.Moved += () =>
            {
                var targetSlot = itemSlots.SingleOrDefault(s =>
                    s.IsHovered() && s.SlotId != slot.SlotId
                );
                if (targetSlot == null)
                {
                    ItemMoved?.Invoke(slot, slot);
                }
                else
                {
                    ItemMoved?.Invoke(slot, targetSlot);
                }
            };
        }
    }

    public void MergeItems(ItemSlot movingSlot, ItemSlot restingSlot, BagItemData resultingItem)
    {
        movingSlot.MergeWithSlot();
        restingSlot.SetItem(resultingItem);
    }

    public void SnapItems(ItemSlot movingSlot, ItemSlot restingSlot)
    {
        restingSlot.SetItem(movingSlot.ItemData);
        movingSlot.SnapToSlot(restingSlot);
    }
}
