using UnityEngine;

/// <summary>
/// Logical bag item data
/// </summary>
[CreateAssetMenu(fileName = "BagItemData", menuName = "Scriptable Objects/BagItemData")]
public class BagItemData : ScriptableObject
{
    public int Id;
    public Sprite Sprite;
}
