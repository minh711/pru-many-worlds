using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Stat Types & Stat Value Types
/// </summary>
public static class StatConstants
{
    /// <summary>
    /// Base Stat Types
    /// </summary>
    public enum CharacterStatType : int
    {
        /// <summary>
        /// Health Points
        /// </summary>
        HP = 0,

        /// <summary>
        /// Mana Points
        /// </summary>
        MP = 1,

        /// <summary>
        /// Attack Power (Physical)
        /// </summary>
        ATK = 2,

        /// <summary>
        /// Defense (Physical)
        /// </summary>
        DEF = 3,

        /// <summary>
        /// Magic Attack Power
        /// </summary>
        MATK = 4,

        /// <summary>
        /// Magic Defense
        /// </summary>
        MDEF = 5,

        /// <summary>
        /// Critical hit chance
        /// </summary>
        CRIT = 6,

        /// <summary>
        /// Speed
        /// </summary>
        SPD = 7,

        /// <summary>
        /// Evasion - Tránh né
        /// </summary>
        EVA = 8,

        /// <summary>
        /// Accuracy - Độ chính xác
        /// </summary>
        ACC = 9
    }

    /// <summary>
    /// Stat Value Types
    /// </summary>
    public enum StatValueType : int
    {
        /// <summary>
        /// Giá trị chỉ số gốc, <b>không thay đổi</b> trong trận
        /// </summary>
        BASE = 0,

        /// <summary>
        /// Giá trị chỉ số hiện tại, có thể thay đổi trong trận, không thể vượt qua chỉ số gốc
        /// </summary>
        CURRENT = 1,

        /// <summary>
        /// Giá trị chỉ số ảo, có thể <b>vượt qua</b> chỉ số gốc
        /// </summary>
        BONUS = 2
    }
}
