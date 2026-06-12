using UnityEngine;

/// <summary>
/// Logical bag item data
/// </summary>
public struct BagItemData
{
    public int Id;
    public int SlotId;
    public Color Color;

    public void SetEmpty() => Id = -1;

    public readonly bool IsEmpty() => Id < 0;
}
