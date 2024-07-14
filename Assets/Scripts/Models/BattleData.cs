using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Gắn vào enemy để lấy dữ liệu của Battle
/// </summary>
/// 
[CreateAssetMenu(fileName = "Batte Name", menuName = "BattleData")]
public class BattleData : ScriptableObject
{
    /// <summary>
    /// Những phase của battle, mỗi phase sẽ có lượng quái khác nhau.
    /// <br/>
    /// Thông thường chỉ cần 1 phase.
    /// </summary>
    public List<BattlePhase> BattlePhases;

    public List<Reward> RewardList;

    protected virtual void Initialize()
    {

    }

    public BattleData Clone()
    {
        BattleData clone = ScriptableObject.CreateInstance<BattleData>();
        return clone;
    }


    public List<Reward> GetReward()
    {
        List<Reward> itemReward = new List<Reward>();
        foreach (Reward reward in RewardList)
        {
            int quantity = 0;
            switch (reward.Rarelity)
            {
                case RewardConstants.RarelityType.G:
                    quantity = UnityEngine.Random.Range(reward.Min, reward.Max);
                    reward.Quantity = quantity;
                    itemReward.Add(reward);
                    break;
                case RewardConstants.RarelityType.R:
                    System.Random random2 = new System.Random();
                    double roll2 = random2.NextDouble() * 100;
                    if (roll2 < 50)
                    {
                        quantity = UnityEngine.Random.Range(reward.Min, reward.Max);
                        reward.Quantity = quantity;
                        itemReward.Add(reward);
                    }
                    break;
                case RewardConstants.RarelityType.SR:
                    System.Random random3 = new System.Random();
                    double roll3 = random3.NextDouble() * 100;
                    if (roll3 <= 20)
                    {
                        quantity = UnityEngine.Random.Range(reward.Min, reward.Max);
                        reward.Quantity = quantity;
                        itemReward.Add(reward);
                    }
                    break;
                case RewardConstants.RarelityType.UR:
                    System.Random random4 = new System.Random();
                    double roll4 = random4.NextDouble() * 100;
                    if (roll4 <= 5)
                    {
                        quantity = UnityEngine.Random.Range(reward.Min, reward.Max);
                        reward.Quantity = quantity;
                        itemReward.Add(reward);
                    }
                    break;
            }
        }
        return itemReward;
    }
}
