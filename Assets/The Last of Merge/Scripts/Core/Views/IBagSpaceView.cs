using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Responsible for the whole UI interactions in the bag space
/// </summary>
public interface IBagSpaceView
{
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

    /// <summary>
    /// Callback whenever user is trying to move an item to some slot (possibly the same one)
    /// </summary>
    public void OnItemMove(int previousSlotId, int newSlotId);
}
