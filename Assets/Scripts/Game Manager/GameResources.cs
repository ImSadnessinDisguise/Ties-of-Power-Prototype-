using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

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

    #region Header SOUNDS
    [Space(10)]
    [Header("Sounds")]
    #endregion Header
    #region Tooltip
    [Tooltip("Populate with sounds master mixer group")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;

    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the music master mixer group")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    #region Tooltip
    [Tooltip("Music on full snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnFullSnapShot;
    #region Tooltip
    [Tooltip("Music low snapshot")]
    #endregion
    public AudioMixerSnapshot musicLowSnapShot;
    #region Tooltip
    [Tooltip("Music off snapshot")]
    #endregion
    public AudioMixerSnapshot musicOffSnapShot;

    #region HEADER SOUNDEFFECT
    [Space(10)]
    [Header("SOUND EFFECT")]
    #endregion
    public SoundEffectSO doorSound;
    public SoundEffectSO swordSound;
    public SoundEffect canonSound;
    public SoundEffect bigCanonSound;

    #region Header Materials
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;
    #region Tooltip
    [Tooltip("Sprite Lit Default Material")]
    #endregion
    public Material litMaterial;
    #region Tooltip
    [Tooltip("Populate with variable lit shader")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Populate with the materialize shader")]
    #endregion
    public Shader materializeShader;

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

    #region HEADER CHEST
    [Space(10)]
    [Header("Chest")]
    #endregion
    #region Tooltip
    [Tooltip("Chest Item Prefab")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("Populate with Heart Icon Sprite")]
    #endregion
    public Sprite heartIcon;

    #region HEADER UI PARAMETERS
    [Space(10)]
    [Header("UI")]
    #endregion HEADER UI PARAMETERS
    #region Tooltip
    [Tooltip("Populate with heart image prefab")]
    #endregion
    public GameObject heartPrefab;

    #region Header PARTICLE EFFECTS
    [Space(10)]
    [Header("PARTICLE EFFECTS")]
    #endregion
    public GameObject bloodSplat;
    





    #region Validation
#if UNITY_EDITOR
    //validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFullSnapShot), musicOnFullSnapShot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLowSnapShot), musicLowSnapShot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOffSnapShot), musicOffSnapShot);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollsionTilesArray), enemyUnwalkableCollsionTilesArray); 
    }
#endif
    #endregion
}
