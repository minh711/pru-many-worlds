using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item: ScriptableObject
{
    #region Thông tin cơ bản
    /// <summary>
    /// Also is the <b>Prefab</b>'s name.
    /// </summary>
    public string ItemCode;

    /// <summary>
    /// Số thứ tự trong kho đồ, dùng để Load và Save game.
    /// </summary>
    public int InventoryIndex;

    /// <summary>
    /// Tên của Item.
    /// </summary>
    public string Name;

    /// <summary>
    /// Mô tả ngắn gọn, liên quan tới Cốt truyện.
    /// </summary>
    public string Description;
    #endregion

    #region Phục vụ Loading

    /// <summary>
    /// Item Sprite Path to load.
    /// </summary>
    public string ItemSpritePath;

    /// <summary>
    /// Item Sprite to store after load.
    /// </summary>
    public Sprite ItemSprite;

    #endregion

    public int Quantity;

    public int Type;
}
