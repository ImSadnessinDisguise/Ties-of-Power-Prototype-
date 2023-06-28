using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("Movement Details")]
    #endregion

    #region Tooltip
    [Tooltip("The minimum movespeed. The GetMoveSpeed method calculates a random value between minimum and maximum")]
    #endregion
    public float minMoveSpeed = 8f;

    #region Tooltip
    [Tooltip("The maximum movespeed. The GetMoveSpeed method calculates a random value between minimum and maximum")]
    #endregion
    public float maxMoveSpeed = 8f;

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values
    /// </summary>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }


    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);
    }
#endif
    #endregion
}
