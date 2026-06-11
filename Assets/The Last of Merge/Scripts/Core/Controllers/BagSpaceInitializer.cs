using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class BagSpaceInitializer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How many rows of items is there in the bag")]
    private int rowsCount = 7;

    [SerializeField]
    [Tooltip("Object to contain all instantiated rows of slots")]
    private RectTransform rowsHolder;

    [SerializeField]
    private RectTransform rowPrefab;

    [Inject]
    private IInstantiator instantiator;

    void Start()
    {
        rowsCount = math.clamp(rowsCount, 0, 10); // ATTENTION: max 10 rows
        for (int i = 0; i < rowsCount; i++)
        {
            instantiator.InstantiatePrefab(rowPrefab, rowsHolder);
        }
    }
}
