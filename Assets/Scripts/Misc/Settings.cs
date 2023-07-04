using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region Units
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixel = 16f;
    #endregion

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuiltAttemptForRoomGraph = 1000;
    public const int maxDungeonBuildAttempt = 10;
    #endregion 

    #region ROOM SETTINGS
    public const float fadeInTime = 0.05f; // time to fade in the room
    public const int maxChildCorridor = 3; //Highest number should be 3 as to not fail dungeon generation
    public const float doorUnlockDelay = 1f;
    #endregion

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultAstarMovementPenalties = 20;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathFindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion

    #region ANIMATOR PARAMETERS
    //Animator Parameters - Player
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");

    //Animator Paramters - Doors
    public static int open = Animator.StringToHash("open");
    #endregion

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI PARAMETERS
    public const float uiHeartSpacing = 16f;
    #endregion

    #region GAMEOBJECT TAG
    public const string playerTag = "Player";
    #endregion

    #region AUDIO
    public const float musicFadeOutTime = 0.5f;
    public const float musicFadeInTime = 0.5f;
    #endregion
}
