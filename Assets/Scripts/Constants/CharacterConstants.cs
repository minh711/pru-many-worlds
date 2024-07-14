using System.Collections.Generic;

public static class CharacterConstants
{
    public const string AVOCADO = "Avocado";
    public const string PEPPER = "Pepper";
    public const string RATCHER = "Ratcher";
    public const string CAT = "Cat";
    public const string FOX = "Fox";
    public const string GUARD = "Guard";
    public const string WITCH = "Witch";
    public const string CRAB = "Crab";
    public const string DEMON = "Demon";
    public const string SLIME = "Slime";
    public const string SHEEP = "Sheep";

    public static Dictionary<string, string> CHARACTER_ASSET_DICTIONARY = new()
    {
        { AVOCADO, "Assets/_Scripts/Characters/Avocado.asset"},
        { PEPPER, "Assets/_Scripts/Characters/Pepper.asset" },
        { RATCHER, "Assets/ScriptableObjects/Characters/Ratcher.asset" },
        { CAT, "Assets/ScriptableObjects/Characters/Cat.asset" },
        { FOX, "Assets/ScriptableObjects/Characters/Fox.asset" },
        { GUARD, "Assets/ScriptableObjects/Characters/Guard.asset"},
        { WITCH, "Assets/ScriptableObjects/Characters/Witch.asset"},
        { CRAB, "Assets/ScriptableObjects/Characters/Crab.asset"},
        { DEMON, "Assets/ScriptableObjects/Characters/Demon.asset"},
        { SLIME, "Assets/ScriptableObjects/Characters/Slime.asset"},
        { SHEEP, "Assets/ScriptableObjects/Characters/Sheep.asset" }
    };
}
