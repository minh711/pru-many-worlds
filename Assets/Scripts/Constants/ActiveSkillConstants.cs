using System.Collections.Generic;

public static class ActiveSkillConstants
{
    public static string NORMAL_ATTACK = "NORMAL_ATTACK";

    public enum SkillPriorities: int
    {
        DO_FIRST = 0,
        NORMAL = 1,
        DO_LAST = 2
    }

    public enum DamageType
    {
        PHYSIC,
        MAGIC
    }

    public static Dictionary<string, System.Type> ACTIVE_SKILL_DICTIONARY = new()
    {
        { NORMAL_ATTACK, typeof(NormalAttack) }
    };
}
