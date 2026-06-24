using System;
using System.Collections.Generic;

public class DeploymentResult
{
    public DateTime FinishTime { get; set; }
    public ICollection<int> FoundItems { get; set; }
    public int ExpFarmed { get; set; }
}
