using System.Collections.Generic;
using UnityEngine;

public class BattleDTO
{
    /// <summary>
    /// Object của Character đang thi triển chiêu thức.
    /// </summary>
    public Character Self;

    /// <summary>
    /// Danh sách Character bên phe đồng minh.
    /// <br/><br/>
    /// Bao gồm cả bản thân.
    /// </summary>
    public List<Character> AllyCharacterList;

    /// <summary>
    /// Danh sách Character bên phe đối thủ
    /// </summary>
    public List<Character> EnemyCharacterList;

    /// <summary>
    /// Attack effect để show (nếu có)
    /// </summary>
    public GameObject AttackEffect;

    public BattleDTO()
    {
        AllyCharacterList = new List<Character>();
        EnemyCharacterList = new List<Character>();
    }
}