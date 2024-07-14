using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ActiveSkillConstants;
using static ElementConstants;
using static StatConstants;
using static StatConstants.CharacterStatType;

public class NormalAttack : ActiveSkill
{
    public NormalAttack()
    {
        Name = "Đánh thường";
        SkillElement = (Element)Activator.CreateInstance(ELEMENT_DICTIONARY[ELEMENT_WATER]);
        SkillPriority = (int)SkillPriorities.NORMAL;
        DamageType = DamageType.PHYSIC;
        Power = 1;
    }

    public override BattleDTO SelectTarget(BattleDTO battleInput)
    {
        List<Character> aliveAlly = battleInput.AllyCharacterList.Where(character => character.IsAlive()).ToList();
        List<Character> aliveEnemy = battleInput.EnemyCharacterList.Where(character => character.IsAlive()).ToList();

        BattleDTO battleOutput = new BattleDTO();
        battleOutput.Self = battleInput.Self;
        battleOutput.AllyCharacterList.Clear();
        battleOutput.EnemyCharacterList.Clear();

        if (aliveEnemy == null || aliveEnemy.Count == 0) return battleOutput;
        
        try
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(aliveEnemy.Count);
            battleOutput.EnemyCharacterList.Add(aliveEnemy[randomIndex]);
        }
        catch { }

        return battleOutput;
    }

    /// <summary>
    /// Sử dụng Skill, dựa vào Target đã chọn bằng hàm trước đó (thông qua BattleManager).
    /// </summary>
    /// <param name="targetList">Danh sách Target (Count = 1 nếu là đơn mục tiêu).</param>
    public override List<Character> UseSkill(BattleDTO battleDTO)
    {
        Character self = battleDTO.Self;

        int crit = self.CharacterStats.StatList[(int)CRIT].Current;
        int atk = self.CharacterStats.StatList[(int)CRIT].Current;
        int def = self.CharacterStats.StatList[(int)DEF].Current;
        int acc = self.CharacterStats.StatList[(int)ACC].Current;
        
        int damage = 0;
        bool isCrit = IsCriticalHit(crit);
        if (isCrit) {
            damage = atk * Power * 2;
        } else
        {
            damage = atk * Power * 1;
        }

        float blockAtk = BlockAtkPercentage(def); //return block %
        damage -= (int)(blockAtk * damage);

        Character targetCharacter = battleDTO.EnemyCharacterList[0];
        battleDTO.EnemyCharacterList.RemoveAt(0);

        int enemyEva = targetCharacter.CharacterStats.StatList[(int)EVA].Current;

        bool isDodge = CalculateDodgeRate(acc, enemyEva);
        if (isDodge)
        {
            damage = 0; // If dodge is successful, set damage to 0
        }

        SkillDTO skillDTO = new SkillDTO
        {
            DamageType = (int)DamageType,
            Damage = damage
        };

        targetCharacter.TakeDamage(skillDTO); // Hàm TakeDamage sẽ có Issue sửa sau.

        List<Character> list = new List<Character>();
        list.Add(targetCharacter);
        return list;
    }
}
