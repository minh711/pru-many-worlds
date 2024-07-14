using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static ActiveSkillConstants;
using static StatConstants;


/// <summary>
/// <b>Active Skill</b> - Skill chủ động
/// <br/>
/// Phân biệt với Skill <b>Bị động</b> vì điều kiện sẽ khác
/// </summary>
public class ActiveSkill
{
    /// <summary>
    /// Tên của Skill
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Nguyên tố của Skill
    /// <br/><br/>
    /// Dùng để thay đổi sát thương, hiệu ứng khi khắc hệ. TODO: In Development ...
    /// </summary>
    public Element SkillElement;

    /// <summary>
    /// Độ ưu tiên của Skill? TODO: In Development ...
    /// </summary>
    public int SkillPriority;

    /// <summary>
    /// Loại sát thương - Vật lý hay Phép. TODO: In Development
    /// </summary>
    public DamageType DamageType;

    /// <summary>
    /// Sức mạnh của Skill, xử lý với ATK của Character để ra sát thương gây ra.
    /// </summary>
    public int Power;

    /// <summary>
    /// Check if Character can use this Skill.
    /// </summary>
    /// <param name="character">Character using this Skill.</param>
    /// <returns>Yes or No</returns>
    public virtual bool CanUseSkill(Character character)
    {
        return character.CharacterStats.StatList[(int)CharacterStatType.HP].Current > 0;
    }

    /// <summary>
    /// Chọn đối tượng để thi triển Skill. Mặc định là List (Count = 1 nếu là đơn mục tiêu).
    /// <br/><br/>
    /// Có thể thi triển lên đồng minh (ví dụ: hồi máu).
    /// </summary>
    /// <param name="allyCharacterList">Danh sách đồng minh (Đối với người thi triển, có thể bao gồm bản thân).</param>
    /// <param name="enemyCharacterList">Danh sách kẻ địch (Đối với người thi triển).</param>
    /// <returns>Đối tượng thi triển Skill, để hàm UseSkill xử lý.</returns>
    public virtual BattleDTO SelectTarget(BattleDTO battleDTO)
    {
        UnityEngine.Debug.Log("Called here");
        return null;
    }

    /// <summary>
    /// Sử dụng Skill, sẽ được class Skill kế thừa override.
    /// </summary>
    /// <param name="battleDTO">Biến thông tin từ Battle truyền vào</param>
    public virtual List<Character> UseSkill(BattleDTO battleDTO)
    {
        return null;
    }

    public virtual bool IsCriticalHit(int critValue)
    {
        System.Random random = new System.Random();

        double critChance = (critValue / 1000.0) * 100;
        double roll = random.NextDouble() * 100;

        return roll <= critChance;
    }

    public virtual float BlockAtkPercentage(int def)
    {
        return def / 1000f;
    }

    public virtual bool CalculateDodgeRate(int evasionValue, int accuracyValue)
    {
        // Calculate the dodge rate by subtracting the dodge value from the accuracy value and dividing by 1000
        float dodgeRate = (accuracyValue - evasionValue) / 1000f;

        // Generate a random number between 0 and 1
        float randomValue = (float)new System.Random().NextDouble();

        // If the random value is less than the dodge rate, return true (dodge successful)
        return randomValue < dodgeRate;
    }

}
