using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventoryConstants;
using static InventoryConstants.InventoryCharacterStatus;


public class MenuUIManager : MonoBehaviour
{
    #region Biến để xử lý UI

    [SerializeField]
    private GameObject MainCanvas;

    private Transform MenuPanel;
    private GameObject SettingsUICanvas;
    private GameObject SelectMapUICanvas;

    #endregion

    #region Buttons

    private Button BtnOpenSettings;
    private Button BtnOpenSelectMap;
    private Button BtnCloseSettings;
    private Button BtnCloseSelectMap;
    private Button BtnSave;

    #endregion

    void Start()
    {
        ExtractMainPanel();
        SettingsUICanvas.SetActive(false);
        SelectMapUICanvas.SetActive(false);
    }

    private void ExtractMainPanel()
    {
        MenuPanel = MainCanvas.transform.GetChild(0);
        SelectMapUICanvas = MainCanvas.transform.GetChild(1).GetChild(0).gameObject;
        SettingsUICanvas = MainCanvas.transform.GetChild(2).GetChild(0).gameObject;

        BtnOpenSelectMap = MenuPanel.GetChild(3).GetComponent<Button>();
        BtnOpenSettings = MenuPanel.GetChild(2).GetComponent<Button>();
        BtnCloseSelectMap = SelectMapUICanvas.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        BtnCloseSettings = SettingsUICanvas.transform.GetChild(0).GetChild(0).GetComponent<Button>();

        BtnOpenSelectMap.onClick.AddListener(() => OnBtnOpenSelectMapClick());
        BtnOpenSettings.onClick.AddListener(() => OnBtnOpenSettingsClick());
        BtnCloseSelectMap.onClick.AddListener(() => OnBtnCloseSelectMapClick());
        BtnCloseSettings.onClick.AddListener(() => OnBtnCloseSettingsClick());

        BtnSave = MenuPanel.Find("BtnSave").GetComponent<Button>();
        BtnSave.onClick.AddListener(() => OnBtnSaveClick());
    }

    private void OnBtnSaveClick()
    {
        GameData.Instance.SaveAndQuit();
    }

    private void OnBtnOpenSettingsClick()
    {
        SettingsUICanvas.SetActive(true);
    }

    private void OnBtnOpenSelectMapClick()
    {
        SelectMapUICanvas.SetActive(true);
    }

    private void OnBtnCloseSettingsClick()
    {
        SettingsUICanvas.SetActive(false);
    }

    private void OnBtnCloseSelectMapClick()
    {
        SelectMapUICanvas.SetActive(false);
    }
}
