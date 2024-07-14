using System.Collections.Generic;

public static class ElementConstants
{
    public static string ELEMENT_FIRE = "ELEMENT_FIRE";
    public static string ELEMENT_WATER = "ELEMENT_WATER";

    public static Dictionary<string, System.Type> ELEMENT_DICTIONARY = new()
    {
        { ELEMENT_FIRE, typeof(Fire) },
        { ELEMENT_WATER, typeof(Water) }
    };
}