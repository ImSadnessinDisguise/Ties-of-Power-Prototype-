using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
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
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    private void Start()
    {
        previousGamestate = Gamestate.gameStarted;
        gameState = Gamestate.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState(); 

        //for testing
        if (Input.GetKeyDown(KeyCode.Return))
        {
            gameState = Gamestate.gameStarted;
        }
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

        Debug.Log("Boss Stage - Find and destroy the boss");
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

        Debug.Log("Game Lost");

        yield return new WaitForSeconds(10f);

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