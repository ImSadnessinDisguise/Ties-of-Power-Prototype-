using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuiltAttemptForRoomGraph = 1000;
    public const int maxDungeonBuildAttempt = 10;
    #endregion 

    #region ROOM SETTINGS

    //Highest number should be 3 as to not fail dungeon generation
    public const int maxChildCorridor = 3;


    #endregion

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultAstarMovementPenalties = 20;
    #endregion
}
