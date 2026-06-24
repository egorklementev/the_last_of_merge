using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Zenject;

public class DeploymentsNetworkManager
{
    [Inject]
    private NetworkManager networkManager;

    public async UniTask<float> RequestTimeLeftInSecs()
    {
        using var request = new UnityWebRequest(
            $"{NetworkManager.BASE_URL}/deployments/time-left",
            "GET"
        );
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {networkManager.AuthToken}");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            var timeLeft = JsonConvert.DeserializeObject<int>(responseText);
            return timeLeft / 1000f;
        }
        else
        {
            return 0f;
        }
    }

    public async UniTask<float> RequestDeploymentStart()
    {
        using var request = new UnityWebRequest(
            $"{NetworkManager.BASE_URL}/deployments/new",
            "POST"
        );
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {networkManager.AuthToken}");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var json = request.downloadHandler.text;
            var timeLeft = (float)JsonConvert.DeserializeObject<double>(json);
            return timeLeft / 1000f;
        }

        return 0f;
    }

    public async UniTask<DeploymentResult> RequestLastDeployment()
    {
        using var request = new UnityWebRequest(
            $"{NetworkManager.BASE_URL}/deployments/last",
            "GET"
        );
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {networkManager.AuthToken}");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var json = request.downloadHandler.text;
            var result = JsonConvert.DeserializeObject<DeploymentResult>(json);

            return result;
        }

        return default;
    }
}
