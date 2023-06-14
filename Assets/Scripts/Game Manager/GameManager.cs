using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
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
        }
    }

    public mainPlayer GetPlayer()
    {
        return player;
    }
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }


    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        //Build Dungeon level
        bool dungeonBuiltSuccesfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccesfully)
        {
            Debug.Log("cannot build dungeon from specified rooms and node graph");
        }

        //set player position roughly mid room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y
            + currentRoom.upperBounds.y) / 2f, 0f);

        // get nearest spawn point in room nearest to player
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
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