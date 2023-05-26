using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIDList = new List<string>();
    public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    public RoomNodeTypeSO roomNodeType;

    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    //Intialise Code
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Room Node";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        //load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    //Draw node with nodestyle

    public void Draw(GUIStyle nodeStyle)
    {
        //Draw node box using begin area
        GUILayout.BeginArea(rect, nodeStyle);

        //start region to detect popup selection changes
        EditorGUI.BeginChangeCheck();

        //if the room node has a parent or is type entrance the label if not popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            //Display popup using room node type name values that can be selected from (default to the currently set roomNodeType)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // If room rtpe selection has changed making child connections pottentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor
                && roomNodeTypeList.list[selection].isCorridor || roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        //Get Child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        //child room node is not null
                        if (childRoomNode != null)
                        {
                            RemoveChildIDFromRoomNode(childRoomNode.id);

                            childRoomNode.RemoveParentIDFromRoomNode(id);

                        }
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
 
    }

    //Populate a string array with the room nodes display that can be selected
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }

    //Process Events for the node
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process mouse down events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                    break;

            //process Mouse Up events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            //Process Mouse Drag events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    private void ProcessMouseDownEvent (Event currentEvent)
    {
        //left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }

        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        //toggle node selection

        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    //process mouse up event

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //If left click up
        if (currentEvent.button == 0) 
        { 
            ProcessLeftClickUpEvent();
        }
    }
    
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    //process mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //process left click drag event
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    //add childID to the node (returns true if the node has been added false otherwise)
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (isChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true; 
        }
        return false;   
    }

    public bool isChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        //check if there is already connected to a boss room
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        //if child node has a type of boss room and there is already a connected boss room node then returns false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        //if the child has a type of none then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        //If the node already  has a child with this child id return false
        if (childRoomNodeIDList.Contains(childID))
            return false;

        if (id == childID)
            return false;

        //if child already has parentID in list return false
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        //if the child node already has a parent return false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        //if child is a corridor and this node is a corridor return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        //if adding a corridor check this node has the permitted maximum permitted child corridor
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridor)
            return false;

        //if the child room is an entrance return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        //one room corridor one room
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;


        return true;
    }
    //add parentID to the node 
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
         parentRoomNodeIDList.Add(parentID);
        return true;
    }

    //remove childID from node
    public bool RemoveChildIDFromRoomNode(string childID)
    {
        //if node contains child remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentIDFromRoomNode (string parentID)
    {
        //if the room node contains the parent Id then remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

     

#endif
  
    #endregion
}
