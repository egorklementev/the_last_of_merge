using System;

public interface IItemSlotView
{
    /// <summary>
    /// Fires when tried to move across the bag.
    /// </summary>
    public event Action Moved;

    /// <summary>
    /// Fires when a user long presses on this view.
    /// </summary>
    public event Action LongPress;

    /// <summary>
    /// Sets (or replaces) an item within this slot.
    /// </summary>
    public void SetItem(BagItemData data);

    /// <summary>
    /// Removes an item from this slot.
    /// </summary>
    public void SetEmpty();

    /// <param name="item">Item to be merged with (contains SlotId).</param>
    public void OnMergeWithSlot();

    /// <param name="item">Contains SlotId, so there is info about where to snap.</param>
    public void OnSnapToSlot(bool isSameSlot = false);

    public ItemSlotState GetState();

    public enum ItemSlotState
    {
        RESTING,
        HOVERED,
        DRAGGING,
        RELEASED,
        SNAPPING,
        MERGING,
        LONG_PRESS,
    }
}
