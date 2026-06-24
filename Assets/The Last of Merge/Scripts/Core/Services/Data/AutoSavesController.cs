using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AutoSavesController : MonoBehaviour
{
    [Inject]
    private BagSpaceModel bagSpaceModel;

    [SerializeField]
    private float saveRate = 10f;

    private float saveTimer = 0;

    void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer < saveRate)
            return;

        saveTimer = 0f;
        UniTask.Void(async () =>
        {
            await bagSpaceModel.SaveDataToServer();
        });
    }
}
