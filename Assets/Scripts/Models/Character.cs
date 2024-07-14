using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static ActiveSkillConstants;
using static StatConstants;

/// <summary>
/// <b>Character</b> - Character for Battles.
/// </summary>
/// 
[CreateAssetMenu(fileName = "Character Name", menuName = "Character")]
public class Character : ScriptableObject
{
    #region Thông tin cơ bản

    /// <summary>
    /// Also is the <b>Prefab</b>'s name.
    /// </summary>
    public string CharacterCode;

    /// <summary>
    /// Số thứ tự trong kho đồ, dùng để Load và Save game.
    /// </summary>
    public int InventoryIndex;

    /// <summary>
    /// Tên của Character.
    /// </summary>
    public string Name;

    /// <summary>
    /// Mô tả ngắn gọn, liên quan tới Cốt truyện.
    /// </summary>
    public string Description;

    #endregion

    #region Chỉ số

    /// <summary>
    /// Bộ các chỉ số cơ bản (HP, MP ...).
    /// </summary>
    public CharacterStats CharacterStats;

    /// <summary>
    /// Nguyên tố, ảnh hưởng đến hiệu quả của Stat và Skill.
    /// </summary>
    public Element Element { get; set; }

    /// <summary>
    /// Tổng số lượng Exp
    /// </summary>
    public int Exp;

    #endregion

    #region Phục vụ Loading

    /// <summary>
    /// Path để load cho CharacterGameObject
    /// </summary>
    public string PrefabPath;

    /// <summary>
    /// Item Sprite Path to load.
    /// </summary>
    public string ItemSpritePath;

    /// <summary>
    /// Item Sprite to store after load.
    /// </summary>
    public Sprite ItemSprite;

    /// <summary>
    /// Lấy từ <b>Prefab</b> để cung cấp đồ họa, animation ...
    /// <br/><br/>
    /// Đồng thời cũng là Object được <b>xuất hiện</b> trên màn hình.
    /// </summary>
    public GameObject CharacterPrefab { get; set; }

    #endregion

    #region Phục vụ Battle

    /// <summary>
    /// Vị trí sẽ được <b>Spawn</b> trên màn hình (0 - 8).
    /// </summary>
    public int SpawnLocation { get; set; }

    public bool IsInTurn { get; set; }

    /// <summary>
    /// Transform của <b>Spawn Point</b> trên màn hình
    /// <br/><br/>
    /// Chứa thanh HP, ...
    /// </summary>
    public Transform SpawnPoint { get; set; }

    /// <summary>
    /// TODO: Temp
    /// </summary>
    public TextMeshProUGUI TextHP { get; set; }
    public Transform HealthBar { get; set; }


    #endregion

    #region Skils Handler

    /// <summary>
    /// Code để load skill, chỉnh trong ScriptableObject
    /// </summary>
    public List<string> ActiveSkillCodeList;

    /// <summary>
    /// Danh sách Skill chủ động
    /// </summary>
    public List<ActiveSkill> ActiveSkillList { get; set; }

    #endregion

    #region TODO: Not used yet 

    /// <summary>
    /// Danh sách Skill bị động
    /// </summary>
    public List<PassiveSkill> PassiveSkillList { get; set; }

    /// <summary>
    /// Danh sách Buff
    /// </summary>
    public List<Buff> BuffList { get; set; }
    public List<int> SelectedActiveSkillList { get; set; }
    public List<int> SelectedPassiveSkillList { get; set; }
    public List<int> SelectedBuffList { get; set; }
    public List<Character> AllyList { get; set; }
    public List<Character> EnemyList { get; set; }
    public List<int> TargetAllyList { get; set; }
    public List<int> TargetEnemyList { get; set; }

    #endregion

    public virtual void Initialize()
    {
        CharacterStats.Initialize(Exp);

        ActiveSkillList = new List<ActiveSkill>();

        LoadSkills();

        IsInTurn = true;
    }

    private void LoadSkills()
    {
        foreach(string skillCode in ActiveSkillCodeList)
        {
            ActiveSkill skill = (ActiveSkill)Activator.CreateInstance(ACTIVE_SKILL_DICTIONARY[skillCode]);
            ActiveSkillList.Add(skill);
        }
    }

    public virtual bool IsAlive()
    {
        bool isAlive = true;

        ShowTextHP();
        Debug.Log($"<color=orange>Check Alive {Name}: {CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current}</color>");
        if (CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current <= 0)
        {
            isAlive = false;
        }

        return isAlive;
    }

    public virtual void TakeDamage(SkillDTO skillDTO)
    {
        int damage = skillDTO.Damage;

        CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current -= damage;
        if (CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current < 0)
        {
            CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current = 0;
        }
        Debug.Log($"{Name} đã nhận {damage} sát thương");
        ShowTextHP();
    }

    /// <summary>
    /// Update a Stat
    /// </summary>
    /// <param name="statType">Stat Value Type</param>
    /// <param name="valueType">
    ///     Value Type (Base, Current, or Bonus?) 
    ///     <br/>
    ///     Use <b>StatConstants</b>
    /// </param>
    /// <param name="amount">Amount changed, "+" for increase, "-" for decrease</param>
    public virtual void UpdateStat(int statType, int valueType, int amount)
    {
        switch (valueType)
        {
            case (int)StatConstants.StatValueType.BASE:
                CharacterStats.StatList[statType].Base += amount;
                break;
            case (int)StatConstants.StatValueType.CURRENT:
                CharacterStats.StatList[statType].Current += amount;
                break;
            case (int)StatConstants.StatValueType.BONUS:
                CharacterStats.StatList[statType].Bonus += amount;
                break;
        }
    }

    public virtual ActiveSkill UseActiveSkill()
    {
        foreach (ActiveSkill skill in ActiveSkillList)
        {
            if (skill.CanUseSkill(this))
            {
                return skill;
            }
        }
        //IsInTurn = false;
        return null;
    }

    public virtual void UsePassiveSkill()
    {
        Debug.Log("<color=red>Not implemented yet</color");
    }

    public virtual void UseBuff()
    {
        Debug.Log("<color=red>Not implemented yet</color");
    }

    /// <summary>
    /// <b>Spawn</b> Character lên trên màn hình dựa vào Spawn Point.
    /// </summary>
    /// <param name="character">Character cần Spawn.</param>
    /// <param name="spawnPoint">Spawn Point sẽ vẽ lên.</param>
    /// <param name="isOpposite">Phe địch hay phe ta, dùng để biết quay mặt đi hướng nào</param>
    public void SpawnCharacter(Transform spawnPoint, bool isOpposite)
    {
        CharacterPrefab = Instantiate(CharacterPrefab, spawnPoint.position, spawnPoint.rotation);

        if (isOpposite)
        {
            // Lật mặt
            CharacterPrefab.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public void ShowTextHP()
    {
        Debug.Log($"{Name}: {CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current}/{CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Base}");
        HealthBar.GetComponent<Image>().fillAmount = 
            CharacterStats.StatList[(int)CharacterStatType.HP].Current 
            / (float)CharacterStats.StatList[(int)CharacterStatType.HP].Base;
        TextHP.text =
            $"{CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current}" +
            $"/" +
            $"{CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Base}";
    }
}
