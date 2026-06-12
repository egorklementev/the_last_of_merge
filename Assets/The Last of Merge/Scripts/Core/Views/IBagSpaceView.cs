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
    public event Action<BagItemData, BagItemData> ItemMoved;

    /// <summary>
    /// Initializes items in the bag space
    /// </summary>
    /// <param name="items">Items to put</param>
    public UniTask InitItemsAsync(IList<BagItemData> items);

    /// <summary>
    /// Clears the bag leaving it empty of any items
    /// </summary>
    public void ClearItems();

    /// <param name="item">Item to be set in the bag</param>
    public void SetItem(BagItemData item);

    public void MergeItems(
        BagItemData movingItem,
        BagItemData restingItem,
        BagItemData resultingItem
    );

    public void SnapItems(BagItemData movingItem, BagItemData restingItem);
}
