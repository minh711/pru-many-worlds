using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryConstants
{
    /// <summary>
    /// Số lượng item tối đa hiển thị trong inventory
    /// </summary>
    public static int INVENTORY_MAX_ITEM_PER_PAGE = 16;

    public enum InventoryCharacterStatus : int
    {
        SELECTING = 0,
        SELECTED = 1,
        NO_SELECT = 2
    }
}
