using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    public static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header Dungeon
    [Space(10)]
    [Header("Dungeon")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with dungeon RoomNodeTypeList")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header Materials
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;


}
