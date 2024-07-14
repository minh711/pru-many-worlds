using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    /// <summary>
    /// Character trong túi đồ.
    /// </summary>
    public List<CharacterData> CharacterDataList;
    
    /// <summary>
    /// Character sẽ xuất hiện trong trận (được chọn).
    /// </summary>
    public List<CharacterData> BattleCharacterDataList;

    /// <summary>
    /// Danh sách Item đang có
    /// </summary>
    public List<ItemData> ItemDataList;

    public PlayerData()
    {
        CharacterDataList = new List<CharacterData>();
        BattleCharacterDataList = new List<CharacterData>();
        ItemDataList = new List<ItemData>();
    }

    public int CurrentSceneIndex;

    public float PlayerX;
    public float PlayerY;
}