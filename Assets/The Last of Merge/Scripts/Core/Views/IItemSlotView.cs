public interface IItemSlotView
{
    /// <summary>
    /// A unique ID of an item slot
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Sets (or replaces) an item within this slot
    /// </summary>
    public void SetItem(BagItemData data);
}
