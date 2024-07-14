using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Bộ các chỉ số mặc định cho Character
/// </summary>
[Serializable]
public class CharacterStats
{

    /////////////////////////
    #region Chỉ số khởi đầu (Level 1)

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Health Points
    /// </summary>
    public int baseHP;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Mana Points
    /// </summary>
    public int baseMP;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Attack Power (Physical)
    /// </summary>
    public int baseATK;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Defense (Physical)
    /// </summary>
    public int baseDEF;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Magic Attack Power
    /// </summary>
    public int baseMATK;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Magic Defense
    /// </summary>
    public int baseMDEF;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Critical hit chance
    /// </summary>
    public int baseCRIT;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Speed
    /// </summary>
    public int baseSPD;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Evasion - Tránh né
    /// </summary>
    public int baseEVA;

    /// <summary>
    /// Chỉ số khởi đầu (Level 1)
    /// <br/><br/>
    /// Accuracy - Độ chính xác
    /// </summary>
    public int baseACC;

    #endregion
    /////////////////////////

    ////////////////////////////
    #region Chỉ số cộng thêm khi lên cấp

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Health Points
    /// </summary>
    public int levelUpBonusHP;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Mana Points
    /// </summary>
    public int levelUpBonusMP;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Attack Power (Physical)
    /// </summary>
    public int levelUpBonusATK;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Defense (Physical)
    /// </summary>
    public int levelUpBonusDEF;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Magic Attack Power
    /// </summary>
    public int levelUpBonusMATK;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Magic Defense
    /// </summary>
    public int levelUpBonusMDEF;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Critical hit chance
    /// </summary>
    public int levelUpBonusCRIT;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Speed
    /// </summary>
    public int levelUpBonusSPD;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Evasion - Tránh né
    /// </summary>
    public int levelUpBonusEVA;

    /// <summary>
    /// Chỉ số cộng thêm khi lên cấp
    /// <br/><br/>
    /// Accuracy - Độ chính xác
    /// </summary>
    public int levelUpBonusACC;

    #endregion
    ////////////////////////////

    /// <summary>
    /// List chứa các chỉ số cơ bản.
    /// <br/><br/>
    /// Truy cập thông qua index từ <b>StatConstants</b>.
    /// </summary>
    public List<Stat> StatList { get; set; }

    /// <summary>
    /// Khiên ảo (chỉ áp dụng trong trận), chặn được cả sát thương vật lý và sát thương phép thuật.
    /// </summary>
    public int Shield { get; set; }

    /// <summary>
    ///  Tổng điểm kinh nghiệm
    /// </summary>
    public int Exp { get; set; }

    /// <summary>
    /// Level tính dựa theo <b>Tổng số kinh nghiệm</b> hiện có.
    /// </summary>
    public int Level
    {
        get
        {
            return CalculateLevel();
        }
    }

    /// <summary>
    /// Điểm kinh nghiệm cần để lên cấp tiếp theo.
    /// </summary>
    public int ExpForNextLevel
    {
        get
        {
            return CalulateExpForNextLevel();
        }
    }

    public void Initialize(int exp)
    {
        Shield = 0;
        Exp = exp;

        int HP   = CalculateStat(  baseHP,   levelUpBonusHP);
        int MP   = CalculateStat(  baseMP,   levelUpBonusMP);
        int DEF  = CalculateStat( baseDEF,  levelUpBonusDEF);
        int ATK  = CalculateStat( baseATK,  levelUpBonusATK);
        int MATK = CalculateStat(baseMATK, levelUpBonusMATK);
        int MDEF = CalculateStat(baseMDEF, levelUpBonusMDEF);
        int CRIT = CalculateStat(baseCRIT, levelUpBonusCRIT);
        int SPD  = CalculateStat( baseSPD,  levelUpBonusSPD);
        int EVA  = CalculateStat( baseEVA,  levelUpBonusEVA);
        int ACC  = CalculateStat( baseACC,  levelUpBonusACC);

        StatList = new List<Stat>
        {
            new(  HP,   HP, 0),     // Health Points
            new(  MP,   MP, 0),     // Mana Points
            new( DEF,  DEF, 0),   // Defense (Physical)
            new( ATK,  ATK, 0),   // Attack Power (Physical)
            new(MATK, MATK, 0), // Magic Attack Power
            new(MDEF, MDEF, 0), // Magic Defense
            new(CRIT, CRIT, 0), // Critical hit chance
            new( SPD,  SPD, 0),   // Speed
            new( EVA,  EVA, 0),   // Evasion - Tránh né
            new( ACC,  ACC, 0)    // Accuracy - Độ chính xác
        };
    }

    /// <summary>
    /// Tính chỉ số hiện tại dụa trên <b>Chỉ số khởi đầu</b> và <b>Level</b>.
    /// </summary>
    /// <param name="baseValue">Chỉ số khởi đầu</param>
    /// <param name="levelUpBonusValue">Level</param>
    /// <returns>Chỉ số hiện tại</returns>
    private int CalculateStat(int baseValue, int levelUpBonusValue)
    {
        return baseValue + levelUpBonusValue * Level;
    }

    /// <summary>
    /// Tính Level dựa trên <b>tổng số kinh nghiệm</b> đang có.
    /// <br/><br/>
    /// Nhằm tăng tính thử thách, không giới hạn Level cho Boss và trong một vài ải nhất định.
    /// </summary>
    /// <param name="exp">Tổng số kinh nghiệm đang có</param>
    /// <returns>
    /// Level hiện tại, giới hạn mặc định là 100, có thể cao hơn tùy trường hợp.
    /// <br/>
    /// </returns>
    private int CalculateLevel()
    {
        if (Exp <= 0) return 1;
        int a = 0, b = 1, level = 1;
        int nextExp = a + b;

        while (nextExp <= Exp)
        {
            a = b;
            b = nextExp;
            nextExp = a + b;
            level++;
        }
        return level;
    }

    /// <summary>
    /// Tính <b>tổng số kinh nghiệm</b> cần có để lên Level tiếp theo.
    /// <br/><br/>
    /// </summary>
    /// <param name="level">Level hiện tại</param>
    /// <returns>
    /// Tổng số kinh nghiệm cần có để lên Level tiếp theo.
    /// <br/><br/>
    /// Trong trường hợp Level Max (100), trả về <b>0</b>.
    /// </returns>
    public int CalulateExpForNextLevel()
    {
        if (Exp <= 0)
        {
            return 1;
        }
        int a = 0, b = 1, nextExp = 1;
        while (nextExp <= Exp) 
        {
            nextExp = a + b;
            a = b;
            b = nextExp;
        }
        return nextExp;
    }
}