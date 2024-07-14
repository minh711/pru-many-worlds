using System.Collections.Generic;

public static class ItemConstants
{
    public const string EXP_SMALL = "ExpSmall";
    public const string EXP_MEDIUM = "ExpMedium";
    public const string EXP_LARGE = "ExpLarge";

    public static Dictionary<string, string> ITEM_ASSET_DICTIONARY = new()
    {
        { EXP_SMALL, "Assets/ScriptableObjects/Items/ExpItems/ExpSmall.asset"},
        { EXP_MEDIUM, "Assets/ScriptableObjects/Items/ExpItems/ExpMedium.asset" },
        { EXP_LARGE, "Assets/ScriptableObjects/Items/ExpItems/ExpLarge.asset" }
    };

    public enum ItemTypes : int
    {
        CONSUMABLE = 0,
        EQUIPMENT = 1,
        QUEST_ITEM = 2,
        EXP_ITEM = 3
    }
}
