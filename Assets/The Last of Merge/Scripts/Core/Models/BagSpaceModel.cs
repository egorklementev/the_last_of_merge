using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BagSpaceModel : IInitializable
{
    public bool Loaded { get; set; } = false;

    [Inject]
    private AuthorizationHandler authorizationHandler;

    [Inject]
    private BagSpaceNetworkManager bagSpaceNetworkManager;

    private Dictionary<int, BagItemData> slotsToItems;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            await UniTask.WaitUntil(() => authorizationHandler.Authorized);
            slotsToItems = await bagSpaceNetworkManager.SendInventoryRequest();

            Loaded = true;
        });
    }

    public Dictionary<int, BagItemData> GetItemsAtSlots()
    {
        if (!Loaded)
            return new();

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

    public async UniTask SaveDataToServer()
    {
        if (!Loaded)
            return;

        var dictToSend = new Dictionary<int, int>();
        foreach (var slotId in slotsToItems.Keys)
        {
            dictToSend[slotId] = slotsToItems[slotId].Id;
        }

        if (await bagSpaceNetworkManager.SendInventorySaveRequest(dictToSend))
        {
            Debug.Log("[BagSpaceModel]: Inventory successfully saved on server!");
        }
    }
}
