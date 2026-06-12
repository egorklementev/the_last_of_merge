using UnityEngine;

/// <summary>
/// TODO:
/// </summary>
public class ItemMergeController
{
    public bool CanBeMerged(BagItemData item1, BagItemData item2, out BagItemData result)
    {
        result = new BagItemData()
        {
            Id = 1,
            Color = Color.bisque,
            SlotId = item2.SlotId,
        };

        var isDifferentSlots = item1.SlotId != item2.SlotId;
        return !item1.IsEmpty() && !item2.IsEmpty() && isDifferentSlots; // && IsValidRecipe(item1, item2);
    }
}
