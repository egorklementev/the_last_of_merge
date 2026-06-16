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
    private BagSpaceModel bagSpaceModel;

    private bool inDeployment = false;

    public void Initialize()
    {
        deploymentScreenView.DeployButtonClicked += () => OnDeploy().Forget();
    }

    private async UniTaskVoid OnDeploy()
    {
        if (inDeployment)
            return;

        inDeployment = true;

        var result = await deploymentManager.DeployPlayer();

        // TODO: while waiting update the deployment bar

        foreach (var item in result.FoundItems)
        {
            var freeSlot = bagSpacePresenter.GetRandomFreeSlot();
            if (freeSlot == null)
                break;

            freeSlot.SetItem(item);
            bagSpaceModel.UpdateSlot(freeSlot);
        }

        // TODO: consume equipped items from result

        inDeployment = false;
    }
}
