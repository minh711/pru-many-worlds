using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Managers;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; set; }

    public PlayerData PlayerData { get; set; }
    public BattleData BattleData { get; set; }

    public int CurrentSceneIndex { get; set; }
    public bool IsLoaded { get; private set; }
    public bool IsSetPosition = false;
    public float PlayerX { get; set; }
    public float PlayerY { get; set; }

    private void Awake()
    {
        // Ensure only one instance of GameData exists
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject); // Make this GameObject persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        #region Set PlayerPrefs
        PlayerPrefs.SetInt(PlayerPrefsKeys.IS_UI_OPEN, 0);
        #endregion
    }

    public void Initialize()
    {
        PlayerData = new PlayerData();

        //Mockup();
        LoadGameData();

        IsLoaded = true;
    }

    /// <summary>
    /// Mockup, will be removed soon.
    /// </summary>
    public void NewGameData()
    {
        PlayerData = new PlayerData();
        CurrentSceneIndex = 0;
        IsSetPosition = false;
        PlayerData.CharacterDataList.Add(new CharacterData { InventoryIndex = 0, CharacterCode = "Avocado", CharacterExp = 0});
        PlayerData.BattleCharacterDataList.Add(new CharacterData { InventoryIndex = 0, CharacterCode = "Avocado", CharacterExp = 0, SpawnLocation = 0 });
    }

    public void LoadGameData()
    {
        GameSave gameSave = new GameSave();
        PlayerData = gameSave.GetPlayerDataFromLocal();
        CurrentSceneIndex = PlayerData.CurrentSceneIndex;
        IsSetPosition = false;
    }

    public void SaveAndQuit()
    {
        GameSave gameSave = new GameSave();
        PlayerData.CurrentSceneIndex = CurrentSceneIndex;
        StartCoroutine(gameSave.SaveData(this));
    }

    public List<CharacterData> GetBattleCharacterDataList()
    {
        return PlayerData.BattleCharacterDataList;
    }

    public int GetTotalInventoryCharacters()
    {
        return PlayerData.CharacterDataList.Count;
    }

    public int GetTotalItems()
    {
        return PlayerData.ItemDataList.Count;
    }

    public bool IsInventoryCharacterSelected(Character nextSelectingCharacter)
    {

        CharacterData selectedCharacterData = PlayerData.BattleCharacterDataList
            .FirstOrDefault
            (
                battleCharacterData 
                    => battleCharacterData.InventoryIndex == nextSelectingCharacter.InventoryIndex
            );

        if (selectedCharacterData != null)
        {
            return true;
        }

        return false;
    }

    public void SetBattleData(BattleData battleData)
    {
        this.BattleData = battleData;
    }

    public List<CharacterData> GetAllyCharacterList()
    {
        return this.PlayerData.BattleCharacterDataList;
    }

    public BattleData GetBattleData()
    {
        return this.BattleData;
    }
}
