using UnityEngine;

/// <summary>
/// Logical bag item data
/// </summary>
public struct BagItemData
{
    public int Id;
    public int SlotId;
    public Color Color;

    public static BagItemData NO_TIEM = new() { Id = -1 };
}
