using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SceneConstants;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField]
    private Transform MainPanel;

    private Button BtnNewGame;
    private Button BtnContinue;
    private Button BtnSettings;

    // Start is called before the first frame update
    void Start()
    {
        ExtractMainPanel();
        HandleButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ExtractMainPanel()
    {
        BtnNewGame = MainPanel.GetChild(0).GetComponent<Button>();
        BtnContinue = MainPanel.GetChild(1).GetComponent<Button>();
    }

    private void HandleButtons()
    {
        BtnNewGame.onClick.AddListener(() => OnBtnNewGameClick());
        BtnContinue.onClick.AddListener(() => OnBtnContinueClick());
    }

    private void OnBtnNewGameClick()
    {
        GameData.Instance.NewGameData();
        GameData.Instance.CurrentSceneIndex = (int)SceneIndexs.ISLAND;
        SceneManager.LoadScene((int)SceneIndexs.ISLAND, LoadSceneMode.Single);
    }

    private void OnBtnContinueClick()
    {
        try
        {
            GameData.Instance.LoadGameData();
            if (GameData.Instance.PlayerData.CharacterDataList.Count == 0)
            {
                OnBtnNewGameClick();
            }

            if (GameData.Instance.CurrentSceneIndex == 0)
            {
                GameData.Instance.CurrentSceneIndex = (int)SceneIndexs.ISLAND;
            }

            SceneManager.LoadScene(GameData.Instance.CurrentSceneIndex, LoadSceneMode.Single);
        } catch
        {
            OnBtnNewGameClick();
        }
    }
}
