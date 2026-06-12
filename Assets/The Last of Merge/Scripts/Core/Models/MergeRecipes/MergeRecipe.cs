using UnityEngine;

[CreateAssetMenu(fileName = "MergeRecipe", menuName = "Scriptable Objects/MergeRecipe")]
public class MergeRecipe : ScriptableObject
{
    public int Id;
    public int ItemId1;
    public int ItemId2;
    public int ResultingItemId;
    public string Metadata;
}
