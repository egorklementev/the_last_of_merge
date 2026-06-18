using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class BagSpaceNetworkManager
{
    [Inject]
    private IBagItemsProvider bagItemsProvider;

    [Inject]
    private NetworkManager networkManager;

    public async UniTask<Dictionary<int, BagItemData>> SendInventoryRequest()
    {
        using var request = new UnityWebRequest($"{NetworkManager.BASE_URL}/inventory", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {networkManager.AuthToken}");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<InventoryRequestResponse>(responseText);
            var inventoryDict = new Dictionary<int, BagItemData>();

            foreach (var slot in response.Slots)
            {
                inventoryDict[slot.Position] = await bagItemsProvider.GetBagItemById(slot.ItemId);
            }

            Debug.Log("Inventory Loaded!");
            return inventoryDict;
        }
        else
        {
            Debug.LogError("Inventory Load Failed: " + request.downloadHandler.text);

            return new() { };
        }
    }

    public async UniTask<bool> SendInventorySaveRequest(Dictionary<int, int> items)
    {
        var json = JsonConvert.SerializeObject(items);

        using var request = new UnityWebRequest($"{NetworkManager.BASE_URL}/inventory", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {networkManager.AuthToken}");
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return true;
        }
        else
        {
            Debug.LogError("Save Inventory Failed: " + request.downloadHandler.text);
            return false;
        }
    }

    private struct InventoryRequestResponse
    {
        public long InventoryId;
        public ICollection<InventorySlotDto> Slots;
    }

    private struct InventorySlotDto
    {
        public long SlotId;
        public int Position;
        public int ItemId;
    }
}
