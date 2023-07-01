using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("Base Enemy Details")]
    #endregion
    #region Tooltip
    [Tooltip("The name of the enemy")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("The prefab for the enemy")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip 
    [Tooltip("Distance to the player before the enemy starts chasing")]
    #endregion
    public float chaseDistance = 50f;

    #region Tooltip
    [Tooltip("Firing Interval")]
    #endregion
    public float firingIntervalMin = 0.1f;

    #region Tooltip
    [Tooltip("Firing Interval Max")]
    #endregion
    public float firingIntervalMax = 1f;

    #region Tooltip
    [Tooltip("Min Firing Duration")]
    #endregion
    public float firingDurationMin = 1f;

    #region Tooltip
    [Tooltip("Max Firing Duration")]
    #endregion
    public float firingDurationMax = 2f;

    #region Tooltip
    [Tooltip("Line of Sight")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ENEMY MATERIAL
    [Space(10)]
    [Header("Enemy Material")]
    #endregion
    #region Tooltip
    [Tooltip("This is the standard lit shader material for the enemy")]
    #endregion
    public Material enemyStandardMaterial;

    #region HEADER ENEMY HEALTH
    [Space(10)]
    [Header("Enemy Health")]
    #endregion
    #region Tooltip
    [Tooltip("The Health of the enemy in each level")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;
    #region Tooltip
    [Tooltip("Is it immune after getting hit")]
    #endregion
    public bool isImmunneAfterHit = false;
    #region Tooltip
    [Tooltip("Immunity Time")]
    #endregion
    public float hitImmunityTime;

    #region Header Materialize Settings
    [Space(10)]
    [Header("Enemy Materialize Settings")]
    #endregion
    #region Tooltip
    [Tooltip("The time in seconds that it takes the enemy to materialize")]
    #endregion
    public float enemyMaterializeTime;
    #region Tooltip
    [Tooltip("The shader to be used when the enemy materialize")]
    #endregion
    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    #region Tooltip
    [Tooltip("The color to use when the enemy materialize")]
    #endregion
    public Color enemyMaterializeColor;

    #region Validation
#if UNITY_EDITOR
    //Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
    }
#endif
    #endregion


}
