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

    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("Room Configuration")]
    #endregion Header ROOM CONFIGURATION

    public RoomNodeTypeSO roomNodeType;

    public Vector2Int lowerBounds;

    public Vector2Int upperBounds;

    [SerializeField] public List<Doorway> doorwayList;


    public Vector2Int[] spawnPositionArray;


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

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        //Check Spawan Position Populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
#endif
        #endregion

    }

}

