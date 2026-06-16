using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;

public class BagSpaceModel : IInitializable
{
    [Inject]
    private IBagItemsProvider bagItemsProvider;

    private bool loaded = false;
    private IList<BagItemData> items;
    private Dictionary<int, BagItemData> slotsToItems;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            items = await bagItemsProvider.GetBagItemsAsync();

            slotsToItems = new()
            {
                { 2, items[0] },
                { 4, items[1] },
                { 6, items[2] },
                { 8, items[3] },
                { 10, items[4] },
                { 12, items[5] },
                { 13, items[6] },
            };

            loaded = true;
        });
    }

    public Dictionary<int, BagItemData> GetItemsAtSlots()
    {
        if (!loaded)
            return new();

        // TODO: load actual items from the server
        return slotsToItems;
    }

    public void SetEmpty(ItemSlot slot) => slotsToItems.Remove(slot.SlotId);

    public void UpdateSlot(ItemSlot slot) => slotsToItems[slot.SlotId] = slot.ItemData;

    public BagItemData GetDataForSlot(int slotId)
    {
        if (slotsToItems.ContainsKey(slotId))
            return slotsToItems[slotId];

        return null;
    } 
}
