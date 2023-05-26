using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("Basic Level Details")]
    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip
    [Tooltip("The name of the level")]
    #endregion Tooltip

    public string levelName;

    #region Header ROOM TEMPLATE FOR LEVEL
    [Space(10)]
    [Header("Room Template For Level")]
    #endregion Header ROOM TEMPLATE FOR LEVEL

    #region Tooltip
    [Tooltip("Populate the list with the room templates that you want ot be part of the level. You need to ensure that room template are included" +
        "all room node types that are specified in the Room Node Graph per level")]
    #endregion Tooltip
    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPH FOR LEVEL
    [Space(10)]
    [Header("Room Node Graph For Level")]
    #endregion Header ROOM NODE GRAPH FOR LEVEL
    #region Tooltip
    [Tooltip("Populate this list with the room node graphs which should be randomly selected for the level")]
    #endregion Tooltip
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR

    //Validate Scriptable Object Details
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        //check to make sure room templates are specified for all the room node types in the specified node graph

        //first check north/south corridor, east/ west corridor and entrance type have been specified

        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false; 

        //loop through all room templates to check that this node type has been specified
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + ": No EW corridor specified");
        }

        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + ": No NS corridor specified");
        }

        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + ": No Entrance specified");
        }

        //Loop through all node graph
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            //loop through all nodes in the node graph
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                //check room template has been specified for each room node type

                //corridors and entrance are checked
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS ||
                    roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                //Loop through all room types to check that this node type has been specified
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateList == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;


                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + "No room Template" + roomNodeSO.roomNodeType.name.ToString() + "found for node graph"
                        + roomNodeGraph.name.ToString());
                }

            }
        }
    }

#endif
    #endregion 
}
