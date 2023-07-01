using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Layer the bullets will hit")]
    #endregion

    [SerializeField] private Transform weaponShootPosition;
    [SerializeField] private LayerMask layerMask;

    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    public float firingDurationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.enemyDetails;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        firingIntervalTimer -= Time.deltaTime;

        if (firingIntervalTimer < 0f)
        {
           if (firingDurationTimer >= 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }

    }

    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    private void FireWeapon()
    {
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);
    }
}
