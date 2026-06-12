using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Debug class for getting bag items
/// </summary>
public class DebugBagItemsProvider : IBagItemsProvider
{
    public async UniTask<IList<BagItemData>> GetBagItemsAsync()
    {
        await UniTask.WaitForSeconds(1f); // Simulating some timeout
        return new List<BagItemData>()
        {
            new()
            {
                Id = 0,
                SlotId = 1,
                Color = Color.gray,
            },
            new()
            {
                Id = 1,
                SlotId = 2,
                Color = Color.green,
            },
            new()
            {
                Id = 3,
                SlotId = 3,
                Color = Color.orangeRed,
            },
            new()
            {
                Id = 1,
                SlotId = 4,
                Color = Color.green,
            },
            new()
            {
                Id = 4,
                SlotId = 8,
                Color = Color.aliceBlue,
            },
            new()
            {
                Id = 2,
                SlotId = 9,
                Color = Color.yellow,
            },
            new()
            {
                Id = 0,
                SlotId = 13,
                Color = Color.gray,
            },
            new()
            {
                Id = 5,
                SlotId = 17,
                Color = Color.violet,
            },
        };
    }
}
