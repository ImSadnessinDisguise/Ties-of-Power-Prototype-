using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccesful;

    private void OnEnable()
    {
        //set dimmed material to off
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        //set dimmed material to fully visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        //Load Room Node Type List
        LoadRoomNodeTypeList();

        //Set dimmed material to fully visible 

    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    ///<summary>
    /// Generate random dungeon, returns true if dungeon built, false if failed
    /// </summary>

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        //Load the scriptable object room template in the dictionary
        LoadRoomTemplateDictionary();

        dungeonBuildSuccesful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccesful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempt)
        {
            dungeonBuildAttempts++;

            //select random room node graph from the list
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuiltAttemptForNodeGraph = 0;
            dungeonBuildSuccesful = false;

            //Loop until succesfully built or more than max attempt for node graph
            while (!dungeonBuildSuccesful && dungeonRebuiltAttemptForNodeGraph <= Settings.maxDungeonRebuiltAttemptForRoomGraph)
            {
                //Clear dungeon room gameObject and dungeon room dictionary
                ClearDungeon();

                dungeonRebuiltAttemptForNodeGraph++;

                //Attempt to build the dungeon for the selected room node graph
                dungeonBuildSuccesful = AttemptToBuildRandomDungeon(roomNodeGraph);

                if (dungeonBuildSuccesful)
                {
                    //Instantiate Room GameObejcts
                    InstantiateRoomGameobjects();
                }
            }
        }
        return dungeonBuildSuccesful;

    }

    /// <summary>
    /// Load the room template into the dictionary
    /// </summary>>
    private void LoadRoomTemplateDictionary()
    {
        //Clear room template dictionary
        roomTemplateDictionary.Clear();

        //Load room tempplate list into dictionary
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In" + roomTemplateList);
            }
        }
    }

    /// <summary>
    /// Attempt to randomly build the dungeon for the specified room node graph
    /// return true if successful random layout was generated, else return false
    /// if a problem was encountered and another attempt is required
    /// </summary>>
    
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        //create open room node queue
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        //Add Entrance node to the queue 
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));
        
        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false; //Dungeon not built
        }

        //start with no room overlap
        bool noRoomOverlap = true;

        //process oprn room node queue
        noRoomOverlap = ProcessRoomInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlap);

        //if all room node has been process and there is no overlap then return true
        if (openRoomNodeQueue.Count == 0 && noRoomOverlap)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Process room in the open room node queue, returning true if there is no room overlap
    /// </summary>

    private bool ProcessRoomInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlap)
    {
        // While room node in open room node queue & no room overlap detected
        while (openRoomNodeQueue.Count > 0 && noRoomOverlap == true)
        {
            //get next room node from open room node queue
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            //Add child room node to queue from room node graph(with links to this parent room)
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode); 
            }

            // if the room is the entrance mark as positioned and add room to dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                //add room to room dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                //else get parent room for node 
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                //see if room can be placed without overlap
                noRoomOverlap = CanPlaceRoomWithNoOverlap(roomNode, parentRoom);
            }

        }

        return noRoomOverlap;
    }

    ///<summary>
    ///Attempt to place the room node in the dungeon - if room can be placed return the room, else return null
    ///</summary>
    private bool CanPlaceRoomWithNoOverlap(RoomNodeSO roomNode, Room parentRoom)
    {
        //Initialise and assume overlap until proven otherwise
        bool roomOverlap = true;

        //do while room overlap -try to place against all available doorways of the parent until
        //room is sucessfully placed without overlap
        while (roomOverlap)
        {
            //select random unconnected available doorway for parent
            List<Doorway> unconnectedAvailableParentDoorway = GetUnconnectedAvailableDoorway(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorway.Count == 0)
            {
                //if no more doorways to try then overlap failure
                return false; //room overlaps
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorway[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorway.Count)];

            //get a random room template for room node that is consistent with the parent door orientation 
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            //Create a room
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            //Place the room without overlap
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlap = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlap = true;
            }
        }
        return true;
    }

    ///<summary>
    ///place room - returns true if room doesnt overlap, false otherwise
    ///</summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        //current doorway position
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // return if no doorway in room opposite to parent doorway
        if (doorway == null)
        {
            //mark doorway as unavailable to not connect
            doorwayParent.isUnavailable = true;

            return false;
        }

        //calculate world grid parent doorway position 
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        switch(doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        //calulate room lower bounds and upper bounds based on positioning to align with parent doorway
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            //dorway mark as connected and unavailable
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            //return to show room connected with no overlap
            return true;
        }
        else
        {
            doorwayParent.isUnavailable = true;
            return false;
        }
    }

    ///<summary>
    ///Check room overlapping upper and lower bounds
    ///</summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            if(IsOverlappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        return null;
    }

    /// <summary>
    /// Check if 2 rooms overlap each other 
    /// </summary>
    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else return false;
    }

    ///<summary>
    /// Check if interval 1 overlap interval 2 
    /// </summary>
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    ///<summary>
    ///get doorway from doorway list that has opposite orientation to doorway
    ///</summary>
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {
        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }
        return null;
    }

    ///<summary>
    ///Get a random room template for room node taking account orientation
    ///</summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        //if room node is a corridor then select random correct corridor 
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }
        return roomTemplate;
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graph
    /// </summary>>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphsList)
    {
        if (roomNodeGraphsList.Count > 0 )
        {
            return roomNodeGraphsList[UnityEngine.Random.Range(0, roomNodeGraphsList.Count)];
        }
        else
        {
            Debug.Log("No Room Node Graph In List");
            return null;
        }
    }

    /// <summary>
    /// create room based on roomTemplate and layoutNode, return the created room
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        //Initialise room from template
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        //Set Parent ID for Room
        if (roomNode.parentRoomNodeIDList.Count == 0) //Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            //Set Entrance in game manager
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        //if there is any enemies to spawn then default to be clear of enemies
        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        {
            room.isClearedOfEnemies = true;
        }

        return room;


    }

    ///<summary>
    ///Get a random room template from the room template list that matches the roomType and return it
    ///return null if no matching room template is found
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // add matching room template
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // add matching room template
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        //return null if list is zero 
        if (matchingRoomTemplateList.Count == 0)
            return null;

        //select random rom template from list and return
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
        


    }

    /// <summary>
    /// Get unconnected doorway
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorway(List<Doorway> roomDoorwayList)
    {
        //Loop through doorway list
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    ///<summary>
    ///instantiate the dungeon room gameobject from prefab
    /// </summary>
    private void InstantiateRoomGameobjects()
    {
        //iterate through all dungeon room
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            //calculate room position 
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            //instantiate room
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            //get instantiated room component from instantiated prefab
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            //initialized the instantiated room
            instantiatedRoom.Initialize(roomGameobject);

            //save gameobject reference
            room.instantiatedRoom = instantiatedRoom;
        }
    }


    ///<summary>
    ///get a room template by room template id, return null if Id doesnt exist
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    ///<summary>
    ///Get room by roomID, if no room exist with that id return null
    /// </summary>
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Clear dungeon room game objects and dungeon room dictionary
    /// </summary>
    private void ClearDungeon()
    {
        //Destroy Instantiated Dungeon Game
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
            dungeonBuilderRoomDictionary.Clear();
        }
    }

    ///<summary>
    ///create deep copy of string list
    ///</summary>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// <summary>
    /// Create deep copy of doorway list
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }



}
