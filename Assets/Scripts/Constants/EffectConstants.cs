using System.Collections.Generic;

public static class EffectConstants
{
    public const string EFFECT_EXPLOSION = "EffectExplosion";

    public static Dictionary<string, string> EFFECT_DICTIONARY = new()
    {
        { EFFECT_EXPLOSION, "Assets/Prefabs/Effects/EffectExplosion.prefab" }
    };
}
