using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Debug class for getting bag items
/// </summary>
public class DebugBagItemsProvider : IBagItemsProvider
{
    public async UniTask<BagItemData> GetBagItemById(int id)
    {
        return new() { Id = 0 };
    }

    public async UniTask<IList<BagItemData>> GetBagItemsAsync()
    {
        await UniTask.WaitForSeconds(1f); // Simulating some timeout
        return new List<BagItemData>()
        {
            new() { Id = 0 },
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
            new() { Id = 4 },
            new() { Id = 5 },
        };
    }
}
