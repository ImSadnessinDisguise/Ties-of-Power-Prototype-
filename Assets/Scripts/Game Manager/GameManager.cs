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
    [HideInInspector] public Gamestate gameState;

    private void Start()
    {
        gameState = Gamestate.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState(); 

        //for testing
        if (Input.GetKeyDown(KeyCode.R))
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

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        //Build Dungeon level
        bool dungeonBuiltSuccesfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccesfully)
        {
            Debug.Log("cannot build dungeon from specified rooms and node graph");
        }
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