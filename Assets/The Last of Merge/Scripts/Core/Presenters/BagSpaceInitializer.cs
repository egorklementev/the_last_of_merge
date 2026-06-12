using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

/// <summary>
/// Initializes the bag rows & slots (actual prefabs)
/// </summary>
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

    [SerializeField]
    private List<ItemSlotView> equipmentSlotViews = new();

    [Inject]
    private DiContainer container;

    public bool Initialized { get; private set; } = false;

    private List<IItemSlotView> itemSlotViews = new();

    void Start()
    {
        int slotCounter = 0;
        rowsCount = math.clamp(rowsCount, 0, 10); // ATTENTION: max 10 rows
        for (int i = 0; i < rowsCount; i++)
        {
            var row = container.InstantiatePrefab(rowPrefab, rowsHolder);
            var slotsInRow = row.GetComponentsInChildren<ItemSlotView>();
            foreach (var slot in slotsInRow)
            {
                slot.Id = slotCounter++;
            }

            itemSlotViews.AddRange(slotsInRow);
        }

        itemSlotViews.AddRange(equipmentSlotViews);
        for (int i = 0; i < equipmentSlotViews.Count; i++)
        {
            equipmentSlotViews[i].Id = slotCounter++;
        }

        Initialized = true;
    }

    public IList<IItemSlotView> GetInitializedItemSlots() => itemSlotViews;
}
