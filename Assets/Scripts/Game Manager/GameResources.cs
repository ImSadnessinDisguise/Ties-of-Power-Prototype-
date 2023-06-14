using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    #region Header PLAYER
    [Space(10)]
    [Header("Player")]
    #endregion
    #region Tooltip
    [Tooltip("used to reference player in between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header Materials
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPecial Tilemap Tiles")]
    #endregion
    #region Tooltip
    [Tooltip("Collsion tiles that the enemy can navigate to")]
    #endregion
    public TileBase[] enemyUnwalkableCollsionTilesArray;
    #region Tooltip
    [Tooltip("Prefered path for enemy navigation")]
    #endregion
    public TileBase preferredEnemyPathTile;

    #region Validation
#if UNITY_EDITOR
    //validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollsionTilesArray), enemyUnwalkableCollsionTilesArray); 
    }
#endif
    #endregion
}
