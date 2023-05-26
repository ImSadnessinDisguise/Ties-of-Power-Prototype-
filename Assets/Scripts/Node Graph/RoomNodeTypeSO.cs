using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "RoomNodeType_", menuName = "ScriptableObject/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Special Rooms Identifier
    #region Header
    [Header("Only flag the RoomNodeType that should be visible in the editor")]
    #endregion
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("One type should be a corridor")]
    #endregion
    //corridor identifier
    public bool isCorridor;
    #region Header
    [Header("One type should be a corridor NS")]
    #endregion
    public bool isCorridorNS;
    #region Header
    [Header("One type should be a corridor EW")]
    #endregion
    public bool isCorridorEW;
    #region Header
    [Header("One type should be an Entrance")]
    #endregion
    public bool isEntrance;
    #region Header
    [Header("One type should be a Boss Room")]
    #endregion
    public bool isBossRoom;
    #region Header
    [Header("One type should be a rest room")]
    #endregion
    public bool isRestRoom;
    #region Header
    [Header("One type should be null")]
    #endregion
    public bool isNone;

    #endregion

    #region Validation 
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName); 
    }
#endif
    #endregion 
}

