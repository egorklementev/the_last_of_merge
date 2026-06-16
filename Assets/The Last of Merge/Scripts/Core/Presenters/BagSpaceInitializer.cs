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

    private List<ItemSlot> itemSlots = new();

    void Start()
    {
        int slotCounter = 0;
        rowsCount = math.clamp(rowsCount, 0, 10); // ATTENTION: max 10 rows
        var allSlots = new List<ItemSlot>();
        for (int i = 0; i < rowsCount; i++)
        {
            var row = container.InstantiatePrefab(rowPrefab, rowsHolder);
            var slotsViewsInRow = row.GetComponentsInChildren<ItemSlotView>();
            for (int j = 0; j < slotsViewsInRow.Length; j++)
            {
                var slot = container.Instantiate<ItemSlot>();
                slot.SlotId = slotCounter++;
                slot.SlotView = slotsViewsInRow[j];
                slot.Initialize();

                slotsViewsInRow[j].ContainingSlot = slot;
                allSlots.Add(slot);
            }
        }

        for (int i = 0; i < equipmentSlotViews.Count; i++)
        {
            var slot = container.Instantiate<ItemSlot>();
            slot.SlotId = slotCounter++;
            slot.SlotView = equipmentSlotViews[i];
            slot.Initialize();

            equipmentSlotViews[i].ContainingSlot = slot;
            allSlots.Add(slot);
        }

        itemSlots.AddRange(allSlots);

        Initialized = true;
    }

    public IList<ItemSlot> GetInitializedItemSlots() => itemSlots;
}
