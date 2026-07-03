using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DeploymentScreenPresenter : IInitializable
{
    [Inject]
    private IDeploymentScreenView deploymentScreenView;

    [Inject]
    private DeploymentManager deploymentManager;

    [Inject]
    private BagSpacePresenter bagSpacePresenter;

    [Inject]
    private IBagItemsProvider bagItemsProvider;

    [Inject]
    private BagSpaceModel bagSpaceModel;

    [Inject]
    private AuthorizationHandler authorization;

    private bool inDeployment = false;
    private float deployTimeLeft = 0f;

    public void Initialize()
    {
        deploymentScreenView.DeployButtonClicked += () => OnDeploy().Forget();

        UniTask.Void(async () =>
        {
            await UniTask.WaitUntil(() => authorization.Authorized);
            await UniTask.WaitUntil(() => bagSpaceModel.Loaded);

            deployTimeLeft = await deploymentManager.GetTimeLeft();
            if (deployTimeLeft == 0f) // Either no deployments, or last one has finished
            {
                var deployment = await deploymentManager.GetLastDeployment();

                if (await deploymentManager.TryToObtainItemsIfAny()) // In case we have not yet get our found items
                {
                    Debug.Log("[DeploymentScreenPresenter]: Obtaining found items...");
                    await PutItemsIntoInventory(deployment);
                }

                await UpdateFoundItems(deployment);
                bagSpacePresenter.OnDeploymentFinish(true);
                deploymentScreenView.FinishDeployment(true);
            }
            else
            {
                OnDeploy(true).Forget();
            }
        });
    }

    private async UniTaskVoid OnDeploy(bool hasAlreadyStarted = false)
    {
        if (inDeployment)
            return;

        inDeployment = true;

        const float overhead = 3f;

        var timeLeft = hasAlreadyStarted ? deployTimeLeft : await deploymentManager.DeployPlayer();
        deploymentScreenView.SetInDeployment(timeLeft + overhead);

        if (!hasAlreadyStarted)
            bagSpacePresenter.ClearEquippedItems(); // TODO: tell the server, what items are being used in a deployment

        bagSpacePresenter.OnDeploymentStart(hasAlreadyStarted);

        // ATTENTION: a little overhead for the server works.
        // I know this is not okay and we need to establish a ask loop
        // with 3 sec delay for example, but for now KISS
        await UniTask.WaitForSeconds(timeLeft + overhead);

        var deployment = await deploymentManager.GetLastDeployment();
        await UpdateFoundItems(deployment);

        bagSpacePresenter.OnDeploymentFinish();

        await PutItemsIntoInventory(deployment);
        await deploymentManager.TryToObtainItemsIfAny(); // Mark items (on backend) as obtained
        await bagSpaceModel.SaveDataToServer();

        inDeployment = false;
    }

    private async UniTask UpdateFoundItems(DeploymentResult deployment)
    {
        if (deployment == null)
            return;

        var bagItems = deployment.FoundItems.Select(id => bagItemsProvider.GetBagItemById(id));
        var items = await UniTask.WhenAll(bagItems);
        deploymentScreenView.SetFoundItems(items);
        deploymentScreenView.FinishDeployment();
    }

    private async UniTask PutItemsIntoInventory(DeploymentResult deployment)
    {
        foreach (var itemId in deployment.FoundItems)
        {
            var freeSlot = bagSpacePresenter.GetRandomFreeSlot(true);
            if (freeSlot == null)
                break;

            var item = await bagItemsProvider.GetBagItemById(itemId);
            if (item == null)
                continue;

            freeSlot.SetItem(item);
            bagSpaceModel.UpdateSlot(freeSlot);
        }
    }
}
