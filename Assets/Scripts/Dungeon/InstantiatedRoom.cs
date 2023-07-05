using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty; 
    [HideInInspector] public Bounds roomColliderBounds;
    
    private BoxCollider2D boxCollider2D;

    #region Header Object Reference
    [Space(10)]
    [Header("Object Reference")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with environment child placeholder game object")]
    #endregion
    [SerializeField] private GameObject environmentGameObject;


    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        //save collider bounds
        roomColliderBounds = boxCollider2D.bounds;
    }

    //Trigger room chaged event when players enters a room
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player triggers the collider
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            //set room as visited
            this.room.isPreviouslyVisited = true;

            //call room changed event
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialize(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorways();

        AddDoorsToRoom();

        DisableCollisionTilemapRenderer();

        AddObstacleAndPreferredPath();
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        //get grid component
        grid = roomGameobject.GetComponentInChildren<Grid>();

        //get tilemap in children
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach(Tilemap tilemap in tilemaps)
        {
             if (tilemap.gameObject.tag == "Ground")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decorations1")
            {
                decoration1Tilemap = tilemap;
            }
            if (tilemap.gameObject.tag == "collision")
            {
                collisionTilemap = tilemap;
            }
            if (tilemap.gameObject.tag == "MiniMap Tilemap")
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// Block Off Unused Doorways in the room
    /// </summary>
    private void BlockOffUnusedDoorways()
    {
        //loop though doorways
        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;

            //Block off unused doorways using tiles on tilemap
            if (collisionTilemap != null)
            {
                BlockDoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockDoorwayOnTilemapLayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockDoorwayOnTilemapLayer(groundTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Block Doorway on tilemap layer
    /// </summary>
    private void BlockDoorwayOnTilemapLayer (Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
        }
            
    }

    /// <summary>
    /// Block Doorways Horizontally
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        //loop through all tiles to copy
        for (int Xpos = 0; Xpos < doorway.doorwayCopyTileWidth; Xpos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                //get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + Xpos, startPosition.y - yPos, 0));

                //Copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + Xpos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x +
                    Xpos, startPosition.y - yPos, 0)));

                //Set rotation tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + Xpos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Block Doorways Vertically
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        //loop through all ties to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // Get tile rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                //Copy Tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x +
                   xPos, startPosition.y - yPos, 0)));

                //Set rotation tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Update Obstacles used by Astar Pathfinding
    /// </summary>
    private void AddObstacleAndPreferredPath()
    {
        //this array will be populated with wall obstacles
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        //Loop through all grid squares
        for (int x  = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                //Set default movement penalty for grid squares
                aStarMovementPenalty[x, y] = Settings.defaultAstarMovementPenalties;

                //add obstacles for collision tiles enemy cant walk on
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollsionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                //Add a preferred path for enemies (1 is the prefered path value, default value
                //for a grid location is specified in the setting)
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    /// <summary>
    /// Add opening doors if this is not a corridor room
    /// </summary>
    private void AddDoorsToRoom()
    {
        // if this room is a corridor then return
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        //Instatntiate door prefab at doorway positions
        foreach (Doorway doorway in room.doorWayList)
        {
            // if doorway prefab isnt null and doorway is connected
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixel / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    // Create a door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    //Create a door with room as the parent
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    //create door with room as parent
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);

                }
                else if (doorway.orientation == Orientation.west)
                {
                    //create door with romm as parent
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                //Get door component
                Door doorComponent = door.GetComponent<Door>();

                //set if door is a part of boss room
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    //lock the door to prevent access
                    doorComponent.LockDoor();
                }

            }
        }
    }
    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    /// <summary>
    /// Disable the room trigger collider that is used to trigger when the players enters a room
    /// </summary>
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }

    /// <summary>
    /// Enable room collider
    /// </summary>
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(true);
        }
    }

    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Lock the doors
    /// </summary>
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        //Trigger the lock
        foreach (Door door in doorArray)
        {
            //Trigger lock doors
            door.LockDoor();
        }

        //Disable Room collider
        DisableRoomCollider();
    }

    /// <summary>
    /// Unlock door
    /// </summary>
    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        //trigger open doors
        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        //enable room collider trigger
        EnableRoomCollider();
    }

}
