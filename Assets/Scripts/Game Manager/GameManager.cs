using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header GAMEOBJECT REFERENCES
    #region Tooltip
    [Tooltip("Populate with pause menu gameobject in hierarchy")]
    #endregion Tooltip
    [SerializeField] private GameObject pauseMenu;
    #region Tooltip
    [Tooltip("Populate with the MessageText textmeshpro component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    #region Tooltip
    [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;


    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("Dungeon Levels")]
    #endregion Header DUNGEON LEVELS
    #region tooltip
    [Tooltip("Populate with dungeon level scriptable object")]
    #endregion tooltip
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region tooltip

    [Tooltip("Poulate with the starting dungeon level for testing, first level = 0")]

    #endregion tooltip
    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private mainPlayer player;

    [HideInInspector] public Gamestate gameState;
    [HideInInspector] public Gamestate previousGamestate;
    private InstantiatedRoom bossRoom;

    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    /// <summary>
    /// Create Player in scene at position
    /// </summary>
    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<mainPlayer>();

        player.Initialize(playerDetails);
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        // Subscribe to player destroyed event
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        // Unubscribe from player destroyed event
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    /// <summary>
    /// Handle player destroyed event
    /// </summary>
    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGamestate = gameState;
        gameState = Gamestate.gameLost;
    }

    private void Start()
    {
        previousGamestate = Gamestate.gameStarted;
        gameState = Gamestate.gameStarted;

        //Set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

    }

    private void HandleGameState()
    {
        //Handle game state
        switch (gameState)
        {
            case Gamestate.gameStarted:

                //Play First Level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = Gamestate.playingLevel;

                RoomEnemiesDefeated();
                break;

            case Gamestate.levelCompleted:
                StartCoroutine(LevelCompleted());
                break;

            case Gamestate.gameWon:

                if (previousGamestate != Gamestate.gameWon)
                    StartCoroutine(GameWon());

                break;

            //Handle game being lost
            case Gamestate.gameLost:

                if(previousGamestate != Gamestate.gameLost)
                {
                    StopAllCoroutines(); //Prevent messages if you clear the level as you get killed
                    StartCoroutine(GameLost());
                }
                
                break;

            //Restart the game
            case Gamestate.restartGame:

                RestartGame();

                break;
        }
    }

    public mainPlayer GetPlayer()
    {
        return player;
    }

    public Sprite GetPlayerMinimapIcon()
    {
        return playerDetails.playerMinimapIcon;
    }

    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void RoomEnemiesDefeated()
    {
        //Initialise dungeon as being cleared
        bool isDungeonClearofRegularEnemies = true;
        bossRoom = null;

        //Loop through all dungeon room if cleared of enemies
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            //Skip boss room for time being
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            //check if other room is cleared of enemies
            if(!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearofRegularEnemies = false;
                break;
            }
        }

        //Set game state
        if ((isDungeonClearofRegularEnemies && bossRoom == null) || (isDungeonClearofRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            // Are there any dungeon levels then
            if (currentDungeonLevelListIndex < dungeonLevelList.Count -1)
            {
                gameState = Gamestate.levelCompleted;
            }
            else
            {
                gameState = Gamestate.gameWon;
            }
        }
        else if (isDungeonClearofRegularEnemies)
        {
            gameState = Gamestate.bossStage;
            StartCoroutine(BossStage());
        }

    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        //Build Dungeon level
        bool dungeonBuiltSuccesfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccesfully)
        {
            Debug.Log("cannot build dungeon from specified rooms and node graph");
        }

        //call static event that room has changed
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        //set player position roughly mid room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y
            + currentRoom.upperBounds.y) / 2f, 0f);

        // get nearest spawn point in room nearest to player
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        StartCoroutine(DisplayDungeonLevelText());
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        //Set Screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "Area " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList
            [currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        //Fade In
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));

    }

    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        //Set text
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        if(displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;
        messageTextTMP.SetText("");
    }

    /// <summary>
    /// Enterr Boss Area
    /// </summary>
    private IEnumerator BossStage()
    {
        //activate boss room
        bossRoom.gameObject.SetActive(true);

        //UNlock boss room
        bossRoom.UnlockDoors(0f);

        //Wait for 2 Secondss
        yield return new WaitForSeconds(2f);

        //fade in canvas to display text message
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        //Display boss message
        yield return StartCoroutine(DisplayMessageRoutine("Well Done" + "\n\nNow Find the boss and free your friend", Color.white, 5f));

        //Set Fade out
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    private IEnumerator LevelCompleted()
    {
        //Play next Level
        gameState = Gamestate.playingLevel;

        yield return new WaitForSeconds(2f);

        // Fade in canvas to display text message
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // Display level completed
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE MANAGED TO FREE YOUR COMRADE", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN\n\nTO MOVE FURTHER NEAR THE CASTLE", Color.white, 5f));

        // Fade out canvas
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        currentDungeonLevelListIndex++;


        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backGroundColor)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backGroundColor;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }

    private IEnumerator GameWon()
    {
        previousGamestate = Gamestate.gameWon;

        Debug.Log("Game Won");

        //wayt for 10 seconds
        yield return new WaitForSeconds(10f);

        //Load Next Scene

    }

    private IEnumerator GameLost()
    {
        previousGamestate = Gamestate.gameLost;

        // Disable player
        GetPlayer().playerControl.DisablePlayer();

        // Wait 1 seconds
        yield return new WaitForSeconds(1f);

        // Fade Out
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // Disable enemies (FindObjectsOfType is resource hungry - but ok to use in this end of game situation)
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        // Display game lost
        yield return StartCoroutine(DisplayMessageRoutine("You have fallen in battle " + "\n\nPrepare further to challenge Illoit", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART TO THE CAMP", Color.white, 0f));

        gameState = Gamestate.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("Main Base");
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// Get the current dungeon level
    /// </summary>
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion

}