using System.Collections.Generic;

public class BattleLoadingInput
{
    /// <summary>
    /// Danh sách Character bên phe đồng minh
    /// </summary>
    public List<Character> AllyCharacterList;

    /// <summary>
    /// Danh sách Character bên phe đối thủ
    /// </summary>
    public List<Character> EnemyCharacterList;

    public List<CharacterData> AllyCharacterDataList;
    public List<CharacterData> EnemyCharacterDataList;
}