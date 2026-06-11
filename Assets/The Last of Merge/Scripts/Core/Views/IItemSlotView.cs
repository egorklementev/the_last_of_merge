using System;

public interface IItemSlotView
{
    /// <summary>
    /// Fires when tried to move across the bag.
    /// </summary>
    public event Action Moved;

    /// <summary>
    /// A unique ID of an item slot.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Sets (or replaces) an item within this slot.
    /// </summary>
    public void SetItem(BagItemData data);

    /// <summary>
    /// Removes an item from this slot.
    /// </summary>
    public void SetEmpty();

    /// <returns>Logical info about containing item.</returns>
    public BagItemData GetItem();

    /// <param name="item">Item to be merged with (contains SlotId).</param>
    public void MergeWithItem(BagItemData item);

    /// <param name="item">Contains SlotId, so there is info about where to snap.</param>
    public void SnapToSlot(BagItemData item);

    public bool IsHovered();
}
