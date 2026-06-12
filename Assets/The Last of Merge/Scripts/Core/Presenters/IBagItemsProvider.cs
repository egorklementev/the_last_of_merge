using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Source of bag item data objects
/// </summary>
public interface IBagItemsProvider
{
    /// <returns>A list of bag items stored in the bag.</returns>
    public UniTask<IList<BagItemData>> GetBagItemsAsync();
}
