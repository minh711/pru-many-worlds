using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public static string ToJson<T>(List<T> list, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T> { Items = list };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
}
