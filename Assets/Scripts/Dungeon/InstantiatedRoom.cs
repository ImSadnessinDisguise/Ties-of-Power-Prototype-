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


    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
