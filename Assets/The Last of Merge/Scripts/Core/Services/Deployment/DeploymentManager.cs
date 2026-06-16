using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DeploymentManager
{
    [Inject]
    private IBagItemsProvider bagItemsProvider;

    public float GetNextDeploymentDuration()
    {
        return 2.7f;
    }

    public async UniTask<DeploymentResult> DeployPlayer()
    {
        await UniTask.WaitForSeconds(GetNextDeploymentDuration());
        var foundItemsCount = Random.Range(1, 5);
        var result = new DeploymentResult()
        {
            FoundItems = new List<BagItemData>(),
            UsedEuippedItems = new List<BagItemData>(),
        };

        while (foundItemsCount-- > 0)
        {
            // TODO: change to actual gameplay reward logic
            result.FoundItems.Add(bagItemsProvider.GetBagItemById(Random.Range(0, 5)));
        }

        return result;
    }
}
