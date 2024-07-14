[System.Serializable]
public class CharacterData
{
    /// <summary>
    /// Vị trí trong túi đồ
    /// </summary>
    public int InventoryIndex;

    /// <summary>
    /// Code để Load assets
    /// </summary>
    public string CharacterCode;

    /// <summary>
    /// Exp dùng để tính Level
    /// </summary>
    public int CharacterExp;

    /// <summary>
    /// Ví trị xuất hiện trong trận đấu (0 - 8).
    /// <br/><br/>
    /// Nếu là '-1' tức là không được chọn. 
    /// </summary>
    public int SpawnLocation = -1;
}