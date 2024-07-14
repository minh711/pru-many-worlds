using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Một tập hợp các giá trị cho <b>một chỉ số</b> của một Character.
/// <br/><br/>
/// Bao gồm Base (Gốc), Current (Hiện tại) và Bonus (Chỉ số cộng thêm).
/// </summary>
public class Stat
{
    /// <summary>
    /// Giá trị chỉ số gốc, <b>không thay đổi</b> trong trận.
    /// </summary>
    public int Base { get; set; }

    /// <summary>
    /// Giá trị chỉ số hiện tại, có thể thay đổi trong trận, không thể vượt qua chỉ số gốc.
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// Giá trị chỉ số công thêm, có thể thay đổi trong trận, có thể <b>vượt qua</b> chỉ số gốc.
    /// </summary>
    public int Bonus { get; set; }

    /// <summary>
    /// Stat Constructor
    /// </summary>
    /// <param name="baseValue">Giá trị chỉ số gốc, <b>không thay đổi</b> trong trận.</param>
    /// <param name="currentValue">Giá trị chỉ số hiện tại, có thể thay đổi trong trận, không thể vượt qua chỉ số gốc.</param>
    /// <param name="ghostValue">Giá trị chỉ số cộng thêm, có thể thay đổi trong trận, có thể <b>vượt qua</b> chỉ số gốc.</param>
    public Stat(int baseValue, int currentValue, int ghostValue)
    {
        Base = baseValue;
        Current = currentValue;
        Bonus = ghostValue;
    }
}
