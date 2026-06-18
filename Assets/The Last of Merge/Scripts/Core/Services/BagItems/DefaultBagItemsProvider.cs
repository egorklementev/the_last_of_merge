using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DefaultBagItemsProvider : IBagItemsProvider, IInitializable
{
    [Inject]
    private AddressablesManager addressablesManager;

    private bool initialized = false;
    private IList<BagItemData> allItems;

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            await UniTask.WaitUntil(() => addressablesManager.Initialized);
            allItems = await addressablesManager.LoadResourcesAsync<BagItemData>("bag_item");

            initialized = true;
            Debug.Log("[DefaultBagItemsProvider]: Initialized.");
        });
    }

    public async UniTask<IList<BagItemData>> GetBagItemsAsync()
    {
        await UniTask.WaitUntil(() => initialized && allItems != null);

        return allItems;
    }

    public async UniTask<BagItemData> GetBagItemById(int id)
    {
        await UniTask.WaitUntil(() => initialized);

        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i].Id == id)
                return allItems[i];
        }

        Debug.LogWarning($"No item data found for id {id}!");
        return default;
    }
}
