using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static CharacterConstants;
using static ItemConstants;

public sealed class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance { get; set; }

    private void Awake()
    {
        // Ensure only one instance of LoadingHandler exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this GameObject persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task LoadPrefabs(HashSet<string> assetKeys, Dictionary<string, GameObject> loadedAssetsList)
    {
        AsyncOperationHandle<IList<GameObject>> loadHandle;

        loadHandle = Addressables.LoadAssetsAsync<GameObject>(
            assetKeys,
            addressable =>
            {
                loadedAssetsList.TryAdd(addressable.name, addressable);
            }, Addressables.MergeMode.Union,
            false);

        await loadHandle.Task;
    }

    public async Task<GameObject> LoadPrefab(string assetKey)
    {
        AsyncOperationHandle<GameObject> loadHandle;

        loadHandle = Addressables.LoadAssetAsync<GameObject>(assetKey);

        await loadHandle.Task;

        return loadHandle.Result;
    }

    public async Task LoadObjects<T>(HashSet<string> objectsToLoadList, Dictionary<string, T> loadedObjectsList) where T : UnityEngine.Object
    {
        AsyncOperationHandle<IList<T>> loadHandle;

        loadHandle = Addressables.LoadAssetsAsync<T>(
            objectsToLoadList,
            obj =>
            {
                loadedObjectsList.TryAdd(obj.name, obj);
            }, Addressables.MergeMode.Union,
            false);

        await loadHandle.Task;
    }

    #region Load Characters

    public async Task LoadBattleCharacters(BattleLoadingInput input)
    {
        #region Load Character ScriptableObjects

        HashSet<string> CharacterKeysList = new HashSet<string>();
        Dictionary<string, Character> LoadedCharactersList = new Dictionary<string, Character>();
        foreach (CharacterData characterData in input.AllyCharacterDataList)
        {
            CharacterKeysList.Add(CHARACTER_ASSET_DICTIONARY[characterData.CharacterCode]);
        }
        foreach (CharacterData characterData in input.EnemyCharacterDataList)
        {
            CharacterKeysList.Add(CHARACTER_ASSET_DICTIONARY[characterData.CharacterCode]);
        }
        await LoadObjects(CharacterKeysList, LoadedCharactersList);

        #endregion

        #region Load Character Prefabs

        HashSet<string> CharacterPrefabKeysList = new HashSet<string>();
        Dictionary<string, GameObject> LoadedCharacterPrefabList = new Dictionary<string, GameObject>();
        foreach (var kvp in LoadedCharactersList)
        {
            CharacterPrefabKeysList.Add(kvp.Value.PrefabPath);
        }
        await LoadPrefabs(CharacterPrefabKeysList, LoadedCharacterPrefabList);

        #endregion

        #region Asign loaded Character to using lists

        foreach (CharacterData characterData in input.AllyCharacterDataList)
        {
            Character character = Instantiate(LoadedCharactersList[characterData.CharacterCode]);
            character.Exp = characterData.CharacterExp;
            character.SpawnLocation = characterData.SpawnLocation;
            character.Initialize();
            input.AllyCharacterList.Add(character);
        }

        foreach (CharacterData characterData in input.EnemyCharacterDataList)
        {
            Character character = Instantiate(LoadedCharactersList[characterData.CharacterCode]);
            character.Exp = characterData.CharacterExp;
            character.SpawnLocation = characterData.SpawnLocation;
            character.Initialize();
            input.EnemyCharacterList.Add(character);
        }

        #endregion

        #region Asign prefab to loaded Characters in using lists

        foreach (var character in input.AllyCharacterList)
        {
            character.CharacterPrefab = LoadedCharacterPrefabList[character.CharacterCode];
        }

        foreach (var character in input.EnemyCharacterList)
        {
            character.CharacterPrefab = LoadedCharacterPrefabList[character.CharacterCode];
        }

        #endregion
    }

    public async Task LoadCharacterItemSprite(List<Character> characterList)
    {
        AsyncOperationHandle<IList<Sprite>> loadHandle;
        Dictionary<string, Sprite> returnCharacterItemSpriteList = new Dictionary<string, Sprite>();

        HashSet<string> characterItemSpriteToLoadList = new HashSet<string>();

        foreach (Character character in characterList)
        {
            characterItemSpriteToLoadList.Add(character.ItemSpritePath);
        }

        loadHandle = Addressables.LoadAssetsAsync<Sprite>(
            characterItemSpriteToLoadList,
            characterItemSprite =>
            {
                returnCharacterItemSpriteList.TryAdd(characterItemSprite.name, characterItemSprite);
            }, Addressables.MergeMode.Union,
            false); ;

        await loadHandle.Task;

        foreach (Character character in characterList)
        {
            character.ItemSprite = returnCharacterItemSpriteList[$"Item{character.CharacterCode}"];
        }
    }

    public async Task LoadCharacterLocationList(List<Character> returnCharacterList)
    {
        List<CharacterData> characterLocationDataList = GameData.Instance.PlayerData.BattleCharacterDataList;
        Dictionary<string, Character> loadedCharacterList = new Dictionary<string, Character>();

        AsyncOperationHandle<IList<Character>> loadHandle;
        HashSet<string> characterToLoadList = new HashSet<string>();

        // Get the paths of the assets to load
        foreach (CharacterData data in characterLocationDataList)
        {
            characterToLoadList.Add(CHARACTER_ASSET_DICTIONARY[data.CharacterCode]);
        }

        // Load data using Addressables
        loadHandle = Addressables.LoadAssetsAsync<Character>(
            characterToLoadList,
            character =>
            {
                loadedCharacterList.TryAdd(character.CharacterCode, character);
            }, Addressables.MergeMode.Union,
        false); ;

        await loadHandle.Task;

        // Asign a clone of the loaded asset list to the return CharacterList
        foreach (CharacterData data in characterLocationDataList)
        {
            Character character = Instantiate(loadedCharacterList[data.CharacterCode]);
            character.InventoryIndex = data.InventoryIndex;
            character.Exp = data.CharacterExp;
            character.SpawnLocation = data.SpawnLocation;
            returnCharacterList.Add(character);
        }
    }

    public async Task LoadCharacterList(List<Character> returnCharacterList, int offset, int limit)
    {
        Dictionary<string, Character> loadedCharacterList = new Dictionary<string, Character>();
        List<CharacterData> characterDataList = GameData.Instance.PlayerData.CharacterDataList;
        int characterDataCount = characterDataList.Count;

        if (offset >= characterDataCount) offset = characterDataCount - 1;
        if (limit >= characterDataCount) limit = characterDataCount;
        List<CharacterData> characterDataInRangeList = characterDataList
            .Skip(offset)
            .Take(limit)
            .ToList();

        AsyncOperationHandle<IList<Character>> loadHandle;
        HashSet<string> characterToLoadList = new HashSet<string>();

        // Get the paths of the assets to load
        foreach (CharacterData data in characterDataInRangeList)
        {
            characterToLoadList.Add(CHARACTER_ASSET_DICTIONARY[data.CharacterCode]);
        }

        // Load data using Addressables
        loadHandle = Addressables.LoadAssetsAsync<Character>(
            characterToLoadList,
            character =>
            {
                loadedCharacterList.TryAdd(character.CharacterCode, character);
            }, Addressables.MergeMode.Union,
            false); ;

        await loadHandle.Task;

        // Asign a clone of the loaded asset list to the return CharacterList
        foreach (CharacterData data in characterDataInRangeList)
        {
            Character character = Instantiate(loadedCharacterList[data.CharacterCode]);
            character.InventoryIndex = data.InventoryIndex;
            character.Exp = data.CharacterExp;
            character.SpawnLocation = data.SpawnLocation;
            returnCharacterList.Add(character);
        }
    }

    #endregion

    #region Load Items

    public async Task LoadItemItemSprite(List<ItemExp> itemList)
    {
        if (itemList.Count == 0)
        {
            return;
        }

        AsyncOperationHandle<IList<Sprite>> loadHandle;
        Dictionary<string, Sprite> returnItemItemSpriteList = new Dictionary<string, Sprite>();

        HashSet<string> itemItemSpriteToLoadList = new HashSet<string>();

        foreach (Item item in itemList)
        {
            itemItemSpriteToLoadList.Add(item.ItemSpritePath);
        }

        loadHandle = Addressables.LoadAssetsAsync<Sprite>(
            itemItemSpriteToLoadList,
            characterItemSprite =>
            {
                returnItemItemSpriteList.TryAdd(characterItemSprite.name, characterItemSprite);
            }, Addressables.MergeMode.Union,
            false); ;

        await loadHandle.Task;

        foreach (Item item in itemList)
        {
            item.ItemSprite = returnItemItemSpriteList[$"Item{item.ItemCode}"];
        }
    }

    public async Task LoadItemList(List<ItemExp> returnItemList, int offset, int limit)
    {
        Dictionary<string, ItemExp> loadedItemList = new Dictionary<string, ItemExp>();
        List<ItemData> itemDataList = GameData.Instance.PlayerData.ItemDataList;
        int itemDataCount = itemDataList.Count;

        if (itemDataList.Count == 0)
        {
            return;
        }

        if (offset >= itemDataCount) offset = itemDataCount - 1;
        if (limit >= itemDataCount) limit = itemDataCount;
        List<ItemData> itemDataInRangeList = itemDataList
            .Skip(offset)
            .Take(limit)
            .ToList();

        AsyncOperationHandle<IList<ItemExp>> loadHandle;
        HashSet<string> itemToLoadList = new HashSet<string>();

        // Get the paths of the assets to load
        foreach (ItemData data in itemDataInRangeList)
        {
            itemToLoadList.Add(ITEM_ASSET_DICTIONARY[data.ItemCode]);
        }

        // Load data using Addressables
        loadHandle = Addressables.LoadAssetsAsync<ItemExp>(
            itemToLoadList,
            item =>
            {
                loadedItemList.TryAdd(item.ItemCode, item);
            }, Addressables.MergeMode.Union,
            false); ;

        await loadHandle.Task;

        // Asign a clone of the loaded asset list to the return CharacterList
        foreach (ItemData data in itemDataInRangeList)
        {
            ItemExp item = Instantiate(loadedItemList[data.ItemCode]);
            item.InventoryIndex = data.InventoryIndex;
            item.Quantity = data.Quantity;
            returnItemList.Add(item);
        }
    }

    #endregion
}
