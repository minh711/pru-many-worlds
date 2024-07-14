using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BattleConstants;
using static StatConstants;

public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// Chứa tất cả Object cần xử lý trong Scene (ví dụ: SpawnPoint)
    /// </summary>
    public Transform GameContainer;

    /// <summary>
    /// Temporary for showing the attack effect.
    /// <br/><br/>
    /// Will be improved with more code and more flexibility.
    /// </summary>
    public GameObject AttackEffect;

    /// <summary>
    /// Character bên phe đồng minh
    /// </summary>
    private List<Character> AllyCharacterList;

    /// <summary>
    /// Character bên phe đối thủ
    /// </summary>
    private List<Character> EnemyCharacterList;

    #region For Audio

    public AudioSource AudioSource;
    public AudioClip AttackSound;

    #endregion

    #region Biến để xử lý UI

    [SerializeField]
    private GameObject BattleResultUI;

    /// <summary>
    /// Dòng Text hiện thông tin trận đấu.
    /// </summary>
    public TextMeshProUGUI Notation;

    private TextMeshProUGUI TxtBattleResult;
    private TextMeshProUGUI TxtRewards;
    private Button BtnOk;

    #endregion

    #region Battle Properties

    /// <summary>
    /// Giai đoạn trong trận để xử lý với hàm Update (dùng cho Turn, Animation, ...)
    /// </summary>
    public BattleState BattleState;

    private Vector3 FromPosition;
    private Vector3 ToPosition;
    private GameObject ActiveChar;

    private bool IsRewardGet = false;

    #endregion

    #region Skill & Animation Handler

    private bool IsMovingToward;
    private bool IsMovingCompleted = false;

    /// <summary>
    /// Skill sẽ dùng trong mỗi lần xử lý animation
    /// </summary>
    private ActiveSkill ToDoSkill;

    /// <summary>
    /// Dữ liệu của Battle truyền cho Skill để xử lý.
    /// </summary>
    private BattleDTO BattleInput;

    /// <summary>
    /// Dữ liệu của Battle mà các method (của Skill) trả về.
    /// </summary>
    private BattleDTO BattleOutput;

    #endregion

    void Awake()
    {
        Initialize();
    }

    async void Start()
    {

        SystemSettings();

        #region Handle Loading

        BattleState = BattleState.LOADING;

        BattleData battleData = GameData.Instance.GetBattleData();
        List<CharacterData> AllyCharacterDataList = GameData.Instance.GetAllyCharacterList();
        // temporary only use phase 1
        List<CharacterData> EnemyCharacterDataList = battleData.BattlePhases[0].EnemyCharacterDataList;

        LoadingHandler LoadingHandler = LoadingHandler.Instance;

        BattleLoadingInput loadingInput = new()
        {
            AllyCharacterList = AllyCharacterList,
            EnemyCharacterList = EnemyCharacterList,
            AllyCharacterDataList = AllyCharacterDataList,
            EnemyCharacterDataList = EnemyCharacterDataList
        };

        await LoadingHandler.LoadBattleCharacters(loadingInput);

        #endregion

        #region Spawning GameObjects

        Transform allySpawnPointList = GameContainer.GetChild((int)SpawnPoints.ALLY_SPAWN_POINTS);
        Transform enemySpawnPointList = GameContainer.GetChild((int)SpawnPoints.ENEMY_SPAWN_POINTS);

        SpawnCharacters(AllyCharacterList, allySpawnPointList, false);
        SpawnCharacters(EnemyCharacterList, enemySpawnPointList, true);

        #endregion

        StartBattle();
    }

    void Update()
    {
        switch (BattleState)
        {
            case BattleState.PAUSED:
                break;

            case BattleState.IN_TURN:
                InTurn();
                break;

            case BattleState.PLAYING:
                PerformAction();
                break;

            case BattleState.WAITING_FOR_NEXT_TURN:
                bool isNextTurn = CheckAlive();
                if (isNextTurn)
                {
                    NewTurn();
                }
                break;
        }
    }

    /// <summary>
    /// Khai báo các biến và gán các giá trị mặc định cho chúng.
    /// </summary>
    private void Initialize()
    {
        AllyCharacterList = new List<Character>();
        EnemyCharacterList = new List<Character>();
        AllyCharacterList = new List<Character>();
        EnemyCharacterList = new List<Character>();
        BattleInput = new BattleDTO();
        BattleOutput = new BattleDTO();

        // for UI
        BattleResultUI.SetActive(false);
        TxtBattleResult = BattleResultUI.transform.Find("TxtResult").GetComponent<TextMeshProUGUI>();
        TxtRewards = BattleResultUI.transform.Find("TxtReward").GetComponent<TextMeshProUGUI>();
        BtnOk = BattleResultUI.transform.Find("BtnOk").GetComponent<Button>();
        BtnOk.onClick.AddListener(() => OnBtnOkClick());
    }

    private void OnBtnOkClick()
    {
        SceneManager.LoadScene(GameData.Instance.CurrentSceneIndex, LoadSceneMode.Single);
    }

    private bool CheckAlive()
    {
        bool isAllAllyDeath = true;
        bool isAllEnemyDeath = true;

        Debug.Log("Checking Ally Characters");
        foreach (Character character in AllyCharacterList)
        {
            Debug.Log($"Ally Character {character.Name} isAlive: {character.IsAlive()}");
            if (character.IsAlive())
            {
                isAllAllyDeath = false;
                break;
            }
        }

        Debug.Log("Checking Enemy Characters");
        foreach (Character character in EnemyCharacterList)
        {
            Debug.Log($"Enemy Character {character.Name} isAlive: {character.IsAlive()}");
            if (character.IsAlive())
            {
                isAllEnemyDeath = false;
                break;
            }
        }

        if (!isAllAllyDeath && !isAllEnemyDeath)
        {
            Debug.Log("Both allies and enemies are alive.");
            return true;
        }

        if (isAllAllyDeath)
        {
            Notation.text = "Tất cả Trợ thủ phe ta đã bị tiêu diệt!";
            TxtBattleResult.text = "THẤT BẠI";
            TxtRewards.text = "Bạn nhận được:<br>Một lời chia buồn sâu sắc";
            BattleResultUI.SetActive(true);
        }

        if (isAllEnemyDeath)
        {
            Notation.text = "Tất cả đối thủ đã bị tiêu diệt!";
            if (!IsRewardGet)
            {
                TxtBattleResult.text = "CHIẾN THẮNG";

                List<Reward> rewards = GameData.Instance.BattleData.GetReward();
                string txtReward = "Bạn nhận được:<br>";
                foreach (Reward reward in rewards)
                {
                    Debug.Log($"{reward.Quantity} {reward.RewardCode}");
                    txtReward += $"{reward.Name} x {reward.Quantity}<br>";

                    switch (reward.RewardType)
                    {
                        case RewardConstants.RewardType.ITEM:

                            var existingItem = GameData.Instance.PlayerData.ItemDataList
                                .FirstOrDefault(item => item.ItemCode == reward.RewardCode);

                            if (existingItem != null)
                            {
                                existingItem.Quantity += reward.Quantity;
                            }
                            else
                            {
                                GameData.Instance.PlayerData.ItemDataList.Add(new ItemData()
                                {
                                    InventoryIndex = GameData.Instance.PlayerData.ItemDataList.Count,
                                    ItemCode = reward.RewardCode,
                                    Quantity = reward.Quantity
                                });
                            }
                            break;
                        case RewardConstants.RewardType.CHARACTER:
                            GameData.Instance.PlayerData.CharacterDataList.Add(new CharacterData()
                            {
                                InventoryIndex = GameData.Instance.PlayerData.CharacterDataList.Count,
                                CharacterCode = reward.RewardCode,
                                CharacterExp = 0,
                                SpawnLocation = -1
                            });
                            break;
                    }
                }
                if (rewards.Count == 0)
                {
                    txtReward += "Một lời chúc bạn may mắn lần sau";
                }
                TxtRewards.text = txtReward;

                IsRewardGet = true;
            }
            BattleResultUI.SetActive(true);
        }

        Debug.Log("Not all allies or enemies are alive.");
        return false;
    }

    /// <summary>
    /// Những settings chung cho hệ thống.
    /// </summary>
    private void SystemSettings()
    {
        Application.targetFrameRate = 30;
    }

    /// <summary>
    /// Bắt đầu trận đấu sau khi đã <b>Load</b> và <b>Setup</b> xong.
    /// </summary>
    private void StartBattle()
    {
        BattleState = BattleState.IN_TURN;

        Notation.text = $"Bắt đầu";
    }

    private void InTurn()
    {
        // Sort the Characters by Speed
        List<(Character character, bool isEnemy)> allCharacterList = SortAllCharacterBySpeedDesc();

        bool isHaveAction = false;
        foreach (var tuple in allCharacterList)
        {
            Character character = tuple.character;
            if (character.IsInTurn && character.IsAlive())
            {
                BattleInput.Self = character;
                ActiveSkill skill = character.UseActiveSkill();
                if (skill != null)
                {
                    if (tuple.isEnemy)
                    {
                        BattleInput.AllyCharacterList = EnemyCharacterList;
                        BattleInput.EnemyCharacterList = AllyCharacterList;
                    }
                    else
                    {
                        BattleInput.AllyCharacterList = AllyCharacterList;
                        BattleInput.EnemyCharacterList = EnemyCharacterList;
                    }
                    BattleOutput = skill.SelectTarget(BattleInput);

                    if (BattleOutput.EnemyCharacterList.Count > 0)
                    {
                        ToDoSkill = skill;

                        Notation.text = $"{character.Name} sử dụng Skill {skill.Name} lên {BattleOutput.EnemyCharacterList[0].Name}";

                        ActiveChar = tuple.character.CharacterPrefab;
                        FromPosition = ActiveChar.transform.position;
                        ToPosition = BattleOutput.EnemyCharacterList[0].CharacterPrefab.transform.position;

                        BattleInput.EnemyCharacterList.Add(BattleOutput.EnemyCharacterList[0]);

                        IsMovingToward = true;
                        character.IsInTurn = false;
                        isHaveAction = true;

                        BattleState = BattleState.PLAYING;
                        break;
                    }
                }
            }
        }
        if (!isHaveAction)
        {
            // No Character is able to move
            BattleState = BattleState.WAITING_FOR_NEXT_TURN;
        }
    }

    // TODO: Clean hàm về animation ... PENDING ...
    private void PerformAction()
    {
        float speed = 20f;

        if (IsMovingCompleted)
        {
            IsMovingCompleted = false;
            BattleState = BattleState.IN_TURN;
        }
        else
        {
            if (IsMovingToward)
            {
                ActiveChar.transform.position = Vector3.MoveTowards(ActiveChar.transform.position, ToPosition, speed * Time.deltaTime);
                if (ActiveChar.transform.position == ToPosition)
                {
                    IsMovingToward = false;
                    List<Character> targetCharacters = ToDoSkill.UseSkill(BattleInput);


                    List<GameObject> instantiatedEffects = new List<GameObject>();

                    // perform effect
                    foreach (Character targetCharacter in targetCharacters)
                    {
                        GameObject effect = Instantiate(AttackEffect, targetCharacter.SpawnPoint.position, targetCharacter.SpawnPoint.rotation);
                        instantiatedEffects.Add(effect);
                        AudioSource.clip = AttackSound;
                        AudioSource.Play();
                    }

                    StartCoroutine(WaitAndDestroyGameObjects(1f, instantiatedEffects));
                }
            }
            else
            {
                ActiveChar.transform.position = Vector3.MoveTowards(ActiveChar.transform.position, FromPosition, speed * Time.deltaTime);
                if (ActiveChar.transform.position == FromPosition)
                {
                    IsMovingToward = true;
                    IsMovingCompleted = true;
                }
            }
        }
    }

    private List<(Character character, bool isEnemy)> SortAllCharacterBySpeedDesc()
    {
        List<(Character character, bool isEnemy)> allCharacterList = new();

        foreach (var character in AllyCharacterList)
        {
            allCharacterList.Add((character, false));
        }

        foreach (var character in EnemyCharacterList)
        {
            allCharacterList.Add((character, true));
        }

        // Sort the Characters by Speed
        allCharacterList = allCharacterList.OrderByDescending(tuple =>
        {
            Character character = tuple.character;
            Stat statSpeed = character.CharacterStats.StatList[(int)CharacterStatType.SPD];
            return statSpeed.Current + statSpeed.Bonus;
        }).ToList();

        return allCharacterList;
    }

    private void NewTurn()
    {
        foreach (var character in AllyCharacterList)
        {
            character.IsInTurn = true;
        }

        foreach (var character in EnemyCharacterList)
        {
            character.IsInTurn = true;
        }

        BattleState = BattleState.IN_TURN;
    }

    /// <summary>
    /// Spawn tất cả Character phe địch hoặc phe đồng minh lên trên màn hình.
    /// </summary>
    /// <param name="characterList">Tất cả Character phe nào đó.</param>
    /// <param name="spawnPointList">Spawn Point tương ứng với phe đó.</param>
    /// <param name="isOpposite">Cho biết là thuộc phe nào.</param>
    private void SpawnCharacters(List<Character> characterList, Transform spawnPointList, bool isOpposite)
    {
        foreach (var character in characterList)
        {
            character.SpawnPoint = spawnPointList.GetChild(character.SpawnLocation);
            character.SpawnCharacter(character.SpawnPoint, isOpposite);
            character.SpawnPoint.Find("Canvas").Find("Red").gameObject.SetActive(true);
            character.SpawnPoint.Find("Canvas").Find("Healthbar").gameObject.SetActive(true);
            character.TextHP = character.SpawnPoint.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
            character.HealthBar = character.SpawnPoint.Find("Canvas").Find("Healthbar");
            ShowTextHP(character);
        }
    }

    private void ShowTextHP(Character character)
    {

        character.TextHP.text =
            $"{character.CharacterStats.StatList[(int)CharacterStatType.HP].Current}" +
            $"/" +
            $"{character.CharacterStats.StatList[(int)CharacterStatType.HP].Base}";
    }

    IEnumerator WaitAndDestroyGameObjects(float second, List<GameObject> gameObjects)
    {
        yield return new WaitForSeconds(second);

        foreach (GameObject item in gameObjects)
        {
            Destroy(item);
        }
    }
}
