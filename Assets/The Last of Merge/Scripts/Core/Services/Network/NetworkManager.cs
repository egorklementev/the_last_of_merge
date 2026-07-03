using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// TODO: think of a better purpose (and maybe a name) for this file
public class NetworkManager
{
    public const string BASE_URL = "http://api.alherorun.ru/api";

    public string AuthToken { get; private set; } = "";

    public async UniTask<LoginResult> SendLoginRequest(string username, string password)
    {
        string json = JsonConvert.SerializeObject(new { Username = username, Password = password });

        using var request = new UnityWebRequest($"{BASE_URL}/auth/login", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        try
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<LoginRequestResponse>(responseText);

                AuthToken = response.token;

                PlayerPrefs.SetString("AuthToken", response.token);
                Debug.Log("Login Success! Token saved.");
                return new()
                {
                    Success = true,
                    Token = response.token,
                    PlayerId = response.playerId,
                };
            }
            else
            {
                Debug.LogError("Login Failed: " + request.downloadHandler.text);

                return new() { Success = false };
            }
        }
        catch (UnityWebRequestException)
        {
            Debug.LogError("Login Request Failed: " + request.downloadHandler.text);

            return new() { Success = false };
        }
    }

    public async UniTask<bool> SendRegisterRequest(string username, string password)
    {
        string json = JsonConvert.SerializeObject(new { Username = username, Password = password });

        using var request = new UnityWebRequest($"{BASE_URL}/auth/register", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        try
        {
            await request.SendWebRequest();

            return request.result == UnityWebRequest.Result.Success;
        }
        catch (UnityWebRequestException)
        {
            Debug.Log("Failed to register!");
            return false;
        }
    }

    public struct LoginResult
    {
        public bool Success;
        public string Token;
        public long PlayerId;
    }

    private struct LoginRequestResponse
    {
        public string token;
        public long playerId;
    }
}
