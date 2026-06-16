using UnityEngine;

[CreateAssetMenu(fileName = "MergeRecipe", menuName = "Scriptable Objects/MergeRecipe")]
public class MergeRecipe : ScriptableObject
{
    public int Id;
    public BagItemData Item1;
    public BagItemData Item2;
    public BagItemData ResultingItem;
    public string Metadata;
}
