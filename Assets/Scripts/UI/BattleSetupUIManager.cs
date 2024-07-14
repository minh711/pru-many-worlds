using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static BattleConstants;
using static InventoryConstants;
using static InventoryConstants.InventoryCharacterStatus;


public class BattleSetupUIManager : MonoBehaviour
{
    #region Biến để xử lý UI

    [SerializeField]
    private GameObject MainCanvas;

    private bool IsToggled = false;
    private GameObject UpgradeCharacterCanvas;
    private Transform BattleSetupUIPanel;
    private GameObject BattleSetupUIBackground;
    private Transform UpgradeCharacterPanel;
    private List<Transform> CharacterLocationPanelItemList;
    private List<Transform> CharacterInventoryPanelItemList;
    private GameObject DisplayingCharacter;

    #endregion

    #region Biến để xử lý Buttons

    private Button BtnClose;
    private Button BtnOpenUpgrade;
    private Button BtnCloseUpgrade;
    private Button BtnDeploy;
    private Button BtnUnDeploy;
    private bool IsDeploying = false;
    private bool IsReArrange = false;

    #endregion

    #region Assets đã load

    private List<Character> LoadedInventoryCharacterList;
    private List<Character> LoadedBattleCharacterList;

    #endregion

    #region Xử lý Character Inventory

    /// <summary>
    /// Chacracter đang chọn (khi bấm vào một Character trên UI)
    /// </summary>
    private Character SelectingCharacter;

    /// <summary>
    /// Phân trang trong inventory
    /// </summary>
    private int InventoryCharacterOffset;
    private int InventoryCharacterLimit;

    #endregion

    void Awake()
    {
        LoadedInventoryCharacterList = new List<Character>();
        LoadedBattleCharacterList = new List<Character>();
        CharacterLocationPanelItemList = new List<Transform>();
        CharacterInventoryPanelItemList = new List<Transform>();
    }

    async void Start()
    {
        ExtractMainPanel();

        MainCanvas.SetActive(false);
        UpgradeCharacterCanvas.SetActive(false);
        BattleSetupUIBackground.SetActive(false);

        #region Display Character Inventory List

        int totalInventoryCharacters = GameData.Instance.GetTotalInventoryCharacters();

        InventoryCharacterOffset = 0;

        // limit bình thường là 16, nhưng trong trường hợp không đủ 16 item thì sẽ lấy số nhỏ hơn
        InventoryCharacterLimit =
        (
            totalInventoryCharacters <= INVENTORY_MAX_ITEM_PER_PAGE
            ? totalInventoryCharacters
            : (InventoryCharacterOffset + INVENTORY_MAX_ITEM_PER_PAGE)
        );

        await DisplayInventoryCharacterList(InventoryCharacterOffset, InventoryCharacterLimit);

        await DisplayLocationCharacterList();

        #endregion
    }

    private void ExtractMainPanel()
    {
        BattleSetupUIPanel = MainCanvas.transform.Find("BattleSetupUIPanel");
        Transform CharacterInformationPanel = BattleSetupUIPanel.Find("CharacterInformationPanel");
        
        BattleSetupUIBackground = BattleSetupUIPanel.Find("BackgroundPanel").gameObject;
        UpgradeCharacterCanvas = MainCanvas.transform.Find("UpgradeCharacterUI").Find("UpgradeCharacterCanvas").gameObject;
        UpgradeCharacterPanel = UpgradeCharacterCanvas.transform.Find("UpgradeCharacterPanel");

        BtnClose = BattleSetupUIPanel.Find("BtnClose").GetComponent<Button>();
        BtnClose.onClick.AddListener(() => ToggleInventory());

        BtnOpenUpgrade = CharacterInformationPanel.Find("BtnUpgrade").GetComponent<Button>();
        BtnOpenUpgrade.onClick.AddListener(() => OnBtnOpenUpgradeClick());
        BtnOpenUpgrade.interactable = false;

        BtnCloseUpgrade = UpgradeCharacterPanel.Find("BtnClose").GetComponent<Button>();
        BtnCloseUpgrade.onClick.AddListener(() => OnBtnCloseUpgradeClick());

        BtnDeploy = CharacterInformationPanel.Find("BtnDeploy").GetComponent<Button>();
        BtnDeploy.onClick.AddListener(() => OnBtnDeployClick());
        BtnDeploy.interactable = false;

        BtnUnDeploy = CharacterInformationPanel.Find("BtnUnDeploy").GetComponent<Button>();
        BtnUnDeploy.onClick.AddListener(() => OnBtnUnDeployClick());
        BtnUnDeploy.interactable = false;

        HandleButtons();
    }

    #region Methods xử lý Buttons

    private void HandleButtons()
    {
        Transform CharacterLocationPanel = BattleSetupUIPanel.Find("CharacterLocationPanel");
        Transform CharacterInventoryPanel = BattleSetupUIPanel.Find("CharacterInventoryPanel");

        // The index 0 is the background, so from 1 to 9
        for (int i = 1; i <= 9; i++)
        {
            Transform item = CharacterLocationPanel.GetChild(i);
            Button button = item.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(() => OnLocationButtonClick(button));
            CharacterLocationPanelItemList.Add(item);
        }

        // The index 0 is the background, so from 1 to 16
        for (int i = 1; i <= 16; i++)
        {
            Transform item = CharacterInventoryPanel.GetChild(i);
            Button button = item.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(() => OnInventoryButtonClick(button));
            CharacterInventoryPanelItemList.Add(item);
        }
    }

    private void OnLocationButtonClick(Button clickedButton)
    {
        BtnOpenUpgrade.interactable = true;

        // index 0 is the background so the sibling indexes are from 1 to 9, but the index for code is from 0 to 8
        int locationIndex = clickedButton.transform.parent.GetSiblingIndex() - 1;
        Character currentCharacter = LoadedBattleCharacterList.FirstOrDefault(character => character.SpawnLocation == locationIndex);

        if (IsReArrange && currentCharacter != null)
        {
            IsDeploying = false;
            IsReArrange = false;
        }

        if (IsDeploying)
        {
            if (IsReArrange)
            {
                RemoveBattleCharacter(SelectingCharacter.SpawnLocation);
                IsReArrange = false;
            }

            if (currentCharacter != null)
            {
                // remove existed Character on this Location
                RemoveBattleCharacter(currentCharacter.SpawnLocation);
            }

            SetBattleCharacter(locationIndex);
            // Update Item Spirte
            RenderHelper renderHelper = GetRenderHelper();
            Transform selectedPanel = clickedButton.transform.parent.Find("ItemDisplayPanel");
            renderHelper.DrawSprite(SelectingCharacter.ItemSprite, selectedPanel);

            IsDeploying = false;
            SwitchDeployButtons(false);

            UpdateLocationButtons();

            return;
        }

        if (currentCharacter != null)
        {
            SelectingCharacter = currentCharacter;
            IsDeploying = false;
            SwitchDeployButtons(false);
        }

        DisplayCharacterInformation();

        IsDeploying = true;
        IsReArrange = true;
        BtnDeploy.interactable = false;
        UpdateLocationButtons();
        EnableLocationButtons();
    }

    private void OnInventoryButtonClick(Button clickedButton)
    {
        BtnOpenUpgrade.interactable = true;

        UpdateLocationButtons();

        // index 0 is the background so the sibling indexes are from 1 to 16, but the index for code is from 0 to 15
        int index = clickedButton.transform.parent.GetSiblingIndex() - 1;

        // If click on empty button, return.
        if (index >= LoadedInventoryCharacterList.Count)
        {
            return;
        }

        IsDeploying = false;

        SelectingCharacter = LoadedInventoryCharacterList
            .FirstOrDefault(character => (character.InventoryIndex % 16) == index);
        DisplayCharacterInformation();

        Character deployedCharacter = LoadedBattleCharacterList.Find(character => character.InventoryIndex == SelectingCharacter.InventoryIndex);

        if (deployedCharacter != null)
        {
            SelectingCharacter.SpawnLocation = deployedCharacter.SpawnLocation;
            SwitchDeployButtons(false);
        }
        else
        {
            SwitchDeployButtons(true);
        }
    }

    /// <summary>
    /// Lúc chọn "Trang bị" thì bật tất cả buttons để chọn.
    /// </summary>
    private void EnableLocationButtons()
    {
        for (int i = 0; i < 9; i++)
        {
            Button button = CharacterLocationPanelItemList[i]
                .GetChild(1).GetComponent<Button>();
            
            button.interactable = true;
        }
    }

    /// <summary>
    /// Lúc bình thường thì disable những buttons bị trống.
    /// </summary>
    private void UpdateLocationButtons()
    {
        for (int i = 0; i < 9; i++)
        {
            Button button = CharacterLocationPanelItemList[i]
                .GetChild(1).GetComponent<Button>();
            if (!LoadedBattleCharacterList.Any(character => character.SpawnLocation == i))
            {
                button.interactable = false;
            } else
            {
                button.interactable = true;
            }
        }
    }

    private void OnBtnDeployClick()
    {
        IsDeploying = true;
        BtnDeploy.interactable = false;
        EnableLocationButtons();
    }

    private void OnBtnUnDeployClick()
    {
        RemoveBattleCharacter(SelectingCharacter.SpawnLocation);
        IsDeploying = false;
        SwitchDeployButtons(true);
    }

    private void SwitchDeployButtons(bool isDeployable)
    {
        BtnDeploy.interactable = isDeployable;
        BtnUnDeploy.interactable = !isDeployable;
    }

    #region Methods xử lý hiển thị trạng thái item trong Inventory

    private void SetBattleCharacter(int locationIndex)
    {
        //Remove old one if exist
        RemoveBattleCharacter(locationIndex);

        Character newCharacter = Instantiate(SelectingCharacter);
        newCharacter.SpawnLocation = locationIndex;
        newCharacter.InventoryIndex = SelectingCharacter.InventoryIndex;
        LoadedBattleCharacterList.Add(newCharacter);

        // Update newly selected Character to Battle data
        LoadedInventoryCharacterList[SelectingCharacter.InventoryIndex % 16].SpawnLocation = locationIndex;
        CharacterData newBattleCharacterData = new CharacterData()
        {
            SpawnLocation = locationIndex,
            CharacterCode = SelectingCharacter.CharacterCode,
            CharacterExp = SelectingCharacter.Exp,
            InventoryIndex = SelectingCharacter.InventoryIndex
        };
        GameData.Instance.PlayerData.BattleCharacterDataList.Add(newBattleCharacterData);

        UpdateInventoryCharacterStatus(SelectingCharacter.InventoryIndex, SELECTED);

        UpdateLocationButtons();

        IsDeploying = false;
    }

    private void RemoveBattleCharacter(int locationIndex)
    {
        List<CharacterData> battleCharacterData = GameData.Instance.PlayerData.BattleCharacterDataList;

        Character characterToRemove = LoadedBattleCharacterList.Find(character => character.SpawnLocation == locationIndex);
        if (characterToRemove != null)
        {
            LoadedBattleCharacterList.Remove(characterToRemove);
        }

        // Kiểm tra xem Location đó đã có Character Data nào chưa
        bool isExist = battleCharacterData
            .Any(battleCharacterData => battleCharacterData.SpawnLocation == locationIndex);

        if (isExist)
        {
            CharacterData selectedCharacterData = battleCharacterData
                .FirstOrDefault(battleCharacterData => battleCharacterData.SpawnLocation == locationIndex);

            // Remove Character cũ
            battleCharacterData.Remove(selectedCharacterData);

            // Chuyển trạng thái của Character bị thay đổi dưới Inventory
            int selectedCharacterIndex = selectedCharacterData.InventoryIndex;
            bool isHaveSelectedCharacter = LoadedInventoryCharacterList.Any(character => character.InventoryIndex == selectedCharacterIndex);
            if (isHaveSelectedCharacter)
            {
                UpdateInventoryCharacterStatus(selectedCharacterIndex, NO_SELECT);
            }

            // Xóa Sprite
            Transform panelSlot = CharacterLocationPanelItemList[locationIndex];
            Transform panelItemDisplay = panelSlot.Find("ItemDisplayPanel");

            GetRenderHelper().DrawSprite(null, panelItemDisplay);
        }

        UpdateLocationButtons();
    }

    private void UpdateInventoryCharacterStatus(int characterInventoryIndex, InventoryCharacterStatus status)
    {
        string displayText = "";

        switch (status)
        {
            case SELECTED:
                displayText = "Đã chọn";
                break;
        }

        CharacterInventoryPanelItemList[characterInventoryIndex % 16]
            .GetChild(1).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = displayText;
    }

    #endregion

    private void OnBtnOpenUpgradeClick()
    {
        UpgradeCharacterUIManager upgradeCharacterUIManager = UpgradeCharacterCanvas.transform.parent.transform.GetChild(1).GetComponent<UpgradeCharacterUIManager>();
        upgradeCharacterUIManager.SelectingCharacter = SelectingCharacter;
        upgradeCharacterUIManager.Refresh();

        UpgradeCharacterCanvas.SetActive(true);
        BattleSetupUIBackground.SetActive(true);
    }

    private async void OnBtnCloseUpgradeClick()
    {
        UpgradeCharacterCanvas.SetActive(false);
        BattleSetupUIBackground.SetActive(false);

        await DisplayInventoryCharacterList(InventoryCharacterOffset, InventoryCharacterLimit);
        await DisplayLocationCharacterList();
    }

#endregion

    #region Methods xử lý Player Input

    public void OnToggleInventory()
    {
        ToggleInventory();
    }

    /// <summary>
    /// Close the Inventory Menu when Toggle Menu key are pressed.
    /// </summary>
    public void OnToggleMenu()
    {
        IsToggled = false;
        MainCanvas.SetActive(false);
        UpgradeCharacterCanvas.SetActive(false);
    }

    public void ToggleInventory()
    {
        IsToggled = !IsToggled;
        PlayerPrefs.SetInt(PlayerPrefsKeys.IS_UI_OPEN, (IsToggled ? 1 : 0));
        MainCanvas.SetActive(IsToggled);
        UpgradeCharacterCanvas.SetActive(false);
        BattleSetupUIBackground.SetActive(false);
    }

    #endregion

    #region Methods xử lý Display

    private async Task DisplayLocationCharacterList()
    {
        LoadingHandler loadingHandler = LoadingHandler.Instance;

        LoadedBattleCharacterList.Clear();

        await loadingHandler.LoadCharacterLocationList(LoadedBattleCharacterList);
        await loadingHandler.LoadCharacterItemSprite(LoadedBattleCharacterList);

        RenderHelper renderHelper = GetRenderHelper();

        // clear the current display
        foreach (Transform panelSlot in CharacterLocationPanelItemList)
        {
            Transform panelItemDisplay = panelSlot.GetChild(0);
            renderHelper.DrawSprite(null, panelItemDisplay);
        }

        foreach (Character character in LoadedBattleCharacterList)
        {
            int slot = character.SpawnLocation;
            Transform panelSlot = CharacterLocationPanelItemList[slot];
            Transform panelItemDisplay = panelSlot.GetChild(0);

            Sprite characterItemSprite = character.ItemSprite;
            renderHelper.DrawSprite(characterItemSprite, panelItemDisplay);
        }

        UpdateLocationButtons();
    }

    private async Task DisplayInventoryCharacterList(int offset, int limit)
    {
        LoadingHandler loadingHandler = LoadingHandler.Instance;

        LoadedInventoryCharacterList.Clear();

        await loadingHandler.LoadCharacterList(LoadedInventoryCharacterList, offset, limit);
        await loadingHandler.LoadCharacterItemSprite(LoadedInventoryCharacterList);

        RenderHelper renderHelper = GetRenderHelper();

        // clear current display
        foreach (Transform panelSlot in CharacterInventoryPanelItemList)
        {
            Transform panelItemDisplay = panelSlot.GetChild(0);
            renderHelper.DrawSprite(null, panelItemDisplay);
        }

        int slot = 0;
        foreach (Character character in LoadedInventoryCharacterList)
        {
            Transform panelSlot = CharacterInventoryPanelItemList[slot];
            Transform panelItemDisplay = panelSlot.GetChild(0);
            Transform button = panelSlot.GetChild(1);
            button.GetComponent<Button>().interactable = true;

            Sprite characterItemSprite = character.ItemSprite;
            renderHelper.DrawSprite(characterItemSprite, panelItemDisplay);

            if (GameData.Instance.PlayerData.BattleCharacterDataList
                .Any(battleCharacter => battleCharacter.InventoryIndex == character.InventoryIndex))
            {
                button.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Đã chọn";
            }

            slot++;
        }

        for (int i = 0; i < 16; i++)
        {
            Button button = CharacterInventoryPanelItemList[i]
               .GetChild(1).GetComponent<Button>();
            if (!LoadedInventoryCharacterList.Any(character => (character.InventoryIndex % 16) == i))
            {
                button.interactable = false;
            } else
            {
                button.interactable = true;
            }
        }
    }

    private async void DisplayCharacterInformation()
    {
        if (DisplayingCharacter != null)
        {
            Destroy(DisplayingCharacter);
        }

        Transform selectingCharacterDisplayPanel = BattleSetupUIPanel.Find("CharacterInformationPanel").Find("CharacterDisplayPanel");
        DisplayingCharacter = await LoadingHandler.Instance.LoadPrefab(SelectingCharacter.PrefabPath);
        DisplayingCharacter = Instantiate(DisplayingCharacter, selectingCharacterDisplayPanel.position, selectingCharacterDisplayPanel.rotation);
        DisplayingCharacter.transform.SetParent(selectingCharacterDisplayPanel, false);
        DisplayingCharacter.transform.localPosition = Vector3.zero;

        RectTransform rectTransform = selectingCharacterDisplayPanel.GetComponent<RectTransform>();
        float panelWidth = rectTransform.rect.width;
        float desiredWidth = panelWidth * 0.3f;
        float scaleFactor = desiredWidth / DisplayingCharacter.transform.localScale.x;
        DisplayingCharacter.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        SpriteRenderer spriteRenderer = DisplayingCharacter.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "UI";
        spriteRenderer.sortingOrder = 1;

        Transform CharacterInformationPanel = BattleSetupUIPanel.Find("CharacterInformationPanel");
        TextMeshProUGUI TxtLevel = CharacterInformationPanel.Find("TxtLevel").GetComponent<TextMeshProUGUI>();
        SelectingCharacter.CharacterStats.Initialize(SelectingCharacter.Exp);
        TxtLevel.text = $"LV {SelectingCharacter.CharacterStats.Level}";

        TextMeshProUGUI TxtName = CharacterInformationPanel.Find("TxtName").GetComponent<TextMeshProUGUI>();
        TxtName.text = $"{SelectingCharacter.Name}";

        TextMeshProUGUI TxtExp = CharacterInformationPanel.Find("TxtExp").GetComponent<TextMeshProUGUI>();
        TxtExp.text = $"{SelectingCharacter.Exp}/{SelectingCharacter.CharacterStats.CalulateExpForNextLevel()}";

        TextMeshProUGUI TxtHp = CharacterInformationPanel.Find("TxtHp").GetComponent<TextMeshProUGUI>();
        TxtHp.text = $"HP: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.HP].Current}";

        TextMeshProUGUI TxtMp = CharacterInformationPanel.Find("TxtMp").GetComponent<TextMeshProUGUI>();
        TxtMp.text = $"MP: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.MP].Current}";

        TextMeshProUGUI TxtAtk = CharacterInformationPanel.Find("TxtAtk").GetComponent<TextMeshProUGUI>();
        TxtAtk.text = $"ATK: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.ATK].Current}";

        TextMeshProUGUI TxtDef = CharacterInformationPanel.Find("TxtDef").GetComponent<TextMeshProUGUI>();
        TxtDef.text = $"DEF: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.DEF].Current}";

        TextMeshProUGUI TxtMatk = CharacterInformationPanel.Find("TxtMatk").GetComponent<TextMeshProUGUI>();
        TxtMatk.text = $"MATK: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.MATK].Current}";

        TextMeshProUGUI TxtMdef = CharacterInformationPanel.Find("TxtMdef").GetComponent<TextMeshProUGUI>();
        TxtMdef.text = $"MDEF: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.MDEF].Current}";

        TextMeshProUGUI TxtCrit = CharacterInformationPanel.Find("TxtCrit").GetComponent<TextMeshProUGUI>();
        TxtCrit.text = $"Crit: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.CRIT].Current}";

        TextMeshProUGUI TxtSpd = CharacterInformationPanel.Find("TxtSpd").GetComponent<TextMeshProUGUI>();
        TxtSpd.text = $"Spd: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.SPD].Current}";

        TextMeshProUGUI TxtEva = CharacterInformationPanel.Find("TxtEva").GetComponent<TextMeshProUGUI>();
        TxtEva.text = $"Eva: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.EVA].Current}";

        TextMeshProUGUI TxtAcc = CharacterInformationPanel.Find("TxtAcc").GetComponent<TextMeshProUGUI>();
        TxtAcc.text = $"DEF: {SelectingCharacter.CharacterStats.StatList[(int)StatConstants.CharacterStatType.ACC].Current}";
    }

    #endregion

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
