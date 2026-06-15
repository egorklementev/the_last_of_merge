using UnityEngine;

/// <summary>
/// Logical bag item data
/// </summary>
[CreateAssetMenu(fileName = "BagItemData", menuName = "Scriptable Objects/BagItemData")]
public class BagItemData : ScriptableObject
{
    public int Id;
    public int SlotId;
    public Color Color;

    public void SetEmpty() => Id = -1;

    public bool IsEmpty() => Id < 0;
}
