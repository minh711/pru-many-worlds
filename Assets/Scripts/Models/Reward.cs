using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Reward
{
    public int Min;
    public int Max;
    public int Quantity;
    public RewardConstants.RarelityType Rarelity;
    public RewardConstants.RewardType RewardType;
    public string RewardCode;
    public string Name;
}
