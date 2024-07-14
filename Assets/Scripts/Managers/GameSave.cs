using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameSave
    {
        //public static GameSave Instance { get; private set; }

        //[SerializeField]
        //private GameData gameData;

        public PlayerData PlayerData { get; private set; }

        private string PATH = string.Empty;

        //private void Awake()
        //{
        //    if (Instance == null)
        //    {
        //        Instance = this;
        //        Initialize();
        //        DontDestroyOnLoad(gameObject); // Make this GameObject persistent across scenes
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }

        //}

        //private void Initialize()
        //{
        //    PATH = GetDataPath();
        //    StartCoroutine(SaveData());
        //}

        public IEnumerator SaveData(GameData gameData)
        {
            PATH = GetDataPath();

            Debug.Log("Wait for game data loaded...");

            // Do nothing until GameData loaded
            yield return new WaitUntil(() => gameData.IsLoaded);

            // Save player data got from GameData object into json file under PATH
            File.WriteAllText(PATH, ToJsonString(gameData.PlayerData));
            Debug.Log("PlayerData stored at " + PATH);

            Debug.Log("Quitting application...");
            yield return new WaitForSeconds(2f); // Optional delay before quitting

            Application.Quit();
        }

        public string GetDataPath()
        {
            string persistenDataPath = Application.persistentDataPath;
            string dataFolder = Path.Combine(persistenDataPath, "UserData");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            string jsonFilePath = Path.Combine(dataFolder, "PlayerData.json");

            return jsonFilePath;
        }

        public string ToJsonString(PlayerData data)
        {
            return JsonHelper.ToJson(new List<PlayerData> { data }, false);
        }

        public PlayerData GetPlayerDataFromLocal()
        {
            string path = GetDataPath();

            if (File.Exists(path)) {
                string jsonContent = File.ReadAllText(path);
                return JsonHelper.FromJson<PlayerData>(jsonContent).First();
            }
            Debug.Log("The file PlayerData.json not found under path " + path);
            return null;
        }

        public void SavePlayerDataToLocalJson()
        {
            //StartCoroutine(SaveData());
        }
    }
}
