using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Dungeon/Room")]

public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB
    [Space(10)]
    [Header("Room Prefab")]


    #endregion

    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; //used to regenerate guid if SO is copied and prefab is changed

    #region HEADER ROOM MUSIC
    [Space(10)]
    [Header("ROOM MUSIC")]
    #endregion

    #region Tooltip
    [Tooltip("The battle music SO when the room hasnt been cleared of enemies")]
    #endregion
    public MusicTrackSO battleMusic;

    #region Tooltip
    [Tooltip("Ambient music SO when the room has been cleared of enemies")]
    #endregion
    public MusicTrackSO ambientMusic;

    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("Room Configuration")]
    #endregion Header ROOM CONFIGURATION

    public RoomNodeTypeSO roomNodeType;

    public Vector2Int lowerBounds;

    public Vector2Int upperBounds;

    [SerializeField] public List<Doorway> doorwayList;

    public Vector2Int[] spawnPositionArray;


    #region Header Enemy Details
    [Space(10)]
    [Header("Enemy Details")]
    #endregion
    #region Tooltip
    [Tooltip("Populate List with all enemies that can be spawned in this room by dungeon level. including the random ratio of the enemy type")]
    #endregion
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;

    #region Tooltip
    [Tooltip("Populate the list with spawn parameters for the enemies")]
    #endregion
    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    /// <summary>
    /// Returns the list of Entrances for the Room
    /// <summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region validation

#if UNITY_EDITOR

    //Validate SO fields

    private void OnValidate()
    {
        //set unique guid if empty or prefab changes

        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);


        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        //Check enemies and room spawn parameters for levels
        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);

            foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel), roomEnemySpawnParameters.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn),
                    roomEnemySpawnParameters.minTotalEnemiesToSpawn, nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn),
                    roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minConcurrentEnemies),
                    roomEnemySpawnParameters.minConcurrentEnemies, nameof(roomEnemySpawnParameters.maxConcurrentEnemies),
                    roomEnemySpawnParameters.maxConcurrentEnemies, false);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minSpawnInterval), roomEnemySpawnParameters.minSpawnInterval,
                    nameof(roomEnemySpawnParameters.maxSpawnInterval), roomEnemySpawnParameters.maxSpawnInterval, true);

                bool isEnemyTypeListForDungeonLevel = false;
  
                //Validate EnemyType List
                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectsByLevel in enemiesByLevelList)
                {
                    if (dungeonObjectsByLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel &&
                        dungeonObjectsByLevel.spawnableObjectRatioList.Count > 0)
                        isEnemyTypeListForDungeonLevel = true;

                    HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectsByLevel.dungeonLevel), dungeonObjectsByLevel.dungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectsByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject), dungeonObjectRatio.dungeonObject);

                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio), dungeonObjectRatio.ratio, false);
                    }
                }
                if (isEnemyTypeListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("No enemy types specified in dungeon level" + roomEnemySpawnParameters.dungeonLevel.levelName + "in gameObject"
                        + this.name.ToString());
                }
            }
        }

        //Check Spawan Position Populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
#endif
        #endregion

    }

}

