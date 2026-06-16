/// <summary>
/// Responsible for the whole UI interactions in the bag space
/// </summary>
public interface IBagSpaceView
{
    public void MergeItems(ItemSlot movingItem, ItemSlot restingItem, BagItemData resultingItem);

    public void SnapItems(ItemSlot movingItem, ItemSlot restingItem);
}
