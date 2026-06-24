using Cysharp.Threading.Tasks;
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

    public void Initialize()
    {
        deploymentScreenView.DeployButtonClicked += () => OnDeploy().Forget();

        UniTask.Void(async () =>
        {
            await UniTask.WaitUntil(() => authorization.Authorized);

            var timeleft = await deploymentManager.GetTimeLeft();
            if (timeleft == 0f)
            {
                deploymentScreenView.FinishDeployment();
            }
            else
            {
                OnDeploy().Forget();
            }
        });
    }

    private async UniTaskVoid OnDeploy()
    {
        if (inDeployment)
            return;

        inDeployment = true;

        const float overhead = 3f;

        var timeLeft = await deploymentManager.DeployPlayer();
        deploymentScreenView.SetInDeployment(timeLeft + overhead);

        await UniTask.WaitForSeconds(timeLeft + overhead); // ATTENTION: a little overhead for the server works

        var deployment = await deploymentManager.GetLastDeployment();
        var bagItems = deployment.FoundItems.Select(id => bagItemsProvider.GetBagItemById(id));
        var items = await UniTask.WhenAll(bagItems);
        deploymentScreenView.SetFoundItems(items);

        foreach (var itemId in deployment.FoundItems)
        {
            var freeSlot = bagSpacePresenter.GetRandomFreeSlot();
            if (freeSlot == null)
                break;

            var item = await bagItemsProvider.GetBagItemById(itemId);
            if (item == null)
                continue;

            freeSlot.SetItem(item);
            bagSpaceModel.UpdateSlot(freeSlot);
        }

        // TODO: consume equipped items from result

        inDeployment = false;
    }
}
