using System.Collections.Generic;

public static class RewardConstants
{
    public enum RewardType : int
    {
        ITEM = 0,
        CHARACTER = 1
    }

    public enum ItemTypes : int
    {
        CONSUMABLE = 0,
        EQUIPMENT = 1,
        QUEST_ITEM = 2,
        EXP_ITEM = 3
    }

    public enum RarelityType : int
    {
        G = 0,
        R = 1,
        SR = 2,
        UR = 3,
    }
}
