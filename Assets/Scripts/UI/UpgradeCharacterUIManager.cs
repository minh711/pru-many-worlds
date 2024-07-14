using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InventoryConstants;
using static InventoryConstants.InventoryCharacterStatus;

public class UpgradeCharacterUIManager : MonoBehaviour
{
    #region Biến để xử lý UI

    [SerializeField]
    private GameObject MainCanvas;

    public Character SelectingCharacter { get; set; }

    private List<Transform> PanelItemList;
    private Transform FunctionPanel;
    private Slider QuantitySlider;
    private TextMeshProUGUI QuantitySliderText;
    private Transform SelectingItemPanel;
    private Transform SelectingCharacterPanel;
    private TextMeshProUGUI CharacterLevelText;
    private TextMeshProUGUI CharacterNameText;
    private TextMeshProUGUI CharacterExpText;
    private TextMeshProUGUI ExpValueText;
    private TextMeshProUGUI LevelUpText;

    #endregion

    #region Buttons

    private Button BtnUpgrade;
    
    #endregion
    
    #region Assets đã load

    private List<ItemExp> LoadedItemList;

    #endregion

    #region Xử lý Item Inventory

    /// <summary>
    /// Chacracter đang chọn (khi bấm vào một Character trên UI)
    /// </summary>
    private ItemExp SelectingItem;

    /// <summary>
    /// Phân trang trong inventory
    /// </summary>
    private int ItemOffset;

    private int ItemLimit;

    #endregion

    #region Biến để xử lý upgrade

    /// <summary>
    /// Giá trị hiện tại của Slider
    /// </summary>
    private int SelectingValue;

    #endregion

    void Awake()
    {
        LoadedItemList = new List<ItemExp>();
        PanelItemList = new List<Transform>();
    }

    async void Start()
    {
        ExtractMainPanel();

        #region Display Item List

        int totalItems = GameData.Instance.GetTotalItems();

        ItemOffset = 0;

        // limit bình thường là 6, nhưng trong trường hợp không đủ 6 item thì sẽ lấy số nhỏ hơn
        ItemLimit =
        (
            totalItems <= 6
            ? totalItems
            : (ItemOffset + 6)
        );

        await DisplayItemList(ItemOffset, ItemLimit);

        #endregion

        HandleButtons();
    }

    private void ExtractMainPanel()
    {
        Transform ItemInventoryPanel = MainCanvas.transform.GetChild(0).GetChild(1);

        FunctionPanel = MainCanvas.transform.Find("UpgradeCharacterPanel").Find("FunctionPanel");
        QuantitySlider = FunctionPanel.Find("QuantitySlider").GetComponent<Slider>();
        QuantitySlider.value = 0f;
        QuantitySlider.onValueChanged.AddListener(OnSliderValueChanged);
        QuantitySliderText = FunctionPanel.Find("QuantitySliderText").GetComponent<TextMeshProUGUI>();
        if (SelectingItem == null)
        {
            QuantitySlider.interactable = false;
        }
        QuantitySliderText.text = $"{0}/{(SelectingItem != null ? SelectingItem.Quantity : 0)}";
        SelectingItemPanel = MainCanvas.transform.Find("UpgradeCharacterPanel").Find("SelectingItemPanel");
        SelectingItemPanel.Find("Button").GetComponent<Button>().interactable = false;

        SelectingCharacterPanel = MainCanvas.transform
            .Find("UpgradeCharacterPanel")
            .Find("CharacterDisplayPanel")
            .Find("CharacterItem");

        SelectingCharacterPanel.Find("Button").GetComponent<Button>().interactable = false;

        CharacterLevelText = FunctionPanel.Find("CharacterLevelText").GetComponent<TextMeshProUGUI>();
        CharacterNameText = FunctionPanel.Find("CharacterNameText").GetComponent<TextMeshProUGUI>();
        CharacterExpText = FunctionPanel.Find("CharacterExpText").GetComponent<TextMeshProUGUI>(); ;
        ExpValueText = FunctionPanel.Find("ExpValueText").GetComponent<TextMeshProUGUI>(); ;
        LevelUpText = FunctionPanel.Find("LevelUpText").GetComponent<TextMeshProUGUI>(); ;
        
        BtnUpgrade = FunctionPanel.Find("BtnUpgrade").GetComponent<Button>();
        BtnUpgrade.onClick.AddListener(() => OnBtnUpgradeClick());

        // The index 0 is the background, so from 1 to 6
        for (int i = 1; i <= 6; i++)
        {
            Transform item = ItemInventoryPanel.GetChild(i);
            PanelItemList.Add(item);
        }
    }

    private async void OnBtnUpgradeClick()
    {
        if (SelectingValue == 0)
        {
            return;
        }

        // Calculate the bonus experience
        int bonusExp = SelectingValue * SelectingItem.ExpBonus;

        // Apply the experience to the character and decrease the item quantity
        SelectingCharacter.Exp += bonusExp;
        SelectingItem.Quantity -= SelectingValue;

        if (SelectingItem.Quantity == 0)
        {
            SelectingValue = 0;
            QuantitySlider.value = 0f;
            QuantitySlider.interactable = false;
            ItemData itemToRemove = GameData.Instance.PlayerData.ItemDataList.Find(item => item.InventoryIndex == SelectingItem.InventoryIndex);
            int removedInventoryIndex = itemToRemove.InventoryIndex;
            GameData.Instance.PlayerData.ItemDataList.Remove(itemToRemove);
            foreach (ItemData item in GameData.Instance.PlayerData.ItemDataList)
            { 
                if (item.InventoryIndex > removedInventoryIndex)
                {
                    item.InventoryIndex--;
                }
            }
        } else if (SelectingValue > SelectingItem.Quantity)
        {
            SelectingValue = SelectingItem.Quantity;
            QuantitySlider.value = 1f;
        } else
        {
            QuantitySlider.value = SelectingValue / (float)SelectingItem.Quantity;
        }

        // Refresh the UI or any necessary components
        Refresh();

        QuantitySliderText.text = $"{SelectingValue}/{SelectingItem.Quantity}";
        ExpValueText.text =
            $"Tăng " +
            $"{SelectingValue}x{SelectingItem.ExpBonus}" +
            $" = " +
            $"{SelectingValue * SelectingItem.ExpBonus}" +
            $" EXP";

        UpdateCharacterData();

        await DisplayItemList(ItemOffset, ItemLimit);
    }

    public void Refresh()
    {
        SelectingCharacter.Initialize();
        Transform panelItemDisplay = SelectingCharacterPanel.GetChild(0);
        Sprite itemItemSprite = SelectingCharacter.ItemSprite;
        RenderHelper renderHelper = GetRenderHelper();
        renderHelper.DrawSprite(itemItemSprite, panelItemDisplay);

        CharacterLevelText.text = $"LV.{SelectingCharacter.CharacterStats.Level}";
        CharacterNameText.text = $"{SelectingCharacter.Name}";
        CharacterExpText.text =
            $"{SelectingCharacter.CharacterStats.Exp}" +
            $"/" +
            $"{SelectingCharacter.CharacterStats.ExpForNextLevel + SelectingCharacter.CharacterStats.Exp}" +
            $" EXP";
    }

    #region Methods xử lý nút bấm

    private void OnSliderValueChanged(float value)
    {
        SelectingValue = Mathf.FloorToInt(value * SelectingItem.Quantity);
        QuantitySliderText.text = $"{SelectingValue}/{SelectingItem.Quantity}";
        ExpValueText.text =
            $"Tăng " +
            $"{SelectingValue}x{SelectingItem.ExpBonus}" +
            $" = " +
            $"{SelectingValue * SelectingItem.ExpBonus}" +
            $" EXP";
    }

    private void HandleButtons()
    {
        for (int i = 0; i < 6; i++)
        {
            Button button = PanelItemList[i]
               .GetChild(1).GetComponent<Button>();
            button.onClick.AddListener(() => OnItemButtonClick(button));
        }
    }

    private void OnItemButtonClick(Button clickedButton)
    {
        // index 0 is the background so the sibling indexes are from 1 to 6, but the index for code is from 0 to 5
        int index = clickedButton.transform.parent.GetSiblingIndex() - 1; 

        // If click on empty button, return.
        if (index >= LoadedItemList.Count)
        {
            return;
        }

        SelectingItem = LoadedItemList
            .FirstOrDefault(item => (item.InventoryIndex % 6) == index);

        ExpValueText.text = $"Tăng 0*{SelectingItem.ExpBonus} = 0 EXP";

        DisplaySelectedItem();
    }

    #endregion

    #region Methods xử lý hiển thị trạng thái item trong Inventory

    #endregion

    #region Methods xử lý Display

    private async Task DisplayItemList(int offset, int limit)
    {
        LoadingHandler loadingHandler = LoadingHandler.Instance;

        LoadedItemList.Clear();

        await loadingHandler.LoadItemList(LoadedItemList, offset, limit);
        await loadingHandler.LoadItemItemSprite(LoadedItemList);

        RenderHelper renderHelper = GetRenderHelper();

        // clear the current display
        foreach(Transform panelSlot in PanelItemList)
        {
            Transform panelItemDisplay = panelSlot.GetChild(0);
            renderHelper.DrawSprite(null, panelItemDisplay);
        }

        int slot = 0;
        foreach (Item item in LoadedItemList)
        {
            Transform panelSlot = PanelItemList[slot];
            Transform panelItemDisplay = panelSlot.GetChild(0);
            Transform button = panelSlot.GetChild(1);

            Sprite characterItemSprite = item.ItemSprite;
            renderHelper.DrawSprite(characterItemSprite, panelItemDisplay);

            slot++;
        }

        for (int i = 0; i < 6; i++)
        {
            Button button = PanelItemList[i]
               .GetChild(1).GetComponent<Button>();
            if (!LoadedItemList.Any(character => (character.InventoryIndex % 16) == i))
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
    }

    private void DisplaySelectedItem()
    {
        Transform panelItemDisplay = SelectingItemPanel.GetChild(0);
        Sprite itemItemSprite = SelectingItem.ItemSprite;
        RenderHelper renderHelper = GetRenderHelper();
        renderHelper.DrawSprite(itemItemSprite, panelItemDisplay);
        QuantitySliderText.text = $"{0}/{SelectingItem.Quantity}";
        QuantitySlider.interactable = true;
        QuantitySlider.value = 0f;
    }

    #endregion

    private void UpdateCharacterData()
    {
        List<CharacterData> battleCharacterData = GameData.Instance.PlayerData.BattleCharacterDataList;
        List<CharacterData> inventoryCharacterData = GameData.Instance.PlayerData.CharacterDataList;

        CharacterData newBattleCharacterData = new CharacterData()
        {
            SpawnLocation = SelectingCharacter.SpawnLocation,
            CharacterCode = SelectingCharacter.CharacterCode,
            CharacterExp = SelectingCharacter.Exp,
            InventoryIndex = SelectingCharacter.InventoryIndex
        };

        // Kiểm tra xem Location đó đã có Character Data nào chưa
        bool isExist = battleCharacterData
            .Any(battleCharacterData => battleCharacterData.SpawnLocation == SelectingCharacter.SpawnLocation);
        if (isExist)
        {
            CharacterData selectedCharacterData = battleCharacterData
                .FirstOrDefault(battleCharacterData => battleCharacterData.SpawnLocation == SelectingCharacter.SpawnLocation);
            // Remove Character cũ
            battleCharacterData.Remove(selectedCharacterData);
            battleCharacterData.Add(newBattleCharacterData);
        }

        // Xử lý trong inventory
        CharacterData currentCharacterData = inventoryCharacterData.Find(character => character.InventoryIndex == SelectingCharacter.InventoryIndex);
        inventoryCharacterData.Remove(currentCharacterData);
        inventoryCharacterData.Add(newBattleCharacterData);


    }

    private RenderHelper GetRenderHelper()
    {
        RenderHelper renderHelper = GetComponent<RenderHelper>();
        if (renderHelper != null)
        {
            return renderHelper;
        }
        renderHelper = gameObject.AddComponent<RenderHelper>();
        return renderHelper;
    }
}
