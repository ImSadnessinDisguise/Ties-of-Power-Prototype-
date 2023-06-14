using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Deatils")]
public class PlayerDetailsSO : ScriptableObject 
{
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("Player Base Details")]
    #endregion

    #region Tooltip
    [Tooltip("PLayer Character Name")]
    #endregion
    public string playerCharacterName;

    #region Tooltp
    [Tooltip("Prefab for player Game Object")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("Player runtime animator controller")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header Health
    [Space(10)]
    [Header("Health")]
    #endregion
    #region Tooltip 
    [Tooltip("Player Starting Health Ammount")]
    #endregion
    public int playerHealthAmount;


    #region Header Other
    [Space(10)]
    [Header("Other")]
    #endregion
    #region Tooltip
    [Tooltip("Player Icon Sprite")]
    #endregion
    public Sprite playerMinimapIcon;


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMinimapIcon), playerMinimapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
    }
#endif
    #endregion
}
