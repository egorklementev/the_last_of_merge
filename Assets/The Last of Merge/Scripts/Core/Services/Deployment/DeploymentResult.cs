using System.Collections.Generic;

public struct DeploymentResult
{
    public float HpDamaged;
    public float ExpEarned;
    public IList<BagItemData> FoundItems;
    public IList<BagItemData> UsedEuippedItems;
}
