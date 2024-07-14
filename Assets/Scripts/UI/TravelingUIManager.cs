using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventoryConstants;
using static InventoryConstants.InventoryCharacterStatus;


public class TravelingUIManager : MonoBehaviour
{
    #region Biến để xử lý UI

    [SerializeField]
    private GameObject MainCanvas;

    private GameObject MenuCanvas;
    private bool IsInventoryOpened = false;
    private bool IsMenuOpened = false;

    #endregion

    #region Buttons

    private Button BtnOpenMenu;
    private Button BtnCloseMenu;
    private Button BtnContinue;
    private Button BtnCloseBatteSetup;

    #endregion

    void Awake()
    {

    }

    void Start()
    {
        ExtractMainPanel();
        MenuCanvas.SetActive(false);
    }

    private void ExtractMainPanel()
    {
        MenuCanvas = MainCanvas.transform.Find("MenuUI").Find("MainCanvas").gameObject;

        BtnOpenMenu = MainCanvas.transform.Find("BtnMenu").GetComponent<Button>();
        BtnCloseMenu = MenuCanvas.transform.Find("MenuPanel").Find("BtnClose").GetComponent<Button>();
        BtnContinue = MainCanvas.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>();
        BtnCloseBatteSetup = MainCanvas.transform.Find("BattleSetupUI").GetChild(0).GetChild(0).GetChild(3).GetComponent<Button>();

        BtnOpenMenu.onClick.AddListener(() => OnBtnOpenMenuClick());
        BtnCloseMenu.onClick.AddListener(() => OnBtnCloseMenuClick());
        BtnContinue.onClick.AddListener(() => OnBtnContinueClick());
        BtnCloseBatteSetup.onClick.AddListener(() => OnBtnCloseBatteSetupClick());
    }

    #region Methods xử lý nút bấm

    private void OnBtnOpenMenuClick()
    {
        IsMenuOpened = true;
        MenuCanvas.SetActive(IsMenuOpened);
        ToggleButtons(false);
    }

    private void OnBtnCloseMenuClick()
    {
        IsMenuOpened = false;
        MenuCanvas.SetActive(IsMenuOpened);
        ToggleButtons(!IsMenuOpened);
    }

    private void OnBtnCloseBatteSetupClick()
    {
        IsInventoryOpened = false;
        IsMenuOpened = false;
        ToggleButtons(!IsInventoryOpened);
    }

    private void OnBtnContinueClick()
    {
        OnBtnCloseMenuClick();
    }

    public void OnToggleInventory()
    {
        IsInventoryOpened = !IsInventoryOpened;
        IsMenuOpened = false;
        MenuCanvas.SetActive(IsMenuOpened);
        ToggleButtons(!IsInventoryOpened);
    }

    public void OnToggleMenu()
    {
        IsMenuOpened = !IsMenuOpened;
        IsInventoryOpened = false;
        MenuCanvas.SetActive(IsMenuOpened);
        ToggleButtons(!IsMenuOpened);
    }

    /// <summary>
    /// Set the Buttons interactable or not.
    /// </summary>
    /// <param name="signal">True means ON, Fasle means OFF</param>
    private void ToggleButtons(bool signal)
    {
        BtnOpenMenu.interactable = signal;
    }

    #endregion
}
