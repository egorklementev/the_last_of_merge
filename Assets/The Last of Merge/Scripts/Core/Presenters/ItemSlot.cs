using System;
using Zenject;
using static IItemSlotView;

public class ItemSlot : IInitializable
{
    public event Action Moved;

    public int SlotId { get; set; }
    public BagItemData ItemData { get; private set; }
    public IItemSlotView SlotView { get; set; }

    public void Initialize()
    {
        SlotView.Moved += () => Moved?.Invoke();
    }

    public void SetEmpty()
    {
        ItemData = null;
        SlotView.SetEmpty();
    }

    public bool IsEmpty() => ItemData == null;

    public bool IsHovered() => SlotView.GetState() == ItemSlotState.HOVERED;

    public bool JustReleased() => SlotView.GetState() == ItemSlotState.RELEASED;

    public void SetItem(BagItemData bagItemData)
    {
        ItemData = bagItemData;
        SlotView.SetItem(bagItemData);
    }

    public void MergeWithSlot()
    {
        SetEmpty();
        SlotView.OnMergeWithSlot();
    }

    public void SnapToSlot(ItemSlot restingSlot)
    {
        if (restingSlot == this)
        {
            SlotView.OnSnapToSlot(true);
        }
        else
        {
            SetEmpty();
            SlotView.OnSnapToSlot();
        }
    }
}
