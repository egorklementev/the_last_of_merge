using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BagSpaceView : MonoBehaviour, IBagSpaceView
{
    [Inject]
    private BagSpaceInitializer bagSpaceInitializer;

    private IList<IItemSlotView> itemSlots;

    public void ClearItems()
    {
        if (itemSlots == null)
            return;

        foreach (var slot in itemSlots)
        {
            slot.SetItem(BagItemData.NO_TIEM);
        }
    }

    public async UniTask InitItemsAsync(IList<BagItemData> items)
    {
        await UniTask.WaitUntil(() => bagSpaceInitializer.Initialized);
        itemSlots = bagSpaceInitializer.GetInitializedItemSlots();

        for (int dataI = 0; dataI < items.Count; dataI++)
        {
            SetItem(items[dataI]);
        }
    }

    public void OnItemMove(int previousSlotId, int newSlotId)
    {
        if (itemSlots == null)
            return;
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
}
