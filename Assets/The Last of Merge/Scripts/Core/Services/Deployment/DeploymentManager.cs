using Cysharp.Threading.Tasks;
using Zenject;

public class DeploymentManager
{
    [Inject]
    private DeploymentsNetworkManager networkManager;

    public async UniTask<float> DeployPlayer()
    {
        var timeLeft = await networkManager.RequestDeploymentStart();
        return timeLeft;
    }

    public async UniTask<float> GetTimeLeft()
    {
        var timeLeft = await networkManager.RequestTimeLeftInSecs();
        return timeLeft;
    }

    public async UniTask<DeploymentResult> GetLastDeployment()
    {
        return await networkManager.RequestLastDeployment();
    }
}
