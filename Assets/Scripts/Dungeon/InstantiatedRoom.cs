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

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        //save collider bounds
        roomColliderBounds = boxCollider2D.bounds;
    }

    public void Initialize(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

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
            if (tilemap.gameObject.tag == "MiiniMap Tilemap")
            {
                minimapTilemap = tilemap;
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

    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
