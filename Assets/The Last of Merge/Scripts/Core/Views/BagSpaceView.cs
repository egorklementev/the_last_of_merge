using UnityEngine;

public class BagSpaceView : MonoBehaviour, IBagSpaceView
{
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
