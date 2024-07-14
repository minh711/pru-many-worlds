using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SceneConstants;

public class SelectMapUIManager : MonoBehaviour
{
    [SerializeField]
    private Transform Panel;

    private Button BtnDesert;
    private Button BtnIsland;
    private Button BtnRuins;
    private Button BtnSpace;

    // Start is called before the first frame update
    void Start()
    {
        ExtractPanel();
        HandleButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ExtractPanel()
    {
        BtnDesert = Panel.GetChild(1).GetComponent<Button>();
        BtnIsland = Panel.GetChild(2).GetComponent<Button>();
        BtnRuins = Panel.GetChild(3).GetComponent<Button>();
        BtnSpace = Panel.GetChild(4).GetComponent<Button>();
    }

    private void HandleButton()
    {
        BtnDesert.onClick.AddListener(() => OnBtnDesertClick());
        BtnIsland.onClick.AddListener(() => OnBtnDIslandClick());
        BtnRuins.onClick.AddListener(() => OnBtnRuinsClick());
        BtnSpace.onClick.AddListener(() => OnBtnSpaceClick());
    }

    private void OnBtnDesertClick()
    {
        GameData.Instance.IsSetPosition = false;
        GameData.Instance.CurrentSceneIndex = (int)SceneIndexs.DESERT;
        SceneManager.LoadScene((int)SceneIndexs.DESERT, LoadSceneMode.Single);
    }

    private void OnBtnDIslandClick()
    {
        GameData.Instance.IsSetPosition = false;
        GameData.Instance.CurrentSceneIndex = (int)SceneIndexs.ISLAND;
        SceneManager.LoadScene((int)SceneIndexs.ISLAND, LoadSceneMode.Single);
    }

    private void OnBtnRuinsClick()
    {
        GameData.Instance.IsSetPosition = false;
        GameData.Instance.CurrentSceneIndex = (int)SceneIndexs.RUINS;
        SceneManager.LoadScene((int)SceneIndexs.RUINS, LoadSceneMode.Single);
    }

    private void OnBtnSpaceClick()
    {
        GameData.Instance.IsSetPosition = false;
        GameData.Instance.CurrentSceneIndex = (int)SceneIndexs.SPACE;
        SceneManager.LoadScene((int)SceneIndexs.SPACE, LoadSceneMode.Single);
    }
}
