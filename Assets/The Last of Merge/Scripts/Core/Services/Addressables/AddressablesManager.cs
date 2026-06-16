using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class AddressablesManager : IInitializable, IDisposable
{
    public bool Initialized { get; private set; } = false;

    private readonly List<AsyncOperationHandle> resourceHandles = new();

    public void Initialize()
    {
        UniTask.Void(async () =>
        {
            await Addressables.InitializeAsync();
            var resourcesToUpdate = await Addressables.CheckForCatalogUpdates();

            if (resourcesToUpdate != null && resourcesToUpdate.Count > 0)
            {
                await Addressables.UpdateCatalogs(resourcesToUpdate);
            }

            Initialized = true;
            Debug.Log("[AddressablesManager]: Initailized.");
        });
    }

    public async UniTask<IList<T>> LoadResourcesAsync<T>(string label)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label);
        resourceHandles.Add(handle);
        return await handle;
    }

    public void Dispose()
    {
        foreach (var handle in resourceHandles)
        {
            handle.Release();
        }
    }
}
