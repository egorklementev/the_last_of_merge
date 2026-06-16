using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Responsible for the whole UI interactions in the bag space
/// </summary>
public interface IBagSpaceView
{
    /// <summary>
    /// Fires when a user tries to move items inside the bag
    /// </summary>
    public event Action<ItemSlot, ItemSlot> ItemMoved;

    /// <summary>
    /// Initializes items in the bag space
    /// </summary>
    /// <param name="items">Items to put</param>
    public UniTask InitItemsAsync(IList<BagItemData> items);

    /// <summary>
    /// Clears the bag leaving it empty of any items
    /// </summary>
    public void ClearItems();

    public void MergeItems(ItemSlot movingItem, ItemSlot restingItem, BagItemData resultingItem);

    public void SnapItems(ItemSlot movingItem, ItemSlot restingItem);
}
